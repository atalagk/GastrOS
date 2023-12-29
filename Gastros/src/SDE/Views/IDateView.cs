using System;
using System.ComponentModel;

namespace GastrOs.Sde.Views
{
    public interface IDateView : IScalarView
    {
        /// <summary>
        /// Triggered when the date component has changed its value
        /// </summary>
        event EventHandler DateChanged;

        /// <summary>
        /// <para>Gets or sets the date component displayed in this view.</para>
        /// <para>Triggers the <see cref="DateChanged"/> event, as well as
        /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
        /// if (and only if) the value has changed.</para>
        /// </summary>
        DateTime? Date { get; set; }
    }
}
