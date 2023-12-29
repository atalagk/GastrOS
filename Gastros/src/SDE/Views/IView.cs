using System;
using System.Collections.Generic;
using System.Drawing;
using GastrOs.Sde.Directives;
using System.ComponentModel;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// <para>Represents a generic graphical view of a model, which can be cloned or
    /// removed on request.</para>
    ///  <para>This interface also extends <see cref="INotifyPropertyChanged"/>,
    /// which means it triggers the <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// event whenever its properties are changed. It is the responsibility of the
    /// implementing class to ensure this.</para>
    /// </summary>
    public interface IView : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Event triggered when this view responds to user action signalling a new,
        /// fresh instance of this view is to be created (c.f. multiple occurrences).
        /// </summary>
        event EventHandler NewInstanceRequest;
        /// <summary>
        /// Event triggered when this view responds to user action signalling this
        /// view is to be removed from its parent (c.f. multiple occurrences).
        /// </summary>
        event EventHandler RemoveInstanceRequest;
        /// <summary>
        /// Event triggered when the title of this view has changed.
        /// </summary>
        event EventHandler TitleChanged;
        /// <summary>
        /// Event triggered when the view's visibility has changed.
        /// </summary>
        event EventHandler VisibleChanged;
        /// <summary>
        /// Event raised when this view has been disposed
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// Gets or sets the title of this view - usually the label displayed
        /// on the form.
        /// Triggers the <see cref="TitleChanged"/> event, as well as the
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
        /// if (and only if) the value has changed.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Whether the title is to be visible to the user. Got to rethink whether
        /// this is absolutely necessary...
        /// </summary>
        bool ShowTitle { get; set; }

        /// <summary>
        /// Gets or sets whether this view is visible.
        /// Triggers the <see cref="VisibleChanged"/> event, as well as the
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
        /// if (and only if) the value has changed.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the displayed Size of this view
        /// </summary>
        Size Size { get; set; }

        /// <summary>
        /// Returns the ideal Size required by this view
        /// </summary>
        Size IdealSize { get; }

        /// <summary>
        /// The list of directives associated with this view
        /// </summary>
        IList<IDirective> Directives { get; }

        /// <summary>
        /// Returns true if whether this control should be autoscrolled by
        /// its container, or false it can happily autoscroll by itself.
        /// </summary>
        bool RequiresExternalScrolling { get; }

        /// <summary>
        /// Gets or sets whether another instance of this view can be created
        /// by the user
        /// </summary>
        bool CanAddNewInstance { get; set; }

        /// <summary>
        /// Gets or sets whether this instance of the view can be removed by
        /// the user
        /// </summary>
        bool CanRemoveInstance { get; set; }

        /// <summary>
        /// Resets this view to an empty state. May raise appropriate property
        /// change events as a result.
        /// </summary>
        void Reset();
    }
}