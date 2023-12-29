using System;
using System.Collections.Generic;
using System.Linq;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AM.OpenehrProfile.DataTypes.Quantity;
using OpenEhr.AssumedTypes;
using OpenEhr.DesignByContract;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.Composition.Content.Navigation;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Quantity.DateTime;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.ViewControls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class GridControlBase<T> : TypedViewControl<T, IGridView> where T : Locatable
    {
        protected static Dictionary<string, GridCellType> dataTypeMap = new Dictionary<string, GridCellType>
                                                                            {
                                                                                {"DV_TEXT", GridCellType.Text},
                                                                                {"DV_DATE_TIME", GridCellType.Date},
                                                                                {"DV_COUNT", GridCellType.Int},
                                                                                {"DV_QUANTITY", GridCellType.Float},
                                                                                {"DV_BOOLEAN", GridCellType.Check},
                                                                                {"DV_CODED_TEXT", GridCellType.Choice}
                                                                            };

        private IList<AttributeDescriptor> attributes = new System.Collections.Generic.List<AttributeDescriptor>();

        public GridControlBase(CComplexObject constraint, IEnumerable<AttributeDescriptor> attributeDescriptors)
            : base(constraint)
        {
            Check.Require(constraint.RmTypeMatches<Section>());
            attributes = new System.Collections.Generic.List<AttributeDescriptor>(attributeDescriptors);

            //Make sure each given column child path is valid against section constraint
            foreach (AttributeDescriptor attribute in attributeDescriptors)
            {
                if (attribute is ColumnDescriptor)
                {
                    ColumnDescriptor column = attribute as ColumnDescriptor;
                    column.Constraint = column.Constraint ?? Constraint.ConstraintAtPath(column.ConstraintPath) as CComplexObject;
                    Check.Require(column.Constraint != null);
                    Check.Require(column.Constraint.RmTypeMatches<Element>());
                    string elemValueType = column.Constraint.ExtractElemValueConstraint().RmTypeName;
                    Check.Require(dataTypeMap.ContainsKey(elemValueType), "Sorry - value type " + elemValueType + " not yet supported for grid views!");
                }
                else if (attribute is CompositeDescriptor)
                {
                    CompositeDescriptor composite = attribute as CompositeDescriptor;
                    if (composite.Components == null)
                    {
                        composite.Components = new CComplexObject[composite.ComponentPaths.Length];
                        for (int i = 0; i < composite.ComponentPaths.Length; i++)
                        {
                            CComplexObject componentConstraint = Constraint.ConstraintAtPath(composite.ComponentPaths[i]) as CComplexObject;
                            Check.Require(componentConstraint != null);
                            Check.Require(componentConstraint.RmTypeMatches<Element>());
                            string elemValueType = componentConstraint.ExtractElemValueConstraint().RmTypeName;
                            Check.Require(dataTypeMap.ContainsKey(elemValueType), "Sorry - value type " + elemValueType + " not yet supported for grid views!");
                            composite.Components[i] = componentConstraint;
                        }
                    }
                }
            }
        }

        protected override void SetViewPostexecute(IGridView oldView)
        {
            if (oldView != null)
            {
                oldView.CellUpdated -= UpdateModelWithCellValue;
                oldView.RowAddRequest -= AddItem;
                oldView.RowDeleteRequest -= RemoveItem;
            }

            View.Title = TitleFunction();
            //For each child, add corresponding column to grid
            foreach (AttributeDescriptor attribute in Attributes)
            {
                if (attribute is ColumnDescriptor)
                {
                    ColumnDescriptor column = attribute as ColumnDescriptor;
                    CObject elemConstraint = column.Constraint.ExtractElemValueConstraint();
                    CArchetypeRoot archRoot = elemConstraint.GetArchetypeRoot();
                    IList<OntologyItem> choices = null;
                    //Treat coded texts specially - populate combo boxes
                    if (elemConstraint.RmTypeMatches<DvCodedText>())
                    {
                        choices = new System.Collections.Generic.List<OntologyItem>();
                        choices.Add(new OntologyItem(RmFactory.DummyCodedValue)); //"dummy" - not very ideal!
                        foreach (string code in (elemConstraint as CComplexObject).ExtractCodePhrase().CodeList)
                        {
                            choices.Add(AomHelper.ExtractOntology(code, archRoot));
                        }
                    }
                    View.AddAttribute(new ColumnAttribute(column.Name, GetDataType(elemConstraint.RmTypeName), 
                        true, column.ShowCell, GetDataValueProvider(elemConstraint), choices));
                }
                else if (attribute is CompositeDescriptor)
                {
                    CompositeDescriptor composite = attribute as CompositeDescriptor;
                    View.AddAttribute(new ColumnAttribute(composite.Name, GridCellType.Text, false, true, null));
                }
            }
        }

        private IDataValueProvider GetDataValueProvider(CObject valueConstraint)
        {
            if (valueConstraint.RmTypeMatches<DvCodedText>())
                return new DvCodedTextProvider(valueConstraint as CComplexObject);
            if (valueConstraint.RmTypeMatches<DvText>())
                return new DvTextProvider();
            if (valueConstraint.RmTypeMatches<DvBoolean>())
                return new DvBooleanProvider();
            if (valueConstraint.RmTypeMatches<DvCount>())
                return new DvCountProvider();
            if (valueConstraint.RmTypeMatches<DvQuantity>())
                return new DvQuantityProvider(valueConstraint as CDvQuantity);
            if (valueConstraint.RmTypeMatches<DvDateTime>())
                return new DvDateTimeProvider();
            return null;
        }

        protected abstract void RemoveItem(object sender, GridViewEventArgs e);
        protected abstract void AddItem(object sender, GridViewEventArgs e);
        protected abstract void UpdateModelWithCellValue(object sender, GridViewEventArgs e);

        protected IList<AttributeDescriptor> Attributes { get { return attributes; } }
        protected IList<ColumnDescriptor> Columns { get { return attributes.Where(c => c is ColumnDescriptor).Cast<ColumnDescriptor>().ToList(); } }
        protected IList<CompositeDescriptor> Composites { get { return attributes.Where(c => c is CompositeDescriptor).Cast<CompositeDescriptor>().ToList(); } }
        protected ColumnDescriptor GetColumnByName(string name)
        {
            return Columns.First(c => Equals(c.Name, name));
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        protected static object convertDataValue(Element element)
        {
            if (element.Value is DvBoolean)
            {
                return element.ValueAs<DvBoolean>().Value;
            }
            if (element.Value is DvCodedText)
            {
                return element.ValueAs<DvCodedText>().DefiningCode.CodeString;
            }
            if (element.Value is DvText)
            {
                return element.ValueAs<DvText>().Value;
            }
            if (element.Value is DvDateTime)
            {
                DateTime convertedDate = Iso8601DateTime.ToDateTime(new Iso8601DateTime(element.ValueAs<DvDateTime>().Value));
                if (convertedDate != new DateTime())
                    return convertedDate;
            }
            if (element.Value is DvCount)
            {
                return element.ValueAs<DvCount>().Magnitude;
            }
            if (element.Value is DvQuantity)
            {
                return element.ValueAs<DvQuantity>().Magnitude;
            }
            return null;
        }

        protected static void setDataValue(Element element, CComplexObject elemConstraint, object value)
        {
            CObject valueConstraint = elemConstraint.ExtractElemValueConstraint();
            if (valueConstraint.RmTypeMatches<DvBoolean>())
            {
                element.Value = new DvBoolean(Convert.ToBoolean(value));
            }
            else if (valueConstraint.RmTypeMatches<DvCodedText>())
            {
                element.Value = new DvCodedText(value as string ?? RmFactory.DummyCodedValue);
            }
            else if (valueConstraint.RmTypeMatches<DvText>())
            {
                element.Value = new DvText(value as string ?? "");
            }
            else if (valueConstraint.RmTypeMatches<DvDateTime>())
            {
                element.Value = new DvDateTime(Convert.ToDateTime(value));
            }
            else if (valueConstraint.RmTypeMatches<DvCount>())
            {
                element.Value = new DvCount(Convert.ToInt32(value));
            }
            else if (valueConstraint.RmTypeMatches<DvQuantity>())
            {
                element.ValueAs<DvQuantity>().Magnitude = Convert.ToDouble(value);
            }
        }

        protected static GridCellType GetDataType(string rmTypeName)
        {
            return dataTypeMap[rmTypeName];
        }
    }
}
