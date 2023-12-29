using System;
using System.Collections.Generic;
using System.Linq;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views;
using Microsoft.Practices.Unity;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Text;
using OpenEhr.Futures.OperationalTemplate;

namespace GastrOs.Sde.Engine
{
    /// <summary>
    /// Generates view components for displaying value instances
    /// 
    /// TODO consider refactoring - segregate logic across different RM-types. Consider using Attributes.
    /// Also the lifetime of this object - should it be reused across multiple generations?
    /// </summary>
    public class ViewsGenerator
    {
        private OperationalTemplate opTemplate;
        private DirectiveStore directives;
        private IUnityContainer container;

        public ViewsGenerator(OperationalTemplate opTemplate, IUnityContainer container)
        {
            Check.Require(opTemplate != null);
            Check.Require(container != null);

            this.opTemplate = opTemplate;
            this.directives = opTemplate.ExtractDirectives();
            this.container = container;
        }

        /// <summary>
        /// Generates a presenter that handles displaying the model (given RM object)
        /// through a generated view.
        /// </summary>
        /// <param name="valueInstance">can be null</param>
        /// <param name="constraint">must not be null</param>
        /// <returns></returns>
        public ViewControl GenerateFor(Locatable valueInstance, CComplexObject constraint)
        {
            ViewControl control = null;

            if (constraint.RmTypeMatches<Element>())
            {
                control = GenerateForElement(valueInstance as Element, constraint);
            }
            else if (constraint.RmTypeMatches<Cluster>())
            {
                control = GenerateForCluster(valueInstance as Cluster, constraint);
            }

            return control;
        }

        protected ViewControl GenerateForElement(Element valueInstance, CComplexObject constraint)
        {
            //first, do a rm-type consistency check
            Check.Require(constraint != null);
            Check.Require(constraint.RmTypeMatches<Element>());

            //indicates whether a stub value instance had to be created in order to display in GUI
            /*bool isStub = valueInstance == null;
            if (isStub)
            {
                valueInstance = RmFactory.InstantiateElement(constraint);
            }*/

            //basic non-null check
            Check.Require(valueInstance != null);
            Check.Require(valueInstance.Value != null);
            //check value constraint exists
            CObject elemValueConstraint = constraint.ExtractElemValueConstraint();
            Check.Require(elemValueConstraint != null, "Element (path: "+constraint.Path+
                ") must specify a constraint for its value in order for GastrOS to be able to "+
                "generate an appropriate GUI control");
            
            ViewControl widget;

            if (elemValueConstraint.RmTypeMatches<DvCodedText>()) //this check should precede DvText
            {
                widget = new CodedTextElementControl(constraint);
                
                //depending on the presence of alternateStyle directive, determine whether to
                //render it as combo or radio
                AlternateStyleDirective alternateStyle =
                    directives.GetDirectiveOfType<AlternateStyleDirective>(constraint);
                IListView view = container.Resolve<IListView>(alternateStyle == null ? "default" : "radio");
                if (alternateStyle != null)
                    view.ShowTitle = alternateStyle.ShowLabel;
                widget.View = view;
                
                ShowValueContextMode valueContext;
                //adjust showValueContext - determines how items are displayed
                ShowValueContextDirective showValueContext =
                    directives.GetDirectiveOfType<ShowValueContextDirective>(constraint);
                if (showValueContext == null)
                    valueContext = ShowValueContextMode.Normal;
                else if (showValueContext.Mode == ShowValueContextMode.Append)
                    valueContext = ShowValueContextMode.Append;
                else
                    valueContext = ShowValueContextMode.Organise;
                ((IListView) widget.View).ShowValueContext = valueContext;
            }
            else if (elemValueConstraint.RmTypeMatches<DvText>())
            {
                widget = new TextElementControl(constraint);

                //generate either a single-line or multi-line text view, depending on directive
                ShowTextDirective showText = directives.GetDirectiveOfType<ShowTextDirective>(constraint);
                if (showText != null && showText.Mode == ShowTextMode.Multi)
                    widget.View = container.Resolve<ITextView>("multi");
                else
                    widget.View = container.Resolve<ITextView>("default");
            }
            else if (elemValueConstraint.RmTypeMatches<DvCount>())
            {
                widget = new CountElementControl(constraint);
                widget.View = container.Resolve<INumericView>();
            }
            else if (elemValueConstraint.RmTypeMatches<DvQuantity>())
            {
                widget = new QuantElementControl(constraint);
                widget.View = container.Resolve<INumericUnitView>();
            }
            else
            {
                throw new InvalidOperationException("Sorry. GastrOS doesn't yet know how to generate a GUI control "+
                    "for an Element with type "+ elemValueConstraint.RmTypeName);
            }

            //process the showDesc directive and determine the exact function to use when
            //displaying title
            if (directives.HasDirectiveOfType<ShowDescriptionDirective>(constraint))
            {
                widget.TitleFunction = widget.GetOntologyTitleAndDesc;
            }
            widget.Model = valueInstance;

            RegisterDirectives(widget.View, constraint);
            return widget;
        }

