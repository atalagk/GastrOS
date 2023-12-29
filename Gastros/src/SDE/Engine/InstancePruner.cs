using System;
using System.Collections.Generic;
using System.Linq;
using GastrOs.Sde.Support;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.Engine
{
    /// <summary>
    /// "Prunes" value instances by removing nodes that are deemed "empty"
    /// 
    /// TODO There should really be no need for this class - if we don't construct dummy instances in the first place
    /// </summary>
    public static class InstancePruner
    {
        /// <summary>
        /// Prunes the value instance. Returns true if the value instance
        /// is as good as empty.
        /// </summary>
        /// <param name="valueInstance"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static bool Prune(Locatable valueInstance, CComplexObject constraint)
        {
            if (!valueInstance.LightValidate(constraint))
                throw new ArgumentException("Given value instance does not match given constraint");

            if (valueInstance is Cluster)
            {
                Cluster cluster = valueInstance as Cluster;
                //recursively prune children
                foreach (Item item in new List<Item>(cluster.Items))
                {
                    CComplexObject itemConstraint = constraint.ExtractChildConstraints().FirstOrDefault(c => item.LightValidate(c)) as CComplexObject; //TODO CR refactor as LocateMatchingChildConstraint(this CComplexObject constraint, Locatable value)
                    if (itemConstraint == null)
                        continue;
                    Prune(item, itemConstraint);
                    //remove if empty
                    if (IsEmpty(item, itemConstraint))
                    {
                        cluster.Items.Remove(item);
                    }
                }
                return IsEmpty(cluster, constraint);
            }
            if (valueInstance is Element)
            {
                return IsEmpty(valueInstance as Element, constraint);
            }
            return false;
        }

        public static bool IsEmpty(Item item, CComplexObject constraint)
        {
            if (item is Cluster)
            {
                return IsEmpty(item as Cluster, constraint);
            }
            if (item is Element)
            {
                return IsEmpty(item as Element, constraint);
            }
            return false;
        }

        /// <summary>
        /// Returns whether the specified element has no data value or
        /// has a data value equivalent to "nothing"
        /// </summary>
        /// <param name="element"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static bool IsEmpty(Element element, CComplexObject constraint)
        {
            if (element == null)
                return true;
            DataValue value = element.Value;
            if (value == null)
                return true;
            if (value is DvText)
            {
                DvText textValue = value as DvText;
                //in case of DV_TEXT, then empty/null value means "nothing"
                if (string.IsNullOrEmpty(textValue.Value))
                    return true;
                if (textValue is DvCodedText)
                {
                    //in case of DV_CODED_TEXT, then having the special dummy value
                    //"atxxxx" means "nothing"
                    return string.Equals(textValue.Value, RmFactory.DummyCodedValue);
                }
            }
            if (value is DvQuantity)
            {
                //in case of DV_QUANTITY, then zero value means "nothing"
                return ((DvQuantity) value).Magnitude == 0;
            }
            if (value is DvCount)
            {
                //in case of DV_COUNT, then zero value means "nothing"
                return ((DvCount) value).Magnitude == 0;
            }
            return false;
        }

        public static bool IsEmpty(Cluster cluster, CComplexObject constraint)
        {
            if (cluster.Items == null || cluster.Items.Count == 0)
                return true;
            //In the case of core concept, return whether this is not present
            if (constraint.GetPresenceConstraint() != null)
            {
                return cluster.GetPresence(constraint) == PresenceState.Null;
            }
            return false;
        }
    }
}
