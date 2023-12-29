using System;
using System.Linq;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AM.OpenehrProfile.DataTypes.Quantity;
using OpenEhr.AM.OpenehrProfile.DataTypes.Text;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.Common.Generic;
using OpenEhr.RM.Composition;
using OpenEhr.RM.Composition.Content;
using OpenEhr.RM.Composition.Content.Entry;
using OpenEhr.RM.Composition.Content.Navigation;
using OpenEhr.RM.DataStructures.History;
using OpenEhr.RM.DataStructures.ItemStructure;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Quantity.DateTime;
using OpenEhr.RM.DataTypes.Text;
using OpenEhr.Futures.OperationalTemplate;

namespace GastrOs.Sde.Engine
{
    /// <summary>
    /// Really a temporary class for generating default RM type values for
    /// constraints
    /// 
    /// TODO CR assign archetype details to each generated stuff
    /// </summary>
    public static class RmFactory
    {
        private static CodePhrase language = new CodePhrase("en-NZ", "LCID");
        public static CodePhrase Language
        {
            get { return language; }
            set { language = value; }
        }

        public const string DummyCodedValue = "atxxxx";
        public const string RmVersion = "1.0.1";

        public static bool RecursivelyGenerateClusters { get; set; }
        public static bool RecursivelyGenerateSections { get; set; }
        public static bool RecursivelyGenerateTrees { get; set; }
        public static bool RecursivelyGenerateHistory { get; set; }
        public static bool RecursivelyGenerateCompositions { get; set; }

        static RmFactory()
        {
            RecursivelyGenerateClusters = true;
            RecursivelyGenerateSections = true;
            RecursivelyGenerateTrees = true;
            RecursivelyGenerateCompositions = true;
            RecursivelyGenerateHistory = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static Locatable Instantiate(string archetypeId, CArchetypeRoot opt)
        {
            CArchetypeRoot archetypeRoot = opt.LocateArchetypeById(archetypeId);
            if (archetypeRoot != null)
                return Instantiate(archetypeRoot);
            return null;
        }

        /// <summary>
        /// A crude temporary replacement of CObject.DefaultValue(), since right now its
        /// implementation seems to be incomplete.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static Locatable Instantiate(CComplexObject constraint)
        {
            if (constraint.RmTypeMatches<Element>())
            {
                return InstantiateElement(constraint);
            }
            if (constraint.RmTypeMatches<Cluster>())
            {
                return InstantiateCluster(constraint);
            }
            if (constraint.RmTypeMatches<ItemTree>())
            {
                return InstantiateTree(constraint);
            }
            if (constraint.RmTypeMatches<History<ItemStructure>>())
            {
                return InstantiateHistory(constraint);
            }
            if (constraint.RmTypeMatches<Observation>())
            {
                return InstantiateObservation(constraint);
            }
            if (constraint.RmTypeMatches<Evaluation>())
            {
                return InstantiateEvaluation(constraint);
            }
            if (constraint.RmTypeMatches<Section>())
            {
                return InstantiateSection(constraint);
            }
            if (constraint.RmTypeMatches<Composition>())
            {
                return InstantiateComposition(constraint);
            }
            
            //Known possibilities have been exhausted.
            throw new NotSupportedException("Sorry... RM type "+constraint.RmTypeName+" is not yet supported.");
        }

        public static Element InstantiateElement(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<Element>());
            Element element = new Element(new DvText(constraint.ExtractOntologyText()),
                                          AomHelper.ExtractArchId(constraint), null, null, AomHelper.ExtractArchetyped(constraint), null, null, null);

            CObject elemValueConst = constraint.ExtractElemValueConstraint();
            if (elemValueConst.RmTypeMatches<DvCodedText>())
            {
                CCodePhrase codePhrase = ((CComplexObject)elemValueConst).ExtractCodePhrase();
                //TODO not the best way - think of something better!
                element.Value = new DvCodedText(DummyCodedValue, "", codePhrase.TerminologyId.Value);
            }
            else if (elemValueConst.RmTypeMatches<DvQuantity>()) 
            {
                string unit = ((CDvQuantity)elemValueConst).ExtractUnits().FirstOrDefault();
                element.Value = new DvQuantity(0, unit ?? "");
            }
            else if (elemValueConst.RmTypeMatches<DvDateTime>())
            {
                element.Value = new DvDateTime(new DateTime());
            }
            else
            {
                element.Value = OpenEhr.Factories.RmFactory.DataValue(elemValueConst.RmTypeName);
            }

            return element;
        }