        protected ViewControl GenerateForSplashedElement(Cluster parentValueInstance, CComplexObject elementConstraint)
        {
            CComplexObject parentConstraint = elementConstraint.GetParent() as CComplexObject;
            Check.Require(parentConstraint != null);
            //construct presenter
            SplasherControl splasher = new SplasherControl(parentConstraint, elementConstraint);

            //set model
            splasher.Model = parentValueInstance;
            //set splasher view (but first assign sub-view to view)
            ISplasherView<IMultiChoiceView> view = container.Resolve<ISplasherView<IMultiChoiceView>>();
            view.SplashedView = container.Resolve<IMultiChoiceView>();
            splasher.View = view;

            RegisterDirectives(view, elementConstraint);
            return splasher;
        }

        protected ViewControl GenerateForMultiChoiceElement(Cluster parentValueInstance, CComplexObject elementConstraint)
        {
            AlternateStyleDirective alternateStyle =
                    directives.GetDirectiveOfType<AlternateStyleDirective>(elementConstraint);

            CComplexObject parentConstraint = elementConstraint.GetParent() as CComplexObject;
            Check.Require(parentConstraint != null);
            //construct presenter
            MultiChoiceControl control = new MultiChoiceControl(parentConstraint, elementConstraint);

            IMultiChoiceView view = container.Resolve<IMultiChoiceView>();
            view.ShowTitle = alternateStyle.ShowLabel;
            view.Framed = alternateStyle.ShowBorder;
            
            //set model and view
            control.Model = parentValueInstance;
            control.View = view;

            RegisterDirectives(view, elementConstraint);
            return control;
        }

        protected ViewControl GenerateForCluster(Cluster valueInstance, CComplexObject constraint)
        {
            return GenerateForCluster(valueInstance, constraint, null);
        }

        /// <summary>
        /// Generates view control(s) for given cluster instance with given constraint. 
        /// </summary>
        /// <param name="valueInstance"></param>
        /// <param name="constraint"></param>
        /// <param name="childrenSubset">if non-null & non-empty, limits children to constraints given in this
        /// list</param>
        /// <returns></returns>
        private ViewControl GenerateForCluster(Cluster valueInstance, CComplexObject constraint,
            IList<CObject> childrenSubset)
        {
            //basic non-null check
            Check.Require(valueInstance != null);
            Check.Require(constraint != null);
            //rm-type consistency check
            Check.Require(CObject.IsSameRmType(constraint.RmTypeName, valueInstance));

            ICollection<CObject> childConstraints = constraint.ExtractChildConstraints();
            if (childrenSubset != null && childrenSubset.Count > 0)
            {
                //verify that the given subset is indeed a subset
                Check.Require(childrenSubset.All(childConstraints.Contains));
            }
            else
            {
                childrenSubset = new List<CObject>(childConstraints);
            }

            //NOTE Doesn't support splitting of core concepts yet
            if (directives.HasDirectiveOfType<CoreConceptDirective>(constraint))
            {
                return GenerateForCoreConcept(valueInstance, constraint);
            }

            bool showDesc = directives.HasDirectiveOfType<ShowDescriptionDirective>(constraint);
            bool isOrganiser = directives.HasDirectiveOfType<OrganiserDirective>(constraint);

            //if isOrganiser directive is set, make sure frame is drawn
            SimpleContainerControl control = new SimpleContainerControl(constraint, isOrganiser);

            //if showDesc directive set use a different function to use when displaying title
            if (showDesc)
                control.TitleFunction = control.GetOntologyTitleAndDesc;

            //Generate a tabbed view if it's for the archetype root, or if children have any break directives
            //Otherwise, generate a simple view
            bool generateTabbed = constraint is CArchetypeRoot ||
                                  childrenSubset.Any(c => directives.HasDirectiveOfType<BreakDirective>(c));

            control.View = container.Resolve<IContainerView>(generateTabbed ? "tabbed" : "default");
            control.Model = valueInstance;
            RegisterDirectives(control.View, constraint);

            //go through child items and recursively generate and add sub-views/controls
            foreach (CComplexObject childConstraint in childrenSubset)
            {
                //add children to whichever is the most recent "split" portion of the view control
                GenerateAndAddChildToParent(control, childConstraint);
            }

            return control;
        }

