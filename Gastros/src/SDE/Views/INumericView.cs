using System;
using System.ComponentModel;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// <para>Represents a view that contains a numeric component.</para>
    /// </summary>
    public interface INumericView : IView
    {
        /// <summary>
        /// Triggered when the numeric component has changed its value
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// <para>Gets or sets the numeric value displayed in this view.
        /// The value may be unspecified, in which case it is set to null.</para>
        /// <para>Triggers the <see cref="ValueChanged"/> event, as well as
        /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
        /// if (and only if) the value has changed.</para>
        /// </summary>
        decimal? Value { get; set; }

        /// <summary>
        /// The minimum allowed value
        /// </summary>
        decimal MinValue { get; set; }

        /// <summary>
        /// The maximum allowed value
        /// </summary>
        decimal MaxValue { get; set; }
    }
}