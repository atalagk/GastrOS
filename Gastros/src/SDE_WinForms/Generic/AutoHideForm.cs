using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GastrOs.Sde.Views.WinForms.Generic
{
    /// <summary>
    /// A form that closes when user clicks outside the form area. This behaviour
    /// can be turned on/off. Also can specify the "anchor control", such that
    /// when this form becomes visible its position is aligned to that of the
    /// anchor control.
    /// </summary>
    public class AutoHideForm : Form
    {
        private readonly Control anchorControl;
        private bool autoHide = false;

        public AutoHideForm() : this(null) {}

        public AutoHideForm(Control anchorControl)
        {
            this.anchorControl = anchorControl;
            AutoHide = true;
        }

        /// <summary>
        /// Gets or sets whether the auto-hide behaviour is on.
        /// </summary>
        public bool AutoHide
        {
            get
            {
                return autoHide;
            }
            set
            {
                if (autoHide == value)
                    return;
                autoHide = value;
                if (autoHide)
                    Deactivate += HandleDeactivate;
                else
                    Deactivate -= HandleDeactivate;
            }
        }

        private void HandleDeactivate(object sender, EventArgs e)
        {
            Hide();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        /// <summary>
        /// Positions this form appropriately
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                if (anchorControl == null)
                {
                    CenterToParent();
                    return;
                }   
                Rectangle screen = Screen.GetWorkingArea(this);
                Point newAbsLoc = anchorControl.PointToScreen(new Point(anchorControl.Width, 0));
                int bottomDiff = screen.Bottom - newAbsLoc.Y - Height;
                if (bottomDiff < 0)
                {
                    newAbsLoc.Y += bottomDiff;
                }
                int rightDiff = screen.Right - newAbsLoc.X - Width;
                if (rightDiff < 0)
                {
                    newAbsLoc.X += rightDiff;
                }
                Location = newAbsLoc;
            }
            else if (Owner != null)
            {
                Owner.Focus();
            }
        }
    }
}