        /// <summary>
        /// "Splits" the children constraints of specified constraints into sub-sequences,
        /// where each "splitter" marks the beginning of the next subsequence.
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="splitters"></param>
        /// <returns></returns>
        private IEnumerable<IList<CObject>> SplitIntoSubsequences(CComplexObject constraint, IEnumerable<CObject> splitters)
        {
            List<IList<CObject>> subsets = new List<IList<CObject>>();
            List<CObject> currentSubSequence = new List<CObject>();
            subsets.Add(currentSubSequence);

            ICollection<CObject> childConstraints = constraint.ExtractChildConstraints();
            foreach (CObject child in childConstraints)
            {
                if (splitters.Contains(child))
                {
                    currentSubSequence = new List<CObject>();
                    subsets.Add(currentSubSequence);
                }
                currentSubSequence.Add(child);
            }
            return subsets;
        }

        protected ViewControl GenerateForCoreConcept(Cluster coreConcept, CComplexObject constraint)
        {
            CoreConceptControl viewControl = new CoreConceptControl(constraint);

            //process the showDesc directive and determine the exact function to use when
            //displaying title
            if (directives.HasDirectiveOfType<ShowDescriptionDirective>(constraint))
            {
                viewControl.TitleFunction = viewControl.GetOntologyTitleAndDesc;
            }

            viewControl.View = container.Resolve<ICoreConceptView>();
            viewControl.Model = coreConcept;

            ICollection<CObject> childConstraints = constraint.ExtractChildConstraints();
            //go through child items and recursively generate and add sub-views/controls
            foreach (CComplexObject childConstraint in childConstraints)
            {
                //skip presence element, since it's already "added"
                if (viewControl.PresenceElement.LightValidate(childConstraint))
                    continue;
                GenerateAndAddChildToParent(viewControl, childConstraint);
            }

            RegisterDirectives(viewControl.View, constraint);
            return viewControl;
        }