        public static Cluster InstantiateCluster(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<Cluster>());
            Cluster cluster = new Cluster(new DvText(constraint.ExtractOntologyText()),
                                          AomHelper.ExtractArchId(constraint), null, null, AomHelper.ExtractArchetyped(constraint), null, new Item[0]);

            if (RecursivelyGenerateClusters)
            {
                foreach (CComplexObject childConstraint in constraint.ExtractChildConstraints())
                {
                    Item child = Instantiate(childConstraint) as Item;
                    if (child != null)
                    {
                        cluster.Items.Add(child);
                        //Special case: if this element is part of core concept, then set its presence value to "Null"
                        if (childConstraint.DenotesPresence())
                        {
                            //We know this cast is safe - implied by the above condition being true
                            Element elemChild = child as Element;
                            elemChild.SetPresenceValue(childConstraint, PresenceState.Null);
                        }
                    }
                }
            }

            return cluster;
        }

        public static ItemTree InstantiateTree(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<ItemTree>());
            ItemTree tree = new ItemTree(new DvText("data"), AomHelper.ExtractArchId(constraint), null, null, AomHelper.ExtractArchetyped(constraint), null, new Item[0]);
            if (RecursivelyGenerateTrees)
            {
                foreach (CComplexObject childConstraint in constraint.ExtractChildConstraints())
                {
                    Item child = Instantiate(childConstraint) as Item;
                    if (child != null)
                    {
                        tree.Items.Add(child);
                    }
                }
            }
            return tree;
        }

        public static History<ItemStructure> InstantiateHistory(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<History<ItemStructure>>());
            History<ItemStructure> hist = new History<ItemStructure>(new DvText(constraint.ExtractOntologyText()),
                                                                     AomHelper.ExtractArchId(constraint), null, null, AomHelper.ExtractArchetyped(constraint), null,
                                                                     new DvDateTime(), null, null, new Event<ItemStructure>[0], null);

            if (RecursivelyGenerateHistory)
            {
                foreach (CComplexObject eventConstraint in constraint.ExtractChildConstraints("events"))
                {
                    //NOTE only supports ItemTree's as structures
                    if (!eventConstraint.RmTypeMatches<Event<ItemStructure>>()) continue;

                    ItemTree eventData = null, eventState = null;
                    CComplexObject dataConstraint =
                        eventConstraint.ExtractChildConstraints("data").FirstOrDefault() as CComplexObject;
                    if (dataConstraint != null)
                    {
                        eventData = InstantiateTree(dataConstraint);
                    }
                    CComplexObject evtStateConstraint =
                        eventConstraint.ExtractChildConstraints("state").FirstOrDefault() as CComplexObject;
                    if (evtStateConstraint != null)
                    {
                        eventState = InstantiateTree(evtStateConstraint);
                    }

                    Event<ItemStructure> evt =
                        new PointEvent<ItemStructure>(new DvText(eventConstraint.ExtractOntologyText()),
                                                      AomHelper.ExtractArchId(eventConstraint), null, null,
                                                      AomHelper.ExtractArchetyped(eventConstraint), null,
                                                      new DvDateTime(new DateTime()), eventData, eventState);
                    hist.Events.Add(evt);
                }
            }

            return hist;
        }

        public static Observation InstantiateObservation(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<Observation>());

            CComplexObject historyConstraint = constraint.ExtractChildConstraints("data").FirstOrDefault() as CComplexObject;
            Check.Assert(historyConstraint != null);
            History<ItemStructure> hist = InstantiateHistory(historyConstraint);

            History<ItemStructure> state = null;
            CComplexObject stateConstraint =
                constraint.ExtractChildConstraints("state").FirstOrDefault() as CComplexObject;
            if (stateConstraint != null)
            {
                state = InstantiateHistory(stateConstraint);
            }
            
            ItemTree protocol = null;
            CComplexObject protocolConstraint =
                constraint.ExtractChildConstraints("protocol").FirstOrDefault() as CComplexObject;
            if (protocolConstraint != null)
            {
                protocol = InstantiateTree(protocolConstraint);
            }
            
            return new Observation(new DvText(constraint.ExtractOntologyText()), AomHelper.ExtractArchId(constraint),
                                   null, null, AomHelper.ExtractArchetyped(constraint), null, Language,
                                   new CodePhrase("utf8", "IANA"), new PartyIdentified("Patient"), null, 
                                   null, null, protocol, null, hist, state);
        }

        public static Evaluation InstantiateEvaluation(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<Evaluation>());
            
            CComplexObject treeConstraint = constraint.ExtractChildConstraints().FirstOrDefault() as CComplexObject;
            Check.Assert(treeConstraint != null);
            Check.Assert(treeConstraint.RmTypeMatches<ItemTree>());

            return new Evaluation(new DvText(constraint.ExtractOntologyText()), AomHelper.ExtractArchId(constraint), null, null,
                                  AomHelper.ExtractArchetyped(constraint), null, Language, new CodePhrase("utf8", "IANA"),
                                  new PartyIdentified("Patient"), null, null, null, null, null,
                                  InstantiateTree(treeConstraint));
        }
        
        public static Section InstantiateSection(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<Section>());
            Section section = new Section(new DvText(constraint.ExtractOntologyText()), AomHelper.ExtractArchId(constraint), null, null,
                                          AomHelper.ExtractArchetyped(constraint), null, new ContentItem[0]);
            if (RecursivelyGenerateSections)
            {
                DateTime time = DateTime.Now;
                foreach (CComplexObject childConstraint in constraint.ExtractChildConstraints())
                {
                    ContentItem child = Instantiate(childConstraint) as ContentItem;
                    if (child != null)
                    {
                        section.Items.Add(child);
                        //NOTE temporary "hack" for synchronising the times of observations
                        if (child is Observation)
                        {
                            Observation obs = child as Observation;
                            foreach (Event<ItemStructure> evt in obs.Data.Events)
                                evt.Time = new DvDateTime(time);
                        }
                    }
                }
            }
            return section;
        }

        public static Composition InstantiateComposition(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<Composition>());
            //TODO category, context, territory and composer should be parameterised
            Composition compo = new Composition(new DvText(constraint.ExtractOntologyText()), AomHelper.ExtractArchId(constraint), null,
                                                null, AomHelper.ExtractArchetyped(constraint), null, Language, new CodePhrase("nz", "ISO-3166"),
                                                new DvCodedText("431", "persistent", "openehr"),
                                                new EventContext(new DvDateTime(), null, new DvCodedText("232", "secondary medical care", "openehr"), null, null,
                                                                 null, null), new ContentItem[0],
                                                new PartyIdentified("Composer"));
            
            if (RecursivelyGenerateCompositions)
            {
                foreach (CComplexObject childConstraint in constraint.ExtractChildConstraints("content"))
                {
                    ContentItem child = Instantiate(childConstraint) as ContentItem;
                    if (child != null)
                    {
                        compo.Content.Add(child);
                    }
                }
            }
            return compo;
        }

        public static PointEvent<ItemStructure> InstantiatePointEvent(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<Event<ItemStructure>>());
            PointEvent<ItemStructure> evt = new PointEvent<ItemStructure>(new DvText(constraint.ExtractOntologyText()),
                                                                          AomHelper.ExtractArchId(constraint), null, null, 
                                                                          AomHelper.ExtractArchetyped(constraint), null,
                                                                          new DvDateTime(new DateTime()), null, null);
            CComplexObject dataConstraint = constraint.ExtractChildConstraints("data").FirstOrDefault() as CComplexObject;
            if (dataConstraint != null)
                evt.Data = InstantiateTree(dataConstraint);
            CComplexObject stateConstraint = constraint.ExtractChildConstraints("state").FirstOrDefault() as CComplexObject;
            if (stateConstraint != null)
                evt.State = InstantiateTree(stateConstraint);
            return evt;
        }
    }
}