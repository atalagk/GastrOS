using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// <para>Represents a view that contains a numeric component with a "unit"
    /// (or proportion).</para>
    /// </summary>
    public interface INumericUnitView : INumericView
    {
        /// <summary>
        /// Triggered when the "unit" part has changed value
        /// </summary>
        event EventHandler UnitChanged;

        /// <summary>
        /// <para>Gets or sets the "unit" part that sits next to the numeric.</para>
        /// <para>Triggers the <see cref="UnitChanged"/> event, as well as
        /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
        /// if (and only if) the value has changed.</para>
        /// </summary>
        string Unit { get; set; }

        /// <summary>
        /// The list of allowed units. Null if any units are allowed.
        /// </summary>
        IList<string> AvailableUnits
        {
            get;
            set;
        }
    }
}