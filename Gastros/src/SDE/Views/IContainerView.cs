using System;
using System.Collections.Generic;

namespace GastrOs.Sde.Views
{
    public class ChildEventArgs : EventArgs
    {
        public IView Child { get; private set; }
        public int Index { get; private set; }
        public ChildEventArgs(IView child) : this(child, -1) {}
        public ChildEventArgs(IView child, int index)
        {
            Child = child;
            Index = index;
        }
    }

    public interface IContainerView : IView
    {
        event EventHandler<ChildEventArgs> ChildAdded;
        event EventHandler<ChildEventArgs> ChildRemoved;

        /// <summary>
        /// Gets a live list of all views graphically contained by this view.
        /// Any updates made to this list should trigger either the <see cref="ChildAdded"/>
        /// or the <see cref="ChildRemoved"/> events.
        /// </summary>
        IList<IView> Children
        {
            get;
        }

        /// <summary>
        /// Gets or sets whether this container should be rendered with a frame around it
        /// 
        /// TODO framed changed event?
        /// </summary>
        bool Framed
        {
            get; set;
        }
    }
}