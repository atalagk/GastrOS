using System;
using System.Linq;
using GastrOs.Sde.Engine;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AM.OpenehrProfile.DataTypes.Quantity;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Quantity;

namespace GastrOs.Sde.ViewControls
{
    public class QuantElementControl : TypedViewControl<Element, INumericUnitView>
    {
        private CDvQuantity quantConstraint;

        public QuantElementControl(CComplexObject constraint)
            : base(constraint)
        {
            quantConstraint = constraint.ExtractElemValueConstraint() as CDvQuantity;
            Check.Invariant(quantConstraint != null);
        }

        ~QuantElementControl()
        {
            if (View != null)
            {
                //Release event handler to avoid memory leak
                View.ValueChanged -= UpdateMagnitudeFromView;
                View.UnitChanged -= UpdateUnitFromView;
            }
        }

        protected override void SetModelPostexecute(Element oldModel)
        {
            Check.Require(Model.Value != null && Model.Value is DvQuantity);
        }

        protected override void SetViewPostexecute(INumericUnitView oldView)
        {
            if (oldView != null)
            {
                oldView.ValueChanged -= UpdateMagnitudeFromView;
                oldView.UnitChanged -= UpdateUnitFromView;
            }

            View.AvailableUnits = quantConstraint.List.Select(quantItem => quantItem.Units).ToList();
            View.DataValueProvider = new DvQuantityProvider(quantConstraint);
            
            View.ValueChanged += UpdateMagnitudeFromView;
            View.UnitChanged += UpdateUnitFromView;
        }

        /// <summary>
        /// Invoked when view's numeric input has changed, thus requiring corresponding
        /// update on the model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateMagnitudeFromView(object sender, EventArgs e)
        {
            if (Model == null)
                return;
            DvQuantity oldQuantity = Model.ValueAs<DvQuantity>();
            //NOTE Special (temporary) provision: if view's value is unset, set model value to 0.
            double viewValue = Convert.ToDouble(View.Value);
            Model.Value = new DvQuantity(viewValue, oldQuantity.Units);
        }

        /// <summary>
        /// Invoked when view's "unit" input has changed, thus requiring corresponding
        /// update on the model. Also the min. and max. values for the view should be
        /// updated to reflect the constraints on the unit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateUnitFromView(object sender, EventArgs e)
        {
            if (Model == null)
                return;
            DvQuantity oldQuantity = Model.ValueAs<DvQuantity>();
            string units = View.Unit;
            Model.Value = new DvQuantity(oldQuantity.Magnitude, units);
            UpdateMinMaxValuesOnView();
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        public override void RefreshViewFromModel()
        {
            View.Title = TitleFunction();
            View.ValueChanged -= UpdateMagnitudeFromView;
            View.UnitChanged -= UpdateUnitFromView;
            DvQuantity quantity = Model.ValueAs<DvQuantity>();
            View.Value = View.DataValueProvider.ToRawValue(Model.Value);
            View.Unit = quantity.Units;
            View.ValueChanged += UpdateMagnitudeFromView;
            View.UnitChanged += UpdateUnitFromView;
            UpdateMinMaxValuesOnView();
        }

        private void UpdateMinMaxValuesOnView()
        {
            CQuantityItem quantItem = quantConstraint.List.SingleOrDefault(qc => string.Equals(qc.Units, View.Unit));
            //If quantity is unconstrained, then make sure min/max are set to default
            if (quantItem == null || quantItem.Magnitude == null)
            {
                View.MinValue = 0;
                View.MaxValue = decimal.MaxValue;
                return;
            }

            //Determine the min. and max. values according to current unit's constraint
            decimal min = quantItem.Magnitude.LowerIncluded ? (decimal)quantItem.Magnitude.Lower : 0;
            decimal max = quantItem.Magnitude.UpperIncluded ? (decimal) quantItem.Magnitude.Upper : decimal.MaxValue;
            View.MinValue = min;
            View.MaxValue = max;
        }
    }
}