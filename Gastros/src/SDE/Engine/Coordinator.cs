using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views;
using Microsoft.Practices.Unity;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.Common.Archetyped;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.AssumedTypes;

namespace GastrOs.Sde.Engine
{
    /// <summary>
    /// The main class that co-ordinates view creation and manipulation.
    /// </summary>
    public class Coordinator
    {
        private IUnityContainer container;
        private OperationalTemplate opTemplate;
        private ViewsGenerator viewsGenerator;

        //Keeps track of all generated viewcontrols to which this is an
        //event listener for later deregistering (prevent memory leaks)
        private HashSet<ViewControl> generatedViewControls = new HashSet<ViewControl>();

        public Coordinator(OperationalTemplate opTemplate)
        {
            Check.Require(opTemplate != null, "Operational template must not be null");
            this.opTemplate = opTemplate;

            //Set up unity container (stuff that deals with instantiating the right GUI
            //widget classes) using application configuration values
            container = new UnityContainer();
            GastrOsConfig.UnityConfig.Configure(container, GastrOsConfig.EngineConfig.UnityContainerName);

            viewsGenerator = new ViewsGenerator(opTemplate, container);
        }

        ~Coordinator()
        {
            //Release event handlers
            foreach (ViewControl viewControl in generatedViewControls)
            {
                viewControl.NewInstanceRequest -= HandleNewInstance;
                viewControl.RemoveInstanceRequest -= HandleRemoveInstance;
            }
        }

        public ViewControl GenerateView(Locatable valueInstance, string archetypeId)
        {
            CArchetypeRoot archetypeRoot = opTemplate.LocateArchetypeById(archetypeId);
            Check.Require(archetypeRoot != null, "Can't find archetype with id " + archetypeId + " from operational template");

            return GenerateView(valueInstance, archetypeRoot);
        }

        /// <summary>
        /// Recursively generate views for given value instance, corresponding to given
        /// constraint. Invokes the <see cref="viewsGenerator"/> to do the dirty work.
        /// </summary>
        /// <param name="valueInstance"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public ViewControl GenerateView(Locatable valueInstance, CComplexObject constraint)
        {
            ViewControl viewControl = viewsGenerator.GenerateFor(valueInstance, constraint);

            Postprocess(viewControl);

            return viewControl;
        }

        /// <summary>
        /// Recursively register instance request events and also perform layout
        /// </summary>
        /// <param name="control"></param>
        private void Postprocess(ViewControl control)
        {
            //Add to the list of view controls to later deregister events from
            generatedViewControls.Add(control);

            //only respond to instance requests if this control has a parent
            if (control.Parent != null && control.Parent.View is IContainerView)
            {
                control.NewInstanceRequest += HandleNewInstance;
                control.RemoveInstanceRequest += HandleRemoveInstance;

                //Update the ability to add/remove instances of this view based on
                //constraint
                Interval<int> occurrences = control.Constraint.Occurrences;
                int currentNoInstances = control.Parent.Model.ChildInstances(control.Constraint).Count();
                control.View.CanAddNewInstance = occurrences.UpperUnbounded ||
                                            occurrences.UpperIncluded && occurrences.Upper > currentNoInstances;
                control.View.CanRemoveInstance = currentNoInstances > 1;
            }

            if (control.AllowsChildren)
            {
                foreach (ViewControl child in control.Children)
                {
                    Postprocess(child);
                }
            }

            //adjust Size at the end to ensure the "leaf-most" controls get
            //resized first
            control.View.Size = control.View.IdealSize;
        }

