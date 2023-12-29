using System;

namespace GastrOs.Sde.Directives
{
    /// <summary>
    /// Represent a GUI directive that modifies how the generated GUI elements are
    /// organised and presented
    /// </summary>
    public interface IDirective
    {
        /// <summary>
        /// Returns the name of this directive
        /// </summary>
        string Name { get; }
        /// <summary>
        /// This method is called right after this directive is created, in order
        /// to set up its parameters according to what's specified in the operational
        /// template
        /// </summary>
        /// <param name="parameters">the string parameters</param>
        /// <exception cref="FormatException">If parameters aren't formatted correctly</exception>
        void ParseParameters(params string[] parameters);
    }
}
