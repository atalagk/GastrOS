using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Views.WinForms.Generic
{
    /// <summary>
    /// 
    /// </summary>
    public class BoxPanel : Panel
    {
        private Control baseContainer; //either this or frame
        private Control contents;
        private bool framed;
        private string text;

        public BoxPanel()
        {
            InitComponents();
        }

        private void InitComponents()
        {
            SuspendLayout();

            contents = new FlowLayoutPanel
                           {
                               AutoSize = true,
                               FlowDirection = FlowDirection.TopDown,
                               Margin = new Padding(0),
                               Dock = DockStyle.Fill,
                               WrapContents = false
                           };

            Controls.Add(contents);
            baseContainer = this;

            ResumeLayout(false);

#if (DEBUG)
            contents.MouseMove += relayMouseMove;
#endif
        }

#if (DEBUG)
        void relayMouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }
#endif

        /// <summary>
        /// Gets or sets whether a frame is to be drawn around the panel
        /// </summary>
        public bool Framed
        {
            get { return framed;}
            set
            {
                if (framed == value)
                    return;
                framed = value;

                baseContainer.Controls.Clear();

                if (framed)
                {
                    baseContainer = new GroupBox
                                        {
                                            Margin = new Padding(0),
                                            Font = GastrOsConfig.LayoutConfig.DefaultFont.Value,
                                            Dock = DockStyle.Fill,
                                            AutoSize = true,
                                            Text = text
                                        };
                    Controls.Add(baseContainer);
                }
                else
                {
                    baseContainer.Dispose(); //dispose old GroupBox
                    baseContainer = this;
                }

                baseContainer.Controls.Add(contents);
            }
        }

        /// <summary>
        /// Return the amount of visual space occuppied by the frame (if any)
        /// </summary>
        public Size FrameAllowance
        {
            get
            {
                if (Framed)
                {
                    return ((GroupBox) baseContainer).GetFrameAllowance();
                }
                return Size.Empty;
            }
        }

        public Control Contents
        {
            get
            {
                return contents;
            }
            set
            {
                if (contents == value)
                    return;
                if (contents != null) //remove old contents
                    baseContainer.Controls.Remove(contents);
                contents = value;
                if (contents != null) //replace new contents
                {
                    contents.Margin = new Padding(0);
                    contents.Dock = DockStyle.Fill;
                    baseContainer.Controls.Add(contents);
                }
            }
        }

        public void AddChild(Control child)
        {
            //child.Width = contents.ClientRectangle.Width - child.Margin.Horizontal;
            contents.Controls.Add(child);
        }

        public void InsertChild(int index, Control child)
        {
            contents.Controls.Add(child);
            contents.Controls.SetChildIndex(child, index);
        }

        public void RemoveChild(Control child)
        {
            contents.Controls.Remove(child);
        }

        public override string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text == value)
                    return;
                text = value;
                if (Framed)
                    baseContainer.Text = text;
                OnTextChanged(EventArgs.Empty);
            }
        }

        public IList<Control> Children
        {
            get
            {
                List<Control> children = new List<Control>();
                foreach (Control child in contents.Controls)
                {
                    children.Add(child);
                }
                return children;
            }
        }
    }
}