using System;
using System.Collections.Generic;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.Futures.OperationalTemplate;

namespace GastrOs.Sde.ViewControls
{
    /// <summary>
    /// Co-ordinates the interaction between a model object (RM instance) and
    /// its associated view. Acts as a "brain" behind the model and view.
    /// </summary>
    public abstract class ViewControl
    {
        private IView view;
        private Locatable model;
        
        private readonly CComplexObject constraint;

        private ViewControl parent;
        private EventRaisingList<ViewControl> children;

        public event EventHandler NewInstanceRequest;
        public event EventHandler RemoveInstanceRequest;

        private Func<string> titleFunction;

        protected ViewControl(CComplexObject constraint)
        {
            Check.Require(constraint != null);
            Check.Require(constraint.GetArchetypeRoot() != null);

            this.constraint = constraint;
            titleFunction = GetOntologyTitle;
        }

        ~ViewControl()
        {
            if (View != null)
            {
                //Release event handler to avoid memory leak
                View.NewInstanceRequest -= handleNewInstance;
                View.RemoveInstanceRequest -= handleRemoveInstance;
            }
        }

        /// <summary>
        /// The viewcontrol that acts as a parent for this viewcontrol. As a general rule,
        /// a viewcontrol P' should be a parent of this viewcontrol P if and only if the
        /// view presented by P' is also a parent of the view presented by P. Otherwise
        /// strange things could happen.
        /// </summary>
        public ViewControl Parent
        {
            get { return parent; }
            protected set
            {
                if (parent == value)
                    return;
                if (parent != null) //old parent
                {
                    parent.Children.Remove(this);
                }
                parent = value;
                if (parent != null && !parent.Children.Contains(this)) //new parent
                {
                    parent.Children.Add(this);
                }
            }
        }

        /// <summary>
        /// Whether or not this viewcontrol allows children.
        /// </summary>
        public abstract bool AllowsChildren
        {
            get;
        }

        /// <summary>
        /// If this viewcontrol allows children, then returns a live list of presenters
        /// that act as children to this viewcontrol. Otherwise returns null.
        /// As a general rule, a viewcontrol P' should be a parent of this viewcontrol P if
        /// and only if the view presented by P' is also a parent of the view presented
        /// by P. Otherwise strange things could happen.
        /// </summary>
        public IList<ViewControl> Children
        {
            get
            {
                if (children == null && AllowsChildren)
                {
                    children = new EventRaisingList<ViewControl>();
                    children.ItemAdded += UpdateParent;
                }   
                return children;
            }
        }

        private void UpdateParent(object sender, ListEventArgs<ViewControl> e)
        {
            e.Item.parent = this;
        }

        /// <summary>
        /// The archetype root housing the constraint model
        /// </summary>
        public CArchetypeRoot ArchetypeRoot
        {
            get
            {
                return constraint.GetArchetypeRoot();
            }
        }

        /// <summary>
        /// The archetype constraint for the model object
        /// </summary>
        public CComplexObject Constraint
        {
            get
            {
                return constraint;
            }
        }

        /// <summary>
        /// The model object for this viewcontrol
        /// </summary>
        public virtual Locatable Model
        {
            get
            {
                return model;
            }
            set
            {
                Locatable oldModel = model;
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.LightValidate(constraint))
                    throw new ArgumentException("Model object must be valid against constraint");
                model = value;
                SetModelPostexecute(oldModel);
                if (view != null)
                {
                    RefreshViewFromModel();
                }
            }
        }

        /// <summary>
        /// The view used by this viewcontrol to display the model
        /// </summary>
        public virtual IView View
        {
            get
            {
                return view;
            }
            set
            {
                IView oldView = view;
                if (value == null)
                    throw new ArgumentNullException("value");
                if (view != null)
                {
                    //de-register event handlers before releasing old view
                    view.NewInstanceRequest -= handleNewInstance;
                    view.RemoveInstanceRequest -= handleRemoveInstance;
                }
                view = value;
                view.NewInstanceRequest += handleNewInstance;
                view.RemoveInstanceRequest += handleRemoveInstance;
                SetViewPostexecute(oldView);
                
                if (model != null)
                {
                    RefreshViewFromModel();
                }
            }
        }

        private void handleNewInstance(object sender, EventArgs e)
        {
            if (NewInstanceRequest != null)
            {
                NewInstanceRequest(this, e);
            }
        }

        private void handleRemoveInstance(object sender, EventArgs e)
        {
            if (RemoveInstanceRequest != null)
            {
                RemoveInstanceRequest(this, e);
            }
        }

        /// <summary>
        /// Function used to determine the title displayed in the view.
        /// By default it is set to <see cref="GetOntologyTitle"/>, which
        /// gets the text portion of the constraint's ontology.
        /// </summary>
        public Func<string> TitleFunction
        {
            get { return titleFunction; }
            set { titleFunction = value; }
        }

        public virtual string GetOntologyTitleAndDesc()
        {
            OntologyItem ontology = Constraint.ExtractOntology();
            if (ontology == null)
                return null;
            return ontology.Text + " " + ontology.Description;
        }

        public virtual string GetOntologyTitle()
        {
            return Constraint.ExtractOntologyText();
        }

        /*
        private void handleViewPropertyChange(object o, PropertyChangedEventArgs e)
        {
            UpdateModelAndSelectability(e.PropertyName);
        }
        */

        /// <summary>
        /// Called in order to force the view to update its values to match those
        /// of the model.
        /// </summary>
        public virtual void RefreshViewFromModel() { }

        /// <summary>
        /// Invoked when the view has changed, which would require a corresponding update
        /// to the model.
        /// </summary>
        /// <param name="propertyName">Name of the view property that has changed</param>
        //public virtual void UpdateModelAndSelectability(string propertyName) { }
        
        /// <summary>
        /// subclasses can optionally implement this method to execute just after the
        /// model is set (and before the view is updated)
        /// </summary>
        protected virtual void SetModelPostexecute(Locatable oldModel) { }

        /// <summary>
        /// subclasses can optionally implement this method to execute just after the
        /// view is set (and before it is updated)
        /// </summary>
        protected virtual void SetViewPostexecute(IView oldView) { }
    }
}