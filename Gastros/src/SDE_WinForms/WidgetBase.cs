using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Directives;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Base class for all windows forms based view implementations
    /// </summary>
    public abstract class WidgetBase : UserControl, IView
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler TitleChanged;

        public event EventHandler NewInstanceRequest;
        public event EventHandler RemoveInstanceRequest;

        public abstract string Title { get; set; }

        public abstract Size IdealSize { get; }

        private List<IDirective> directives;
        private bool canAddNewInstance;
        private bool canRemoveInstance;

        /// <summary>
        /// The list of directives associated with this view
        /// </summary>
        public IList<IDirective> Directives
        {
            get
            {
                if (directives == null)
                {
                    directives = new List<IDirective>();
                }
                return directives;
            }
        }

        /// <summary>
        /// Returns true if whether this control should be autoscrolled by
        /// its container, or false it can happily autoscroll by itself.
        /// </summary>
        public virtual bool RequiresExternalScrolling
        {
            get { return true; }
        }

        public virtual bool ShowTitle { get; set; }

        /// <summary>
        /// Raises the <see cref="TitleChanged"/> event. Also raises
        /// <see cref="PropertyChanged"/> event on the property "Title".
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTitleChanged(EventArgs e)
        {
            if (TitleChanged != null)
            {
                TitleChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Title"));
        }

        /// <summary>
        /// Raises the <see cref="Control.VisibleChanged"/> event. Also raises
        /// <see cref="PropertyChanged"/> event on the property "Visible".
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            //Notify listeners of change
            OnPropertyChanged(new PropertyChangedEventArgs("Visible"));
        }

        /// <summary>
        /// Raises the event <see cref="PropertyChanged"/>. Implementing class should
        /// call this method after any of its properties has changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the event <see cref="NewInstanceRequest"/>.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewInstanceRequest(EventArgs e)
        {
            if (NewInstanceRequest != null)
            {
                NewInstanceRequest(this, e);
            }
        }

        /// <summary>
        /// Raises the event <see cref="RemoveInstanceRequest"/>.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRemoveInstanceRequest(EventArgs e)
        {
            if (RemoveInstanceRequest != null)
            {
                RemoveInstanceRequest(this, e);
            }
        }

        /// <summary>
        /// This method is called whenever the can-add/remove-instance status
        /// of this widget changes. Subclasses can specify any further behaviour
        /// that depends on this status (for example enabling/disabling menu
        /// items for adding/removing instances)
        /// </summary>
        protected virtual void OnCanAddRemoveInstanceChange()
        {
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            return IdealSize;
        }

        /// <summary>
        /// Gets or sets whether another instance of this view can be created
        /// by the user
        /// </summary>
        public bool CanAddNewInstance
        {
            get { return canAddNewInstance; }
            set
            {
                if (canAddNewInstance == value)
                    return;
                canAddNewInstance = value;
                OnCanAddRemoveInstanceChange();
            }
        }

        /// <summary>
        /// Gets or sets whether this instance of the view can be removed by
        /// the user
        /// </summary>
        public bool CanRemoveInstance
        {
            get { return canRemoveInstance; }
            set
            {
                if (canRemoveInstance == value)
                    return;
                canRemoveInstance = value;
                OnCanAddRemoveInstanceChange();
            }
        }

        /// <summary>
        /// Resets this view to an empty state. May raise appropriate property
        /// change events as a result.
        /// </summary>
        public abstract void Reset();
    }
}