        /// <summary>
        /// Invoked whenever a view has requested a new instance to be created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleNewInstance(object sender, EventArgs e)
        {
            ViewControl control = sender as ViewControl;
            Check.Require(control != null);
            ViewControl parentControl = control.Parent;
            Check.Require(parentControl != null);
            IContainerView parentView = parentControl.View as IContainerView;
            Check.Require(parentView != null);

            CComplexObject constraint = control.Constraint;
            
            //construct new instance of model
            Locatable newModel = RmFactory.Instantiate(constraint, opTemplate);
            Pathable parent = control.Model.Parent;
            if (parent == null) return;
            parent.AddChild(newModel);
            
            //construct new instance of view control
            ViewControl newInstance = viewsGenerator.GenerateFor(newModel, constraint);

            //figure out the position to insert new control/view
            int position = parentControl.Children.IndexOf(control);
            int viewPosition = parentView.Children.IndexOf(control.View);

            //insert new instance view/control to parent view/control at the appropriate position
            control.Parent.Children.Insert(position + 1, newInstance);
            parentView.Children.Insert(viewPosition + 1, newInstance.View);

            //register instance requests on the new control
            newInstance.NewInstanceRequest += HandleNewInstance;
            newInstance.RemoveInstanceRequest += HandleRemoveInstance;

            generatedViewControls.Add(newInstance);

            //update can-add/remove state for view and all of its siblings
            //note this will include the newly created view
            UpdateInstanceModifiability(parentControl, constraint);

            ResizeViewControl(newInstance);
        }

        /// <summary>
        /// Invoked whenever a view has requested itself to be removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleRemoveInstance(object sender, EventArgs e)
        {
            ViewControl control = sender as ViewControl;
            Check.Require(control != null);
            ViewControl parentControl = control.Parent;
            Check.Require(parentControl != null);
            IContainerView parentView = parentControl.View as IContainerView;
            Check.Require(parentView != null);

            Pathable parent = control.Model.Parent;
            if (parent == null) return;
            parent.RemoveChild(control.Model);

            control.Parent.Children.Remove(control);
            parentView.Children.Remove(control.View);

            //de-register listeners to avoid memory leak
            control.NewInstanceRequest -= HandleNewInstance;
            control.RemoveInstanceRequest -= HandleRemoveInstance;

            generatedViewControls.Remove(control);

            //update can-add/remove state for removed view's siblings
            UpdateInstanceModifiability(parentControl, control.Constraint);

            ResizeViewControl(parentControl);
        }

        private static void UpdateInstanceModifiability(ViewControl parentControl, CObject constraint)
        {
            //TODO must be a more elegant way of going about this

            //figure out occurrence limit for the particular constraint & the current no. instances of
            //children with that constraint
            Interval<int> occurrences = constraint.Occurrences;
            IEnumerable<Locatable> childInstances = parentControl.Model.ChildInstances(constraint);
            int currentNoInstances = childInstances.Count();
            //then for each view representing one of the existing instances, update its
            //ability to add/remove instances of the view
            foreach (ViewControl sibling in parentControl.Children.Where(control => childInstances.Contains(control.Model)))
            {
                //TODO template designer has bug that may affect this check
                sibling.View.CanAddNewInstance = occurrences.UpperUnbounded || occurrences.Upper > currentNoInstances;
                sibling.View.CanRemoveInstance = currentNoInstances > 1 && currentNoInstances > occurrences.Lower;
            }
        }

        /// <summary>
        /// Resize view (along with its children and parents)
        /// Probably not a very elegant solution - rethink
        /// </summary>
        /// <param name="control"></param>
        private static void ResizeViewControl(ViewControl control)
        {
            RecursiveResize(control);
            //Now traverse up the parent widget hierarchy and resize
            for (ViewControl updatedControl = control.Parent; updatedControl != null; updatedControl = updatedControl.Parent)
            {
                Size idealSize = updatedControl.View.IdealSize;
                updatedControl.View.Size = idealSize;
            }
        }

        /// <summary>
        /// Recursively resizes self and children from leaf upwards
        /// </summary>
        /// <param name="control"></param>
        private static void RecursiveResize(ViewControl control)
        {
            if (control.AllowsChildren)
            {
                foreach (ViewControl child in control.Children)
                {
                    RecursiveResize(child);
                }
            }
            control.View.Size = control.View.IdealSize;
        }
    }
}
