using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GastrOs.Sde.Engine;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Directives;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AM.Archetype.Ontology;
using OpenEhr.AM.OpenehrProfile.DataTypes.Quantity;
using OpenEhr.AM.OpenehrProfile.DataTypes.Text;
using OpenEhr.RM.Common.Archetyped;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.Common.Resource;
using OpenEhr.RM.Composition;
using OpenEhr.RM.Composition.Content;
using OpenEhr.RM.Composition.Content.Entry;
using OpenEhr.RM.Composition.Content.Navigation;
using OpenEhr.RM.DataStructures.History;
using OpenEhr.RM.DataStructures.ItemStructure;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Text;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.RM.Impl;
using Archetyped=OpenEhr.RM.Common.Archetyped.Impl.Archetyped;

namespace GastrOs.Sde.Support
{
    /// <summary>
    /// Provides reusable (extension) methods for extracting information from OpenEHR archetype objects
    /// </summary>
    public static class AomHelper
    {
        public const string DecimalPattern = @"\d+(?:\.\d+)?";
        public static readonly Regex DescriptionPattern = new Regex("(?:("+DecimalPattern+@")\s*;\s*)?(.*)");

        private static PropertyInfo constraintParent;
        private static Dictionary<Type, string> rmTypeNameCache;
        private static HashSet<Type> supportedContainerTypes;

        static AomHelper()
        {
            constraintParent = typeof(ArchetypeConstraint).GetProperty("ConstraintParent",
                                                                       BindingFlags.Instance | BindingFlags.NonPublic);
            Check.Assert(constraintParent != null);
            //stores a cache of frequently used rm type names, since reflection is expensive
            rmTypeNameCache = new Dictionary<Type, string>();
            rmTypeNameCache[typeof(Composition)] = RmType.GetRmTypeName(typeof(Composition));
            rmTypeNameCache[typeof(Section)] = RmType.GetRmTypeName(typeof(Section));
            rmTypeNameCache[typeof(Evaluation)] = RmType.GetRmTypeName(typeof(Evaluation));
            rmTypeNameCache[typeof(Observation)] = RmType.GetRmTypeName(typeof(Observation));
            rmTypeNameCache[typeof(Event<ItemStructure>)] = RmType.GetRmTypeName(typeof(Event<ItemStructure>));
            rmTypeNameCache[typeof(ItemTree)] = RmType.GetRmTypeName(typeof(ItemTree));
            rmTypeNameCache[typeof(Element)] = RmType.GetRmTypeName(typeof(Element));
            rmTypeNameCache[typeof(Cluster)] = RmType.GetRmTypeName(typeof(Cluster));
            rmTypeNameCache[typeof(Item)] = RmType.GetRmTypeName(typeof(Item));
            rmTypeNameCache[typeof(DvText)] = RmType.GetRmTypeName(typeof(DvText));
            rmTypeNameCache[typeof(DvCodedText)] = RmType.GetRmTypeName(typeof(DvCodedText));
            rmTypeNameCache[typeof(DvQuantity)] = RmType.GetRmTypeName(typeof(DvQuantity));
            rmTypeNameCache[typeof(DvCount)] = RmType.GetRmTypeName(typeof(DvCount));
            rmTypeNameCache[typeof(DvBoolean)] = RmType.GetRmTypeName(typeof(DvBoolean));

            supportedContainerTypes = new HashSet<Type>();
            supportedContainerTypes.Add(typeof(Composition));
            supportedContainerTypes.Add(typeof(Section));
            supportedContainerTypes.Add(typeof(Evaluation));
            supportedContainerTypes.Add(typeof(Observation));
            supportedContainerTypes.Add(typeof(History<ItemStructure>));
            supportedContainerTypes.Add(typeof(PointEvent<ItemStructure>));
            supportedContainerTypes.Add(typeof(Event<ItemStructure>));
            supportedContainerTypes.Add(typeof(ItemTree));
            supportedContainerTypes.Add(typeof(Cluster));
        }

