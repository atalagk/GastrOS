using System;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// <para>Represents a view that contains a numeric component.</para>
    /// </summary>
    public interface INumericView : IScalarView
    {
        /// <summary>
        /// Triggered when the numeric component has changed its value
        /// </summary>
        event EventHandler ValueChanged;

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