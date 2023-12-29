using System;

namespace GastrOs.Sde.Views
{
    /// <summary>
    /// Represents a view that invokes another view ("splash"). The view
    /// itself is simple (e.g. button) and the main content resides in the
    /// splashed view
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISplasherView<T> : IView where T : IView
    {
        /// <summary>
        /// The splash view that actually contains the interesting content
        /// </summary>
        T SplashedView { get; set; }

        /// <summary>
        /// Opens the splash view and raises the <see cref="SplashOpened"/> event
        /// (only when splash view was closed beforehand).
        /// </summary>
        void OpenSplash();

        /// <summary>
        /// Closes the splash view and raises the <see cref="SplashClosed"/> event
        /// (only when splash view was open beforehand)
        /// </summary>
        void CloseSplash();

        /// <summary>
        /// Triggered when the splashed view is opened (by user)
        /// </summary>
        event EventHandler SplashOpened;
        /// <summary>
        /// Triggered when the splashed view is closed (by user)
        /// </summary>
        event EventHandler SplashClosed;
    }
}