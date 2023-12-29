using System;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// Represents a view that represents a boolean value - checked/unchecked
    /// </summary>
    public interface ICheckView : IScalarView
    {
        /// <summary>
        /// Triggered when the checked status of this view is changed
        /// </summary>
        event EventHandler CheckedChanged;

        /// <summary>
        /// Whether this view is checked
        /// </summary>
        bool Checked { get; set; }
    }
}