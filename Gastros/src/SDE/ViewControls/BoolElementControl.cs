using System;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.DesignByContract;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;

namespace GastrOs.Sde.ViewControls
{
    public class BoolElementControl : TypedViewControl<Element, ICheckView>
    {
        private CComplexObject valueConstraint;

        public BoolElementControl(CComplexObject constraint) : base(constraint)
        {
            valueConstraint = constraint.ExtractElemValueConstraint() as CComplexObject;
            Check.Require(valueConstraint != null);
        }

        protected override void SetModelPostexecute(Element oldModel)
        {
            Check.Require(Model != null && Model.Value is DvBoolean);
        }

        protected override void SetViewPostexecute(ICheckView oldView)
        {
            if (oldView != null)
            {
                oldView.CheckedChanged -= handleCheckedChanged;
            }

            View.CheckedChanged += handleCheckedChanged;
            View.DataValueProvider = new DvBooleanProvider();
        }

        private void handleCheckedChanged(object sender, EventArgs args)
        {
            Model.Value = View.DataValueProvider.ToDataValue(View.Value);
        }

        public override void RefreshViewFromModel()
        {
            View.Title = Constraint.ExtractOntologyText();
            View.CheckedChanged -= handleCheckedChanged;
            View.Value = View.DataValueProvider.ToRawValue(Model.Value);
            View.CheckedChanged += handleCheckedChanged;
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }
    }
}
