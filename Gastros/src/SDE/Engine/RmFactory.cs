using System.Linq;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AM.OpenehrProfile.DataTypes.Quantity;
using OpenEhr.AM.OpenehrProfile.DataTypes.Text;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.Common.Generic;
using OpenEhr.RM.Composition.Content.Entry;
using OpenEhr.RM.DataStructures.History;
using OpenEhr.RM.DataStructures.ItemStructure;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Quantity;
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
        public const string DummyCodedValue = "atxxxx";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static Locatable Instantiate(string archetypeId, OperationalTemplate opt)
        {
            CArchetypeRoot archetypeRoot = opt.LocateArchetypeById(archetypeId);
            if (archetypeRoot != null)
                return Instantiate(archetypeRoot, opt);
            return null;
        }

        /// <summary>
        /// A crude temporary replacement of CObject.DefaultValue(), since right now its
        /// implementation seems to be incomplete.
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static Locatable Instantiate(CComplexObject constraint, OperationalTemplate opt)
        {
            if (constraint.RmTypeMatches<Element>())
            {
                return InstantiateElement(constraint);
            }
            if (constraint.RmTypeMatches<Cluster>())
            {
                return InstantiateCluster(constraint, opt);
            }

            //Known possibilities have been exhausted.
            return null;
        }

        public static Element InstantiateElement(CComplexObject constraint)
        {
            Check.Require(constraint.RmTypeMatches<Element>());
            Element element = new Element(new DvText(constraint.ExtractOntologyText()), 
                                          constraint.NodeId, null, null, null, null, null, null);

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
            else
            {
                element.Value = OpenEhr.Factories.RmFactory.DataValue(elemValueConst.RmTypeName);
            }

            return element;
        }

        public static Cluster InstantiateCluster(CComplexObject constraint, OperationalTemplate opt)
        {
            return InstantiateCluster(constraint, opt, true); //TODO set to false
        }

        public static Cluster InstantiateCluster(CComplexObject constraint, OperationalTemplate opt, bool recursive)
        {
            Cluster cluster = new Cluster(new DvText(constraint.ExtractOntologyText()), 
                                          constraint.NodeId, null, null, null, null, new Item[0]);

            if (recursive)
            {
                foreach (CComplexObject childConstraint in constraint.ExtractChildConstraints())
                {
                    Item child = Instantiate(childConstraint, opt) as Item;
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

        public static Observation InstantiateObservation(CComplexObject constraint, OperationalTemplate opt)
        {
            Observation obs = new Observation();
            obs.Name = new DvText(constraint.ExtractOntologyText());
            obs.ArchetypeNodeId = constraint.NodeId;
            obs.Language = opt.Language;
            obs.Encoding = new CodePhrase("utf8", "IANA");
            obs.Subject = new PartyIdentified("Patient");

            
            History<ItemStructure> hist = new History<ItemStructure>();

            return null;
        }
    }
}