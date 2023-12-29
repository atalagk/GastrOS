using System;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.DesignByContract;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Quantity.DateTime;

namespace GastrOs.Sde.ViewControls
{
    public class DateElementControl : TypedViewControl<Element, IDateView>
    {
        private CComplexObject valueConstraint;
        
        public DateElementControl(CComplexObject constraint)
            : base(constraint)
        {
            valueConstraint = constraint.ExtractElemValueConstraint() as CComplexObject;
            Check.Invariant(valueConstraint != null);
        }

        ~DateElementControl()
        {
            if (View != null)
            {
                //Release event handler to avoid memory leak
                View.DateChanged -= UpdateModel;
            }
        }

        protected override void SetModelPostexecute(Element oldModel)
        {
            Check.Require(Model.Value != null && Model.Value is DvDateTime);
        }

        protected override void SetViewPostexecute(IDateView oldView)
        {
            if (oldView != null)
                oldView.DateChanged -= UpdateModel;
            View.DateChanged += UpdateModel;
            View.DataValueProvider = new DvDateTimeProvider();
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        public override void RefreshViewFromModel()
        {
            View.Title = TitleFunction();
            //temporarily disable view-to-model update, since we're updating view manually
            View.DateChanged -= UpdateModel;
            //manually set view text
            View.Value = View.DataValueProvider.ToRawValue(Model.Value);
            //then re-enable view-to-model update
            View.DateChanged += UpdateModel;
        }

        private void UpdateModel(object sender, EventArgs e)
        {
            if (Model == null || !View.Date.HasValue)
                return;
            Model.Value = View.DataValueProvider.ToDataValue(View.Date);
        }
    }
}