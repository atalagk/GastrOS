using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Test
{
    class SelectRootDialog : Form
    {
        private ComboBox rootSelector;
        private DialogResult result = DialogResult.None;

        public SelectRootDialog(string optName, IList<string> roots)
        {
            Font = new Font("Arial", 9);
            
            TableLayoutPanel basePanel = new TableLayoutPanel();
            basePanel.ColumnCount = 1;
            basePanel.RowCount = 3;
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            basePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            basePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            basePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            
            Label optNameLabel = new Label {Text = "Select root.   (Operational Template: "+optName+")"};
            optNameLabel.AutoSize = true;
            
            rootSelector = new ComboBox();
            rootSelector.DataSource = roots;
            rootSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            
            Button ok = new Button {Text = "OK"};
            ok.Anchor = AnchorStyles.Right;

            basePanel.Controls.Add(optNameLabel, 0, 0);
            basePanel.Controls.Add(rootSelector, 0, 1);
            basePanel.Controls.Add(ok, 0, 2);

            ok.Click += delegate
                            {
                                result = DialogResult.OK;
                                Dispose();
                            };

            basePanel.Dock = DockStyle.Fill;
            basePanel.AutoSize = true;
            Controls.Add(basePanel);

            AutoSize = true;

            Load += delegate
                        {
                            rootSelector.SetBestFitWidthForCombo();
                            CenterToScreen();
                        };
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(300, 120);
            }
        }

        public new DialogResult DialogResult
        {
            get
            {
                return result;
            }
        }

        public string SelectedRoot
        {
            get
            {
                return rootSelector.SelectedItem as string;
            }
        }
    }
}