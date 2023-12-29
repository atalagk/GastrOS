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
    /// Used to handle multiply-occurring elements whose values are mutually exclusive,
    /// c.f. GastrOS "Sites". Since we like to keep
    /// a one-to-one mapping between a model object and view, we represent
    /// this in terms of the Cluster containing the multiply-occurable
    /// Element.
    /// </summary>
    public class MultiChoiceControl : TypedViewControl<Cluster, IMultiChoiceView>
    {
        private readonly int maxInstances;
        private readonly CComplexObject elementConstraint;
        private readonly Dictionary<string, Element> selectableElements;

        public MultiChoiceControl(CComplexObject containerConstraint, CComplexObject elementConstraint)
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

        ~MultiChoiceControl()
        {
            //Release event handler to avoid memory leak
            View.ItemSelectionChanged -= UpdateModelAndSelectability;
        }

        protected override void SetViewPostexecute(IMultiChoiceView oldView)
        {
            //de-register listener to old view (prevent memory leak)
            if (oldView != null)
                oldView.ItemSelectionChanged -= UpdateModelAndSelectability;

            View.SetDisplayFunction(CodeToString);
            //register listener to new view
            View.ItemSelectionChanged += UpdateModelAndSelectability;
            //Initialise the view by populating it with selectable Elements
            foreach (string code in selectableElements.Keys)
            {
                View.AddSelectableItem(code);
            }
        }

        /// <summary>
        /// Find all child elements that correspond to the element constraint
        /// (i.e. that should be selected). Note: returns a live collection
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetPresentCodesFromModel()
        {
            IEnumerable<Item> instances = Model.Items.Where(item => AomHelper.LightValidate(item, elementConstraint));
            //TODO Implicit invariant that all instances are of type Element with DvCodedText values - may be worth making explicit
            return instances.Cast<Element>().Select(elem => elem.ValueAs<DvCodedText>().Value);
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
              first RefreshViewFromModel() is going to be called, which in turn will select
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

            IEnumerable<string> selectedCodesFromModel = GetPresentCodesFromModel();
            //update view as to whether more items can be selected
            View.CanSelectMore = selectedCodesFromModel.Count() < maxInstances;
        }

        public override bool AllowsChildren
        {
            get { return false; }
        }

        public override void RefreshViewFromModel()
        {
            View.Title = TitleFunction();
            IEnumerable<string> selectedCodes = GetPresentCodesFromModel();
            foreach (string selectable in selectableElements.Keys)
            {
                View.SetSelected(selectable, selectedCodes.Contains(selectable));
            }
        }
    }
}