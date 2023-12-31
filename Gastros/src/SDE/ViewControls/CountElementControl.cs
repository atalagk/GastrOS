using System;
using GastrOs.Sde.Engine;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AM.Archetype.ConstraintModel.Primitive;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.AssumedTypes;

namespace GastrOs.Sde.ViewControls
{
    public class CountElementControl : TypedViewControl<Element, INumericView>
    {
        private CInteger magnitudeConstraint;

        public CountElementControl(CComplexObject constraint)
            : base(constraint)
        {
            //must contain a constraint for the element's value
            CComplexObject valueConstraint = constraint.ExtractElemValueConstraint() as CComplexObject;
            Check.Require(valueConstraint != null);
            //check if value has further magnitude constraints
            CAttribute magnitudeAttribute = valueConstraint.ExtractSingleAttribute();
            if(magnitudeAttribute != null && magnitudeAttribute.Children.Count > 0)
            {
                CPrimitiveObject primtiveConstraint = magnitudeAttribute.Children[0] as CPrimitiveObject;
                Check.Require(primtiveConstraint != null);
                magnitudeConstraint = primtiveConstraint.Item as CInteger;
            }
        }

        ~CountElementControl()
        {
            //Release event handler to avoid memory leak
            View.ValueChanged -= UpdateCount;
        }

        protected override void SetModelPostexecute(Element oldModel)
        {
            Check.Require(Model.Value != null && Model.Value is DvCount);
        }

        protected override void SetViewPostexecute(INumericView oldView)
        {
            if (oldView != null)
                oldView.ValueChanged -= UpdateCount;

            View.ValueChanged += UpdateCount;
            View.DataValueProvider = new DvCountProvider();
            UpdateMinMaxValues();
        }

        private void UpdateMinMaxValues()
        {
            //Only update min/max if magnitude constraint exists
            if (magnitudeConstraint == null)
                return;

            //Determine the min. and max. values according to constraint
            Interval<int> range = magnitudeConstraint.Range;
            decimal min = range.LowerIncluded ? range.Lower : 0;
            decimal max = range.UpperIncluded ? range.Upper : decimal.MaxValue;
            View.MinValue = min;
            View.MaxValue = max;
        }

        private void UpdateCount(object sender, EventArgs e)
        {
            if (Model == null)
                return;
            Model.Value = View.DataValueProvider.ToDataValue(View.Value);
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        public override void RefreshViewFromModel()
        {
            View.Title = TitleFunction();
            View.ValueChanged -= UpdateCount;
            View.Value = View.DataValueProvider.ToRawValue(Model.Value);
            View.ValueChanged += UpdateCount;
        }
    }
}