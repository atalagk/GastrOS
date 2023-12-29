using System;
using System.ComponentModel;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// <para>Represents a view that contains a text component (either
    /// one line text or free text).</para>
    /// </summary>
    public interface ITextView : IScalarView
    {
        /// <summary>
        /// Triggered when the text component has changed its value
        /// </summary>
        event EventHandler TextChanged;

        /// <summary>
        /// <para>Gets or sets the text component displayed in this view.</para>
        /// <para>Triggers the <see cref="TextChanged"/> event, as well as
        /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
        /// if (and only if) the value has changed.</para>
        /// </summary>
        string Text { get; set; }
    }
}