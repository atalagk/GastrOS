using System;
using System.Drawing;
using System.Windows.Forms;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Views.WinForms.Generic;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Windows forms implementation of a splasher view, which is a single button
    /// that, on click, pops up a dialog containing the inner view.
    /// </summary>
    public class SplasherWidget<T> : WidgetBase, ISplasherView<T> where T : class, IView
    {
        public const int MaxSplashWidth = 200; //CFG
        public const int MaxSplashHeight = 320; //CFG

        public event EventHandler SplashOpened;
        public event EventHandler SplashClosed;

        private Button splashButton;
        private Form popupDialog;
        private T splashedView;

        public SplasherWidget()
        {
            splashButton = new Button();
            splashButton.AutoSize = true;
            splashButton.Margin = new Padding(0);
            Controls.Add(splashButton);

            /* initialising button */
            splashButton.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            splashButton.Click += delegate
                                      {
                                          OpenSplash();
                                      };

            popupDialog = new AutoHideForm(this) { AutoSize = true };
            popupDialog.MinimizeBox = false;
            popupDialog.MaximizeBox = false;
            popupDialog.VisibleChanged += delegate
                                              {
                                                  if (!popupDialog.Visible)
                                                      OnSplashClose(EventArgs.Empty);
                                              };

            popupDialog.Width = MaxSplashWidth;
            popupDialog.Height = MaxSplashHeight;
        }

        public override string Title
        {
            get { return splashButton.Text; }
            set
            {
                if (Title == value)
                    return;
                splashButton.Text = value;
                popupDialog.Text = value;
                //fire off event as per interface contract
                OnTitleChanged(EventArgs.Empty);
            }
        }

        public T SplashedView
        {
            get { return splashedView; }
            set
            {
                //Don't know if this is the most elegant way to deal with adding views
                //as winforms controls. Possible to somehow "dynamically dispatch" the
                //part that deals explicitly with controls??
                Check.Require(value is Control);
                splashedView = value;

                Control splashedViewAsControl = splashedView as Control;
                splashedViewAsControl.Dock = DockStyle.Fill;
                popupDialog.Controls.Add(splashedViewAsControl);

                //Support the functionality to close when 'esc' key is pressed within the splashed view
                splashedViewAsControl.KeyPress += FilterKeyPress;
            }
        }

        private void FilterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
                CloseSplash();
        }

        public void OpenSplash()
        {
            if (splashedView == null)
                throw new InvalidOperationException("Must set SplashedView first before attempting to open it");
            Size minSize = splashedView.IdealSize;
            popupDialog.AutoScrollMinSize = minSize;
            popupDialog.ClientSize = new Size(Math.Min(minSize.Width, MaxSplashWidth),
                                              Math.Min(minSize.Height, MaxSplashHeight));
            popupDialog.Show();
            OnSplashOpen(EventArgs.Empty);
        }

        public void CloseSplash()
        {
            popupDialog.Hide();
            OnSplashClose(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the event <see cref="SplashOpened"/>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSplashOpen(EventArgs e)
        {
            if (SplashOpened != null)
            {
                SplashOpened(this, e);
            }
        }

        /// <summary>
        /// Raises the event <see cref="SplashClosed"/>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSplashClose(EventArgs e)
        {
            if (SplashClosed != null)
            {
                SplashClosed(this, e);
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(60, 20); //CFG
            }
        }

        public override Size IdealSize
        {
            get
            {
                return splashButton.PreferredSize;
            }
        }

        public override void Reset()
        {
            if (splashedView != null)
                splashedView.Reset();
        }
    }
}