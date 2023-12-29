using System;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;
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
            //Release event handler to avoid memory leak
            View.TextChanged -= UpdateText;
        }

        protected override void SetModelPostexecute(Element oldModel)
        {
            Check.Require(Model.Value != null && Model.Value is DvText);
        }

        protected override void SetViewPostexecute(ITextView oldView)
        {
            if (oldView != null)
                oldView.TextChanged -= UpdateText;

            View.TextChanged += UpdateText;
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        public override void UpdateViewFromModel()
        {
            View.Title = TitleFunction();
            DvText text = Model.ValueAs<DvText>();
            if (text != null)
                View.Text = text.Value;
        }

        private void UpdateText(object sender, EventArgs e)
        {
            if (Model == null || View.Text == null)
                return;
            Model.Value = new DvText(View.Text);
        }
    }
}