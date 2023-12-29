using System;
using System.Collections.Generic;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.ViewControls
{
    public class CodedTextElementControl : TypedViewControl<Element, IListView>
    {
        private CComplexObject valueConstraint;
        private string termId;

        public CodedTextElementControl(CComplexObject constraint)
            : base(constraint)
        {
            valueConstraint = constraint.ExtractElemValueConstraint() as CComplexObject;
            Check.Invariant(valueConstraint != null);
            termId = valueConstraint.ExtractCodePhrase().TerminologyId.Value;
        }

        ~CodedTextElementControl()
        {
            //Release event handler to avoid memory leak
            View.TextChanged -= UpdateModel;
        }

        protected override void SetModelPostexecute(Element oldModel)
        {
            Check.Require(Model.Value != null && Model.Value is DvCodedText);
        }

        protected override void SetViewPostexecute(IListView oldView)
        {
            if (oldView != null)
                oldView.TextChanged -= UpdateModel;

            //Load possible codes into view
            List<OntologyItem> codeList = new List<OntologyItem>();
            codeList.Add(new OntologyItem(RmFactory.DummyCodedValue)); //TODO wise??
            foreach (string codePhrase in valueConstraint.ExtractCodePhrase().CodeList)
            {
                codeList.Add(AomHelper.ExtractOntology(codePhrase, ArchetypeRoot));
            }
            //Note this will implicitly invoke the TextChanged event, so important not to listen to it yet
            View.ChoiceList = codeList;
            View.TextChanged += UpdateModel;
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        public override void UpdateViewFromModel()
        {
            View.Title = View.Title = TitleFunction();
            DvCodedText text = Model.ValueAs<DvCodedText>();
            if (text != null)
            {
                //temporarily disable view-to-model update, since we're updating view manually
                View.TextChanged -= UpdateModel;
                //manually set view text
                View.Text = text.Value;
                //then re-enable view-to-model update
                View.TextChanged += UpdateModel;
            }
        }

        private void UpdateModel(object sender, EventArgs e)
        {
            if (Model == null || View.Text == null)
                return;
            Model.Value = new DvCodedText(View.Text, "", termId);
        }
    }
}