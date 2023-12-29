using System;
using System.Windows.Forms;
using GastrOs.Sde.Views.WinForms.Generic;

namespace GastrOs.Wrapper.Forms.Overview
{
    public class SearchDialog : AutoHideForm
    {
        private Label searchLabel;
        private TextBox searchBox;
        private Button btnCancel;
        private Button btnOkay;
        public event EventHandler<SearchEventArgs> SearchClick;

        public SearchDialog(Control anchorControl) : base(anchorControl)
        {
            InitializeComponent();
            btnOkay.Click += TriggerSearchEvent;
            btnCancel.Click += CloseForm;
        }

        private void TriggerSearchEvent(object sender, EventArgs e)
        {
            OnSearchClick(new SearchEventArgs(searchBox.Text));
        }

        private void CloseForm(object sender, EventArgs e)
        {
            Hide();
        }

        protected virtual void OnSearchClick(SearchEventArgs e)
        {
            if (SearchClick != null)
                SearchClick(this, e);
        }

        private void InitializeComponent()
        {
            this.searchLabel = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(4, 5);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(59, 13);
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "Search for:";
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                          | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Location = new System.Drawing.Point(72, 3);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(191, 20);
            this.searchBox.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(188, 29);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkay.Location = new System.Drawing.Point(107, 29);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(75, 26);
            this.btnOkay.TabIndex = 2;
            this.btnOkay.Text = "OK";
            this.btnOkay.UseVisualStyleBackColor = true;
            // 
            // SearchDialog
            // 
            this.ClientSize = new System.Drawing.Size(267, 60);
            this.Controls.Add(this.btnOkay);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.searchLabel);
            this.Name = "SearchDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }

    public class SearchEventArgs : EventArgs
    {
        public string SearchString { get; private set; }
        public SearchEventArgs(string searchString)
        {
            SearchString = searchString;
        }
    }
}