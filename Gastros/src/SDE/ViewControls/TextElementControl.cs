using System;
using GastrOs.Sde.Engine;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.ViewControls
{
    /// <summary>
    /// Presents an element containing DvText (and DvCodedText) through a text view
    /// </summary>
    public class TextElementControl : TypedViewControl<Element, ITextView>
    {
        public TextElementControl(CComplexObject constraint)
            : base(constraint)
        {
        }

        ~TextElementControl()
        {
            if (View != null)
            {
                //Release event handler to avoid memory leak
                View.TextChanged -= UpdateText;
            }
        }

        protected override void SetModelPostexecute(Element oldModel)
        {
            Check.Require(Model.Value != null && Model.Value is DvText);
        }

        protected override void SetViewPostexecute(ITextView oldView)
        {
            if (oldView != null)
                oldView.TextChanged -= UpdateText;

            View.DataValueProvider = new DvTextProvider();
            View.TextChanged += UpdateText;
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        public override void RefreshViewFromModel()
        {
            View.Title = TitleFunction();
            View.TextChanged -= UpdateText;
            View.Value = View.DataValueProvider.ToRawValue(Model.Value);
            View.TextChanged += UpdateText;
        }

        private void UpdateText(object sender, EventArgs e)
        {
            if (Model == null || View.Text == null)
                return;
            Model.Value = View.DataValueProvider.ToDataValue(View.Value);
        }
    }
}