        #region(directives processing)
        /// <summary>
        /// Extracts the directives part of the given OpenEHR template.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static DirectiveStore ExtractDirectives(this OperationalTemplate template)
        {
            DirectiveStore directives = new DirectiveStore();

            if (template.Annotations != null)
            {
                foreach (Annotation annotation in template.Annotations)
                {
                    CObject constraint;
                    try
                    {
                        constraint = template.Definition.ConstraintAtPath(annotation.Path);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Warning: constraint at path " + annotation.Path + " not found.", e);
                        continue;
                    }

                    string annotationText = annotation.Items.Item(GastrOsConfig.EngineConfig.AnnotationIdForDirectives);

                    try
                    {
                        foreach (IDirective directive in DirectiveHelper.Parse(annotationText))
                        {
                            directives.AddDirectiveFor(constraint, directive);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Attempting to parse directives from text '" + annotationText + "' failed", e);
                        continue;
                    }
                }
            }

            return directives;
        }

        /// <summary>
        /// Extracts a specific type of directive for specific constraint within operational template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static TDirective FindDirectiveOfType<TDirective>(this OperationalTemplate template, CObject constraint)
            where TDirective : class, IDirective
        {
            foreach (Annotation annotation in template.Annotations)
            {
                try
                {
                    if (constraint == template.Definition.ConstraintAtPath(annotation.Path))
                    {
                        string annotationText = annotation.Items.Item(GastrOsConfig.EngineConfig.AnnotationIdForDirectives);
                        var dir = DirectiveHelper.Parse(annotationText).FirstOrDefault(d => d is TDirective) as TDirective;
                        if (dir != null)
                            return dir;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Warning: processing annotation at " + annotation.Path + " failed", e);
                    continue;
                }
            }
            return null;
        }

        private static void visitAttributeDoNothing(CAttribute attr, CArchetypeRoot root, object[] args)
        {
            //Do nothing.
        }
        #endregion

        #region(simple retrievals)
        /// <summary>
        /// Finds the archetype root object with given achetype id. Returns null
        /// if not found
        /// </summary>
        /// <param name="id"></param>
        /// <param name="archRoot"></param>
        /// <returns></returns>
        public static CArchetypeRoot LocateArchetypeById(this CArchetypeRoot archRoot, string id)
        {
            if (archRoot == null)
                throw new ArgumentNullException("opt");
            if (id == null)
                throw new ArgumentNullException("id");
            IDictionary<string, CArchetypeRoot> foundRoots = RetrieveAllArchetypeRoots(archRoot);
            if (foundRoots.ContainsKey(id))
                return foundRoots[id];
            return null;
        }

        /// <summary>
        /// Retrieves all archetype roots underneath the given op. template and returns
        /// a dictionary that maps each found archetype root's id to itself
        /// </summary>
        /// <param name="archRoot"></param>
        /// <returns></returns>
        public static IDictionary<string, CArchetypeRoot> RetrieveAllArchetypeRoots(this CArchetypeRoot archRoot)
        {
            if (archRoot == null)
                throw new ArgumentNullException("opt");
            IDictionary<string, CArchetypeRoot> foundRoots = new Dictionary<string, CArchetypeRoot>();
            SimpleAomVisitor.Visit(archRoot, visitCObjectForRetrieveRoots, visitAttributeDoNothing, foundRoots);
            return foundRoots;
        }

        private static void visitCObjectForRetrieveRoots(CObject obj, CArchetypeRoot root, object[] args)
        {
            IDictionary<string, CArchetypeRoot> foundRoots = (IDictionary<string, CArchetypeRoot>)args[0];
            if (obj is CArchetypeRoot)
            {
                CArchetypeRoot currentRoot = (CArchetypeRoot) obj;
                foundRoots[currentRoot.ArchetypeId.Value] = currentRoot;
            }
        }

        /// <summary>
        /// Convenience method for obtaining the parent of a constraint object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CObject GetParent(this CObject obj)
        {
            CAttribute parentAtt = obj.Parent;
            if (parentAtt != null)
            {
                try
                {
                    return constraintParent.GetValue(parentAtt, null) as CObject;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the archetype root that this constraint object belongs to
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CArchetypeRoot GetArchetypeRoot(this CObject obj)
        {
            for (CObject parent = obj; parent != null; parent = GetParent(parent))
            {
                if (parent is CArchetypeRoot)
                    return parent as CArchetypeRoot;
            }
            return null;
        }

        /// <summary>
        /// Retrieves a CAttribute from a CComplexObject by its name
        /// </summary>
        /// <param name="cObj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CAttribute GetAttributeByName(this CComplexObject cObj, string name)
        {
            if (cObj.Attributes == null)
                return null;
            foreach (CAttribute attr in cObj.Attributes)
            {
                if (attr.RmAttributeName.Equals(name))
                    return attr;
            }
            return null;
        }
        #endregion

        #region(complex retrievals)
        /// <summary>
        /// Extracts child constraints from given constraint's first attribute
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICollection<CObject> ExtractChildConstraints(this CComplexObject obj)
        {
            List<CObject> children = new List<CObject>();
            CAttribute items = ExtractSingleAttribute(obj);
            if (items != null && items.Children != null)
            {
                foreach (CObject child in items.Children)
                {
                    //NOTE don't handle unfilled archetype slots
                    if (child is ArchetypeSlot) continue;
                    children.Add(child);
                }
            }
            return children;
        }

        /// <summary>
        /// Extracts child constraints from given constraint's given attribute
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static ICollection<CObject> ExtractChildConstraints(this CComplexObject obj, string attributeName)
        {
            List<CObject> children = new List<CObject>();
            CAttribute items = GetAttributeByName(obj, attributeName);
            if (items != null && items.Children != null)
            {
                foreach (CObject child in items.Children)
                {
                    //NOTE don't handle unfilled archetype slots
                    if (child is ArchetypeSlot) continue;
                    children.Add(child);
                }
            }
            return children;
        }

        /// <summary>
        /// Extracts the value portion of the given constraint, assuming it is on
        /// an ELEMENT type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CObject ExtractElemValueConstraint(this CComplexObject obj)
        {
            Check.Require(obj.RmTypeMatches<Element>(),
                          "Attempt to extract value constraint where the RM type is not ELEMENT");

            CAttribute value = ExtractSingleAttribute(obj);
            if(value != null && value.Children != null)
                return value.Children.FirstOrDefault();
            return null;
        }

        /// <summary>
        /// Extracts a single attribute from given constraint - if it has any. Otherwise
        /// returns null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CAttribute ExtractSingleAttribute(this CComplexObject obj)
        {
            if (obj.Attributes != null)
                return obj.Attributes.FirstOrDefault();
            return null;
        }

        /// <summary>
        /// Extracts units from a given object of RM type DV_QUANTITY
        /// </summary>
        /// <returns></returns>
        public static ICollection<string> ExtractUnits(this CDvQuantity quant)
        {
            List<string> units = new List<string>();
            foreach (CQuantityItem quantItem in quant.List)
            {
                units.Add(quantItem.Units);
            }
            return units;
        }

        /// <summary>
        /// Extracts ontology information (text and description) for given code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="arch"></param>
        /// <returns></returns>
        public static OntologyItem ExtractOntology(string code, CArchetypeRoot arch)
        {
            Check.Require(arch.TermDefinitions != null);
            if (!arch.TermDefinitions.HasKey(code))
                return null;
            ArchetypeTerm term = arch.TermDefinitions.Item(code);
            OntologyItem ontology = new OntologyItem(code);
            if (term.Items.HasKey("text"))
                ontology.Text = term.Items.Item("text");
            if (term.Items.HasKey("description"))
            {
                //description is formatted as [Ordinal;]Desc
                string rawDescription = term.Items.Item("description");
                Match m = DescriptionPattern.Match(rawDescription);
                if (m.Groups.Count < 3)
                {
                    //there must be three capturing groups (including the whole string)
                    //otherwise regular expression is wrong
                    ontology.Description = rawDescription;
                }
                else
                {
                    float parsed;
                    //the first capturing group must be a decimal indicating ordinal
                    if (Single.TryParse(m.Groups[1].Value, out parsed))
                        ontology.Ordinal = parsed;
                    //the second capturing group must be the remaining part of the description
                    ontology.Description = m.Groups[2].Value;
                }
            }
            if (term.Items.HasKey("annotation"))
                ontology.Annotation = term.Items.Item("annotation");
            return ontology;
        }

        /// <summary>
        /// Convenience method for extracting the ontology associated with given
        /// complex object
        /// </summary>
        /// <param name="cObject"></param>
        /// <returns></returns>
        public static OntologyItem ExtractOntology(this CObject cObject)
        {
            return ExtractOntology(cObject.NodeId, GetArchetypeRoot(cObject));
        }

        /// <summary>
        /// Convenience method for extracting the text portion of the ontology
        /// associated with given code
        /// </summary>
        /// <returns></returns>
        public static string ExtractOntologyText(string code, CArchetypeRoot arch)
        {
            OntologyItem ontology = ExtractOntology(code, arch);
            if (ontology == null)
                return null;
            return ontology.Text;
        }

        /// <summary>
        /// Convenience method for extracting the text portion of the ontology
        /// associated with given complex object
        /// </summary>
        /// <param name="cObject"></param>
        /// <returns></returns>
        public static string ExtractOntologyText(this CObject cObject)
        {
            OntologyItem ontology = ExtractOntology(cObject);
            if (ontology == null)
                return null;
            return ontology.Text;
        }

        /// <summary>
        /// Extracts the CCodePhrase portion of given complex object of RM type
        /// DV_CODED_TEXT
        /// </summary>
        /// <param name="cObj"></param>
        /// <returns></returns>
        public static CCodePhrase ExtractCodePhrase(this CComplexObject cObj)
        {
            Check.Require(cObj != null);
            Check.Require(cObj.RmTypeMatches<DvCodedText>());
            Check.Require(cObj.Attributes.Count == 1);

            CAttribute attr = cObj.Attributes.First();

            Check.Assert(attr.RmAttributeName.Equals("defining_code"));
            Check.Assert(attr.Children != null && attr.Children.Count == 1);

            CObject child = attr.Children.First;
            Check.Assert(child.RmTypeName.Equals("CODE_PHRASE"));
            Check.Assert(child is CCodePhrase);

            return child as CCodePhrase;
        }

        public static string ExtractArchId(this CComplexObject constraint)
        {
            return constraint is CArchetypeRoot ? (constraint as CArchetypeRoot).ArchetypeId.Value : constraint.NodeId;
        }

        public static Archetyped ExtractArchetyped(this CComplexObject constraint)
        {
            CArchetypeRoot root = constraint.GetArchetypeRoot();
            return new Archetyped(root.ArchetypeId, RmFactory.RmVersion, root.TemplateId);
        }
        #endregion

        #region(parent-child relationships)
        /// <summary>
        /// Adds specified child instance to parent instance.
        /// In doing so, ensures that the child has a unique name among its
        /// new siblings
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="childConstraint"></param>
        public static bool AddChild(this Pathable parent, Locatable child, CComplexObject childConstraint)
        {
            //Right now only limited to these RM types
            Check.Require(supportedContainerTypes.Contains(parent.GetType()),
                          "Sorry, don't yet support adding children to RM type " + parent.GetType());

            child.AssignUniqueName(parent, childConstraint);
            if (parent is Cluster)
            {
                Check.Require(child is Item);
                Cluster parentCluster = parent as Cluster;
                parentCluster.Items.Add(child as Item);
                return true;
            }
            if (parent is ItemTree)
            {
                Check.Require(child is Item);
                ItemTree parentTree = parent as ItemTree;
                parentTree.Items.Add(child as Item);
                return true;
            }
            if (parent is History<ItemStructure>)
            {
                Check.Require(child is Event<ItemStructure>);
                History<ItemStructure> parentHist = parent as History<ItemStructure>;
                parentHist.Events.Add(child as Event<ItemStructure>);
            }
            if (parent is Section)
            {
                Check.Require(child is ContentItem);
                Section section = parent as Section;
                section.Items.Add(child as ContentItem);
                return true;
            }

            //TODO do it for other container types
            return false;
        }

        /// <summary>
        /// Basically assigns a unique id suffix to the name by brute-force searching for existing id's in
        /// would-be-parent's children...
        /// </summary>
        /// <param name="parentToBe"></param>
        /// <param name="child"></param>
        /// <param name="childConstraint"></param>
        /// <returns></returns>
        public static void AssignUniqueName(this Locatable child, Pathable parentToBe, CComplexObject childConstraint)
        {
            Check.Require(child.LightValidate(childConstraint));
            IEnumerable<Locatable> existingInstances = parentToBe.ChildInstances(childConstraint);
            int uniqueId = 1;
            //This is rather tricky - the reason for wrapping name around a singleton
            //array is so that the delegate function used below has an up-to-date value
            //at each iteration of the loop
            string[] uniqueNames = new string[1];
            uniqueNames[0] = child.Name.Value;
            //translation: while there are instances that already have the name we want
            while (existingInstances.Where(item => String.Equals(item.Name.Value, uniqueNames[0])).Count() > 0)
            {
                uniqueNames[0] = child.Name.Value + "__" + (uniqueId ++);
            }
            child.Name.Value = uniqueNames[0];
        }

        /// <summary>
        /// Remove specified child instance from parent instance
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static bool RemoveChild(this Pathable parent, Locatable child)
        {
            //Right now only limited to these RM types
            Check.Require(supportedContainerTypes.Contains(parent.GetType()),
                          "Sorry, don't yet support removing children from RM type " + parent.GetType());

            if (parent is Cluster)
            {
                Check.Require(child is Item);
                Cluster parentCluster = parent as Cluster;
                parentCluster.Items.Remove(child as Item);
                return true;
            }
            if (parent is ItemTree)
            {
                Check.Require(child is Item);
                ItemTree parentTree = parent as ItemTree;
                parentTree.Items.Remove(child as Item);
                return true;
            }
            if (parent is Section)
            {
                Check.Require(child is ContentItem);
                Section section = parent as Section;
                section.Items.Remove(child as ContentItem);
                return true;
            }

            //TODO do it for other container types
            return false;
        }

        /// <summary>
        /// Generic method for returning the child instances that share the same constraint.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Locatable> ChildInstances(this Pathable parent, CObject constraint)
        {
            return ChildInstances(parent, constraint, null);
        }

        /// <summary>
        /// Generic method for returning the child instances that share the same constraint.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Locatable> ChildInstances(this Pathable parent, CObject constraint, string attribute)
        {
            //Right now only limited to these RM types
            Check.Require(supportedContainerTypes.Contains(parent.GetType()),
                          "Sorry, don't yet support adding children to RM type " + parent.GetType());
            
            IEnumerable children;
            if (parent is Cluster)
            {
                children = (parent as Cluster).Items;
            }
            else if (parent is ItemTree)
            {
                children = (parent as ItemTree).Items;
            }
            else if (parent is Event<ItemStructure>)
            {
                Event<ItemStructure> evt = parent as Event<ItemStructure>;
                if (attribute == null || String.Equals(attribute, "data"))
                    children = evt.Data != null ? new[] {evt.Data} : new Locatable[0];
                else if (String.Equals(attribute, "state"))
                    children = evt.State != null ? new[] {evt.State} : new Locatable[0];
                else
                    throw new ArgumentException("Unknown attribute "+(attribute)+" for RM type EVENT");
            }
            else if (parent is History<ItemStructure>)
            {
                children = (parent as History<ItemStructure>).Events;
            }
            else if (parent is Evaluation)
            {
                children = new Locatable[] {(parent as Evaluation).Data};
            }
            else if (parent is Observation)
            {
                Observation obs = parent as Observation;
                if (attribute == null || String.Equals(attribute, "data"))
                    children = obs.Data != null ? new[] { obs.Data } : new Locatable[0];
                else if (String.Equals(attribute, "state"))
                    children = obs.State != null ? new[] {obs.State} : new Locatable[0];
                else if (String.Equals(attribute, "protocol"))
                    children = obs.Protocol != null ? new[] { obs.Protocol } : new Locatable[0];
                else
                    throw new ArgumentException("Unknown attribute " + (attribute) + " for RM type OBSERVATION");
            }
            else if (parent is Section)
            {
                children = (parent as Section).Items;
            }
            else if (parent is Composition)
            {
                children = (parent as Composition).Content;
            }
            else
            {
                throw new NotSupportedException("Sorry... can't yet retrieve child instances for RM type "+parent.GetType());
            }

            return children.Cast<Locatable>().Where(child => child.LightValidate(constraint));
        }

        /// <summary>
        /// Returns the child items in the cluster that share the same constraint.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Item> ChildInstances(this Cluster parent, CObject constraint)
        {
            return ChildInstances(parent as Locatable, constraint).Cast<Item>();
        }
        #endregion

        #region(presence semantics)
        /// <summary>
        /// Checks whether given constraint denotes a "presence" Element, and if so
        /// returns a mapping from presence semantics to at codes (e.g. presence=null -> at0123) 
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        internal static IDictionary<PresenceState, string> GetPresenceSemantics(this CComplexObject constraint)
        {
            if (constraint == null)
                return null;
            
            //first check at code (OBSOLETE)
            //if (!string.Equals(GastrOsConfig.EngineConfig.PresenceSemantics.PresenceCode, constraint.NodeId))
            //    return null;

            //then validate the constraint further defines the presence codes
            if (!constraint.RmTypeMatches<Element>())
                return null;
            CComplexObject elemValueConstraint = constraint.ExtractElemValueConstraint() as CComplexObject;
            if (elemValueConstraint == null || !elemValueConstraint.RmTypeMatches<DvCodedText>())
                return null;

            //Go through each defined code, and lookup its description -> must match that of
            //the appropriate presence semantics
            Dictionary<string, string> descToCode = new Dictionary<string, string>();
            foreach (string definedCode in elemValueConstraint.ExtractCodePhrase().CodeList)
            {
                OntologyItem ontology = ExtractOntology(definedCode, constraint.GetArchetypeRoot());
                if (ontology == null || ontology.Description == null) continue;
                descToCode[ontology.Description] = definedCode;
            }
            
            Dictionary<PresenceState, string> presenceSemantics = new Dictionary<PresenceState, string>();
            foreach (PresenceState presence in Enum.GetValues(typeof(PresenceState)))
            {
                string desc = GastrOsConfig.EngineConfig.PresenceSemantics.OntologyDescriptionFor(presence);
                if (descToCode.ContainsKey(desc))
                    presenceSemantics[presence] = descToCode[desc];
            }

            //Must contain at least the codes "Present" and "Null"
            if (presenceSemantics.ContainsKey(PresenceState.Present) && presenceSemantics.ContainsKey(PresenceState.Null))
                return presenceSemantics;
            return null;
        }


        /// <summary>
        /// Returns whether the given constraint object denotes the notion of "Present?".
        /// The criteria for this is that 1) it has to be an element with a designated at code
        /// (defined in the app. configuration), 2) it has to contain a dv_coded_text with four
        /// designated code each representing different semantics of "presence"
        /// 
        /// TODO CR test
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static bool DenotesPresence(this CComplexObject constraint)
        {
            return GetPresenceSemantics(constraint) != null;
        }

        /// <summary>
        /// Locates the child (if exists) of the given constraint object that denotes the
        /// notion of "Present?"
        /// 
        /// TODO CR test
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CComplexObject GetPresenceConstraint(this CComplexObject obj)
        {
            ICollection<CObject> children = ExtractChildConstraints(obj);
            //warning: implicit cast from CObject to CComplexObject
            foreach (CComplexObject child in children)
            {
                if (DenotesPresence(child))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the element that represents the "presence" semantics of given
        /// potential coreconcept cluster. If there's no such element (i.e. the
        /// the cluster is not after all a core concept) then returns null.
        /// 
        /// TODO CR test
        /// </summary>
        /// <param name="coreConcept">the (supposed) core concept cluster</param>
        /// <param name="constraint">the constraint for the cluster</param>
        /// <returns></returns>
        public static Element PresenceElement(this Cluster coreConcept, CComplexObject constraint)
        {
            CComplexObject presenceConstraint = constraint.GetPresenceConstraint();
            if (presenceConstraint == null)
                return null;
            return coreConcept.Items.FirstOrDefault(i => LightValidate(i, presenceConstraint)) as Element;
        }

        /// <summary>
        /// TODO CR test
        /// </summary>
        /// <param name="coreConcept"></param>
        /// <param name="constraint"></param>
        /// <param name="presence"></param>
        public static void SetPresence(this Cluster coreConcept, CComplexObject constraint, PresenceState presence)
        {
            CComplexObject presenceConstraint = GetPresenceConstraint(constraint);
            Check.Assert(presenceConstraint != null);
            Element presenceElement = coreConcept.Items.FirstOrDefault(i => LightValidate(i, presenceConstraint)) as Element;
            Check.Assert(presenceElement != null);
            SetPresenceValue(presenceElement, presenceConstraint, presence);
        }

        /// <summary>
        /// Sets the presence value for the element (requires that it carries the
        /// semantics for recording presence in the first place)
        /// </summary>
        /// <param name="presenceElement"></param>
        /// <param name="constraint"></param>
        /// <param name="presence"></param>
        public static void SetPresenceValue(this Element presenceElement, CComplexObject constraint, PresenceState presence)
        {
            DvCodedText presenceValue = presenceElement.Value as DvCodedText;
            Check.Assert(presenceValue != null);
            IDictionary<PresenceState, string> presenceSemantics = GetPresenceSemantics(constraint);
            Check.Assert(presenceSemantics != null);
            presenceValue.Value = presenceSemantics[presence];
        }

        /// <summary>
        /// TODO CR test
        /// </summary>
        /// <param name="coreConcept"></param>
        /// <param name="constraint">constraint for the core concept cluster</param>
        /// <returns></returns>
        public static PresenceState GetPresence(this Cluster coreConcept, CComplexObject constraint)
        {
            CComplexObject presenceConstraint = GetPresenceConstraint(constraint);
            Check.Assert(presenceConstraint != null);
            Element presenceElement = coreConcept.Items.FirstOrDefault(i => LightValidate(i, presenceConstraint)) as Element;
            Check.Assert(presenceElement != null);
            DvCodedText presenceValue = presenceElement.Value as DvCodedText;
            Check.Assert(presenceValue != null);
            IDictionary<PresenceState, string> presenceSemantics = GetPresenceSemantics(presenceConstraint);
            return presenceSemantics.Keys.FirstOrDefault(p => String.Equals(presenceSemantics[p], presenceValue.Value));
        }
        #endregion

        /// <summary>
        /// Does a very "light" validation of the given reference model object against
        /// the given constraint. Just checks whether the constraint specifies the same
        /// RM type as the model instance, and whether they share the same node id's
        /// TODO could also compare hierarchy.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static bool LightValidate(this Locatable value, CObject constraint)
        {
            if (constraint == null)
                throw new ArgumentNullException("constraint");
            if (value == null)
                throw new ArgumentNullException("value");
            //same RM type?
            if (!CObject.IsSameRmType(constraint.RmTypeName, value))
                return false;
            //same archetype?
            if (value.ArchetypeDetails != null && !Equals(value.ArchetypeDetails.ArchetypeId, constraint.GetArchetypeRoot().ArchetypeId))
                return false;
            //If given value is archetype root, then compare archetype id
            if (value.IsArchetypeRoot)
                return String.Equals(value.ArchetypeNodeId, constraint.GetArchetypeRoot().ArchetypeId.Value);
            return String.Equals(value.ArchetypeNodeId, constraint.NodeId);
        }

        /// <summary>
        /// Convenience method for extracting the "value" attribute of an Element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T ValueAs<T>(this Element element) where T : DataValue
        {
            return element.Value as T;
        }

        public static bool IsMandatoryAttribute(this CAttribute attr)
        {
            return attr.Existence.Lower == 1;
        }

        /// <summary>
        /// Returns whether this constraint allows for multiple instances
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static bool MultiplicityAllowed(this CObject constraint)
        {
            return constraint.Occurrences.UpperUnbounded ||
                   constraint.Occurrences.UpperIncluded && constraint.Occurrences.Upper > 1;
        }

        /// <summary>
        /// Returns whether specified constraint has the specified RM type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static bool RmTypeMatches<T>(this CObject constraint) where T : IRmType
        {
            Type rmType = typeof (T);
            string name = rmTypeNameCache.ContainsKey(rmType)
                              ? rmTypeNameCache[rmType]
                              : RmType.GetRmTypeName(rmType);
            return String.Equals(constraint.RmTypeName, name);
        }

        /// <summary>
        /// Works out the path of the constraint relative to the given parent constraint.
        /// E.g. path of constraint "/content/items[at0000]/items[at0001]" relative to
        /// constraint "/content/items[at0000]" is "items[at0001]".
        /// </summary>
        /// <param name="parentConstraint"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static string RelativePath(this CObject constraint, CObject parentConstraint)
        {
            if (!parentConstraint.IsParentOf(constraint))
                throw new ArgumentException("Constraint at path " + constraint.Path +
                                            " is not a child of constraint at path " + parentConstraint.Path);
            
            //Position of the '/'
            int startOfRelativePath = parentConstraint.Path.Length + 1;

            //Means self
            if (startOfRelativePath >= constraint.Path.Length)
                return "";
            return constraint.Path.Substring(startOfRelativePath);
        }

        public static bool IsParentOf(this CObject constraint, CObject potentialChild)
        {
            return potentialChild.Path.IndexOf(constraint.Path) == 0;
        }
    }

    [Flags]
    public enum PresenceState { Null = 0x00, Present = 0x01, Unknown = 0x02, Absent = 0x04}
}