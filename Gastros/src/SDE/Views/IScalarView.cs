using GastrOs.Sde.Engine;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// A view that represents a single-valued item (a DataValue)
    /// </summary>
    public interface IScalarView : IView
    {
        /// <summary>
        /// The data value provider that maps the value on this view to and
        /// from a DataValue object
        /// </summary>
        IDataValueProvider DataValueProvider { get; set; }

        object Value { get; set; }
    }
}
