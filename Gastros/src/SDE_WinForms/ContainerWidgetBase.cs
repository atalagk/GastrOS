using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Base class for implementations of <see cref="IContainerView"/>
    /// </summary>
    public abstract class ContainerWidgetBase : WidgetBase, IContainerView
    {
        public event EventHandler<ChildEventArgs> ChildAdded;
        public event EventHandler<ChildEventArgs> ChildRemoved;
        
        private EventRaisingList<IView> children;
        
        public IList<IView> Children
        {
            get
            {
                if (children == null)
                {
                    children = new EventRaisingList<IView>();
                    children.ItemAdded += AddControl;
                    children.ItemRemoved += RemoveControl;
                }
                return children;
            }
        }

        /// <summary>
        /// Clean up resources and avoid memory leak
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (children != null)
            {
                children.ItemAdded -= AddControl;
                children.ItemRemoved -= RemoveControl;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// By default, framed is set to none - subclasses can override this
        /// </summary>
        public virtual bool Framed
        {
            get { return false; }
            set { }
        }

        private void AddControl(object sender, ListEventArgs<IView> e)
        {
            IView child = e.Item;
            //Don't know if this is the most elegant way to deal with adding views
            //as winforms controls. Possible to somehow "dynamically dispatch" the
            //part that deals explicitly with controls??
            Check.Require(child is Control, "Only Winddows Forms-based implementations of views can be added");

            AddChildView(child, e.Index);
            //fire event as per interface contract
            OnChildAdded(new ChildEventArgs(child));
        }

        private void RemoveControl(object sender, ListEventArgs<IView> e)
        {
            IView child = e.Item;
            //Don't know if this is the most elegant way to deal with adding views
            //as winforms controls. Possible to somehow "dynamically dispatch" the
            //part that deals explicitly with controls??
            Check.Require(child is Control, "Only Winddows Forms-based implementations of views can be removed");

            RemoveChildView(child);
            //fire event as per interface contract
            OnChildRemoved(new ChildEventArgs(child));
        }

        /// <summary>
        /// This method is invoked when this view is about to add the given child
        /// view to itself. After this, the <see cref="ChildAdded"/> event will
        /// automatically be raised.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        protected abstract void AddChildView(IView child, int index);

        /// <summary>
        /// This method is invoked when this view is about to remove the given child
        /// view from itself. After this, the <see cref="ChildRemoved"/> event will
        /// automatically be raised.
        /// </summary>
        /// <param name="child"></param>
        protected abstract void RemoveChildView(IView child);

        /// <summary>
        /// Raises the <see cref="ChildAdded"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChildAdded(ChildEventArgs e)
        {
            if (ChildAdded != null)
            {
                ChildAdded(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ChildRemoved"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChildRemoved(ChildEventArgs e)
        {
            if (ChildRemoved != null)
            {
                ChildRemoved(this, e);
            }
        }

        public override void Reset()
        {
            foreach (IView child in children)
            {
                child.Reset();
            }
        }
    }
}
