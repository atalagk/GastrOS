using System;
using System.Collections.Generic;

namespace GastrOs.Sde.Views
{
    public class SelectionEventArgs : EventArgs
    {
        public object Item { get; private set; }
        public bool Selected { get; private set; }
        public SelectionEventArgs(object item, bool selected)
        {
            Item = item;
            Selected = selected;
        }
    }

    public delegate string ToString(object obj);

    /// <summary>
    /// Represents a collection of items from a finite set. Useful for
    /// representing a group of mutually-exclusive values of the same
    /// "type".
    /// </summary>
    public interface IMultiChoiceView : IView
    {
        /// <summary>
        /// Raised when items have been selected or deselected
        /// </summary>
        event EventHandler<SelectionEventArgs> ItemSelectionChanged;

        /// <summary>
        /// Sets the function (that takes in an object and outputs a string) to
        /// be invoked when displaying the items. By default it's the
        /// object.ToString() function.
        /// </summary>
        /// <param name="func"></param>
        void SetDisplayFunction(ToString func);

        /// <summary>
        /// Add item to the list of selectable items
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool AddSelectableItem(object item);
        /// <summary>
        /// Remove item from the list of selectable items
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool RemoveSelectableItem(object item);

        /// <summary>
        /// Gets the currently selected items
        /// </summary>
        IEnumerable<object> SelectedItems { get; }
        /// <summary>
        /// Sets selected status of the item (it MUST be in the list of
        /// selectable items - otherwise an exception will be thrown)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        bool SetSelected(object item, bool selected);

        /// <summary>
        /// Denotes whether more items can be selected
        /// </summary>
        bool CanSelectMore { get; set; }

        /// <summary>
        /// Gets or sets whether this view should be rendered with a border around it
        /// </summary>
        bool Framed { get; set; }

        /// <summary>
        /// Gets or sets whether this view should display title as caption
        /// </summary>
        bool ShowTitle { get; set; }
    }
}