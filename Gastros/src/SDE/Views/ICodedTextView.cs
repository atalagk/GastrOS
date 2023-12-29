using System.Collections.Generic;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Support;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// <para>Represents a view that contains a coded text component - i.e.
    /// an enumerated list of possible values.</para>
    /// </summary>
    public interface ICodedTextView : ITextView
    {
        /// <summary>
        /// The list of items to display in the dropdown menu.
        /// MAY be null if not present yet.
        /// </summary>
        IList<OntologyItem> ChoiceList
        {
            get; set;
        }

        /// <summary>
        /// If set to Append, then will append description and term together
        /// in the display; if set to Organise, then will organise the items
        /// into groups according to description
        /// </summary>
        ShowValueContextMode ShowValueContext
        {
            get; set;
        }
    }
}