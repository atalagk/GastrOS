using System.Collections.Generic;
using System.Linq;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AM.OpenehrProfile.DataTypes.Text;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.ViewControls
{
    /// <summary>
    /// Splasher presenter is a bit tricky - it is mainly used to handle
    /// GastrOS "Sites", which is basically a set of multiply-occurrable
    /// Elements holding mutually-exclusive values. Since we like to keep
    /// a one-to-one mapping between a model object and view, we represent
    /// this in terms of the Cluster containing the multiply-occurable
    /// Element.
    /// 
    /// TODO refactor
    /// </summary>
    public class SplasherControl : TypedViewControl<Cluster, ISplasherView<IMultiChoiceView>>
    {
        private readonly int maxInstances;
        private readonly CComplexObject elementConstraint;
        private readonly Dictionary<string, Element> selectableElements;

        public SplasherControl(CComplexObject containerConstraint, CComplexObject elementConstraint)
            : base(containerConstraint)
        {
            Check.Assert(elementConstraint.RmTypeName.Equals("ELEMENT"));
            //figure out max. occurrences of the element
            maxInstances = elementConstraint.Occurrences.UpperIncluded ? elementConstraint.Occurrences.Upper : int.MaxValue;
            this.elementConstraint = elementConstraint;
            
            //populate the list of selectable elements - containing the set of all
            //possible instances (each with different code value)
            selectableElements = new Dictionary<string, Element>();
            CComplexObject valueConstraint = elementConstraint.ExtractElemValueConstraint() as CComplexObject;
            CCodePhrase codePhrase = valueConstraint.ExtractCodePhrase();
            foreach (string code in codePhrase.CodeList)
            {
                Element element = RmFactory.InstantiateElement(elementConstraint);
                element.Name.Value += code; //THIS IS IMPORTANT to ensure uniqueness of names
                DvCodedText codedText = element.Value as DvCodedText;
                Check.Assert(codedText != null);
                codedText.Value = code;
                selectableElements[code] = element;
            }
        }

        ~SplasherControl()
        {
            //Release event handler to avoid memory leak
            View.SplashedView.ItemSelectionChanged -= UpdateModelAndSelectability;
        }

        protected override void SetViewPostexecute(ISplasherView<IMultiChoiceView> oldView)
        {
            Check.Require(View.SplashedView != null, "You need to first assign a splashed view to "+
                                                     "the view object before feeding it to the presenter");
            
            //de-register listener to old view (prevent memory leak)
            if (oldView != null)
                oldView.SplashedView.ItemSelectionChanged -= UpdateModelAndSelectability;

            View.SplashedView.SetDisplayFunction(CodeToString);
            //register listener to new view
            View.SplashedView.ItemSelectionChanged += UpdateModelAndSelectability;
            //Initialise the view by populating it with selectable Elements
            foreach (string code in selectableElements.Keys)
            {
                View.SplashedView.AddSelectableItem(code);
            }
        }

        /// <summary>
        /// Find all child elements that correspond to the element constraint
        /// (i.e. that should be selected). Note: returns a live collection
        /// </summary>
        /// <returns></returns>
        private IList<string> GetPresentCodesFromModel()
        {
            List<string> presentCodes = new List<string>();
            foreach (Item item in Model.Items)
            {
                if (!item.LightValidate(elementConstraint) || !(item is Element)) continue;
                Element elem = (Element) item;
                if (!(elem.Value is DvCodedText)) continue;
                DvCodedText value = (DvCodedText)elem.Value;
                //TODO should really eliminate the need for "atxxxx" values at some point soon
                if (value.Value != RmFactory.DummyCodedValue)
                    presentCodes.Add(value.Value);
            }
            return presentCodes;
        }

        /// <summary>
        /// Locates the element that contains the specific coded text value
        /// from the model. Kinda similar to the above method
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private Element FindElementWithCodedTextFromModel(string code)
        {
            foreach (Item item in Model.Items)
            {
                Element elem = item as Element;
                if (elem == null) continue;
                DvCodedText text = elem.Value as DvCodedText;
                if (text == null) continue;
                if (string.Equals(text.Value, code))
                    return elem;
            }
            return null;
        }

        private string CodeToString(object obj)
        {
            string code = obj as string;
            Check.Require(code != null);
            return AomHelper.ExtractOntology(code, ArchetypeRoot).Text;
        }

        /// <summary>
        /// This method is called whenever the view receives a change from user input
        /// and the model must be updated accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdateModelAndSelectability(object sender, SelectionEventArgs e)
        {
            /*
            //NOTE Bulk updating isn't reliable:
            //consider the case of loading view from model where more than one item is selected.
              first UpdateViewFromModel() is going to be called, which in turn will select
              the first item in the view, effectively feeding back to this method and trying to
              update the model to have just that one corresponding item (thereby deleting the
              rest).
            */

            string affectedCode = e.Item as string;
            if (affectedCode == null)
                return;
            
            if (e.Selected)
            {
                //means the model corresponding to the changed code should now be present
                //(which implies it wasn't before, but check nonetheless)
                if (FindElementWithCodedTextFromModel(affectedCode) == null)
                    Model.Items.Add(selectableElements[affectedCode]);
            }
            else
            {
                //means the opposite - the model corresponding to the changed code should
                //now be removed (which implies it was present before, but check nonetheless)
                Element toRemove = FindElementWithCodedTextFromModel(affectedCode);
                if (toRemove != null)
                    Model.Items.Remove(toRemove);
            }

            //NOTE at this point there is no guarantee that the number of elements in the model
            //match the number of corresponding items selected in the view, as the thread may
            //be in the process of populating a fresh view from an existing model

            IList<string> selectedCodesFromModel = GetPresentCodesFromModel();
            //update view as to whether more items can be selected
            View.SplashedView.CanSelectMore = selectedCodesFromModel.Count() < maxInstances;

            //update tooltip - display contents
            string toolTip = selectedCodesFromModel.Count > 0
                                 ? selectedCodesFromModel.Select(s => CodeToString(s)).ToPrettyString(", ")
                                 : null;
            View.FurtherInformation = toolTip;
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        public override void UpdateViewFromModel()
        {
            View.Title = TitleFunction();
            IEnumerable<string> selectedCodes = GetPresentCodesFromModel();
            foreach (string selectable in selectableElements.Keys)
            {
                View.SplashedView.SetSelected(selectable, selectedCodes.Contains(selectable));
            }
        }

        public override string GetOntologyTitleAndDesc()
        {
            OntologyItem ontology = elementConstraint.ExtractOntology();
            if (ontology == null)
                return null;
            return ontology.Text + " " + ontology.Description;
        }

        public override string GetOntologyTitle()
        {
            return elementConstraint.ExtractOntologyText();
        }
    }
}