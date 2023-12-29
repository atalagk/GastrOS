using System;
using System.ComponentModel;
using GastrOs.Sde.Support;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// <para>Represents a view that displays "core concepts" - essentially a group
    /// of fields that can be shown or hidden via a switch (e.g. check box)
    /// that denotes the presence of the concept.</para>
    /// </summary>
    public interface ICoreConceptView : IContainerView
    {
        /// <summary>
        /// Triggered when the presence (yes/no) has chanced its value
        /// </summary>
        event EventHandler PresenceChanged;

        /// <summary>
        /// Sets the available presence states (present, absent, unknown, etc.)
        /// </summary>
        /// <param name="states"></param>
        void SetAvailablePresenceStates(PresenceState states);

        /// <summary>
        /// <para>Gets or sets whether the concept represented by this view
        /// is present in the data instance.</para>
        /// <para>Triggers the <see cref="PresenceChanged"/> event, as well as
        /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
        /// if (and only if) the value has changed.</para>
        /// </summary>
        PresenceState Presence { get; set; }
    }
}