        /// <summary>
        /// A reusable method block for generating and adding children to a container
        /// </summary>
        /// <typeparam name="TM">type of model (value instance)</typeparam>
        /// <typeparam name="TV">type of view</typeparam>
        /// <param name="parentControl"></param>
        /// <param name="childConstraint"></param>
        protected void GenerateAndAddChildToParent<TM, TV>(TypedViewControl<TM, TV> parentControl, CComplexObject childConstraint)
            where TM : Cluster
            where TV : class, IContainerView
        {
            if (directives.HasDirectiveOfType<HideOnGuiDirective>(parentControl.Constraint))
                return; //do what it says - omit! :)

            TM valueInstance = parentControl.Model;
            List<ViewControl> generatedControls = new List<ViewControl>();

            //Special case #1: if child constraint has an alternateStyle directive
            //and is a multiply-occurring coded_text, then display as multi-choice checkboxes
            if (directives.HasDirectiveOfType<AlternateStyleDirective>(childConstraint)
                && childConstraint.RmTypeMatches<Element>()
                && childConstraint.ExtractElemValueConstraint().RmTypeMatches<DvCodedText>()
                && childConstraint.MultiplicityAllowed())
            {
                ViewControl childControl = GenerateForMultiChoiceElement(valueInstance, childConstraint);
                generatedControls.Add(childControl);
            }
            //Special case #2: if show as splash, then add a single special
            //splashed widget that represents all of the relevant children
            //TODO separate out the predicates for rendering splash vs. multi-choice check boxes
            else if (directives.HasDirectiveOfType<ShowAsDirective>(childConstraint))
            {
                ViewControl childControl = GenerateForSplashedElement(valueInstance, childConstraint);
                generatedControls.Add(childControl);
            }
            else
            {
                //amass all value instances based on the same constraint
                List<Item> childrenWithSameConstraint =
                    valueInstance.Items.Where(
                        itemInstance => itemInstance.LightValidate(childConstraint)).ToList();

                //Special case: if child has further children with n > 0 break(parent) directives,
                //then generate n+1 "split" view controls for the child
                Func<CObject, bool> isParentSplitter =
                    c => directives.GetDirectivesFor(c).Contains(new BreakDirective(BreakStyle.Parent));
                //amass grandchildren constraints that are supposed to "split parents"
                IEnumerable<CObject> parentSplitters = childConstraint.ExtractChildConstraints().Where(isParentSplitter);
                
                //work out min. instances and ensure there are at least that many
                //NOTE even if min instances is 0, create at least 1, so it's editable
                int minInstances = GetMinInstances(childConstraint);

                //now generate the necessary number of children to "fill" the minimum
                for (int i = childrenWithSameConstraint.Count; i < minInstances; i++)
                {
                    Locatable newChildInstance = RmFactory.Instantiate(childConstraint, opTemplate);
                    Check.Assert(newChildInstance is Item);
                    valueInstance.AddChild(newChildInstance);
                    childrenWithSameConstraint.Add(newChildInstance as Item);
                }
                
                //for each, generate a control/view and add it to the cluster control
                foreach (Item child in childrenWithSameConstraint)
                {
                    //if this child has to be split (due to break(parent) directive) generate
                    //multiple view controls (tied to the same model)
                    //TODO only supports cluster as splittable parent
                    if (parentSplitters.Count() > 0 && child is Cluster)
                    {
                        int i = 0;
                        foreach (IList<CObject> splitSubset in SplitIntoSubsequences(childConstraint, parentSplitters))
                        {
                            //Get rid of the break(parent) directive for first child constraint - it's already been "used up"
                            CObject first = splitSubset.FirstOrDefault();
                            if (first != null)
                                directives.RemoveDirectiveFor(first, new BreakDirective(BreakStyle.Parent));
                            ViewControl splitControl = GenerateForCluster(child as Cluster, childConstraint, splitSubset);
                            //Also inject a break(next) onto split container control
                            if (i++ > 0)
                            {
                                splitControl.View.Directives.RemoveDirectivesOfType<BreakDirective>();
                                splitControl.View.Directives.Add(new BreakDirective(BreakStyle.Next));
                            }
                            generatedControls.Add(splitControl);
                        }
                    }
                    else
                    {
                        //recursively generate child control
                        ViewControl childControl = GenerateFor(child, childConstraint);
                        Check.Assert(childControl != null, "Unable to generate GUI control for constraint " + childConstraint.Path);
                        generatedControls.Add(childControl);
                    }
                }
            }

            foreach (ViewControl childToAdd in generatedControls)
            {
                parentControl.Children.Add(childToAdd);
                parentControl.View.Children.Add(childToAdd.View);
            }
        }

        private int GetMinInstances(CComplexObject childConstraint)
        {
            int minInstances = 1;
            if (childConstraint.Occurrences.LowerIncluded && childConstraint.Occurrences.Lower > 0)
                minInstances = childConstraint.Occurrences.Lower;
                
            //process the showInstances directive, in order to generate as many "dummy" children as
            //necessary to fill the number specified in the directive
            ShowInstancesDirective showInstances = directives.GetDirectiveOfType<ShowInstancesDirective>(childConstraint);
            if (showInstances != null)
            {
                int maxAllowed = childConstraint.Occurrences.UpperIncluded
                                     ? childConstraint.Occurrences.Upper
                                     : int.MaxValue;

                //make sure we don't generate more than is actually allowed by constraint!
                int instances = Math.Min(showInstances.Instances, maxAllowed);
                minInstances = Math.Max(minInstances, instances);
            }
            return minInstances;
        }

        private void RegisterDirectives(IView view, CComplexObject constraint)
        {
            foreach (IDirective directive in directives.GetDirectivesFor(constraint))
            {
                view.Directives.Add(directive);
            }
        }
    }
}
