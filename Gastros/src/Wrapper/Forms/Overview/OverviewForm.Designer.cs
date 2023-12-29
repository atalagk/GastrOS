namespace GastrOs.Wrapper.Forms.Overview
{
    partial class OverviewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.patientsContainer = new System.Windows.Forms.GroupBox();
            this.examsContainer = new System.Windows.Forms.GroupBox();
            this.splitPanel = new System.Windows.Forms.SplitContainer();
            this.splitPanel.Panel1.SuspendLayout();
            this.splitPanel.Panel2.SuspendLayout();
            this.splitPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // patientsContainer
            // 
            this.patientsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientsContainer.Location = new System.Drawing.Point(0, 0);
            this.patientsContainer.Name = "patientsContainer";
            this.patientsContainer.Size = new System.Drawing.Size(749, 244);
            this.patientsContainer.TabIndex = 0;
            this.patientsContainer.TabStop = false;
            this.patientsContainer.Text = "Patients";
            // 
            // examsContainer
            // 
            this.examsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.examsContainer.Location = new System.Drawing.Point(0, 0);
            this.examsContainer.Name = "examsContainer";
            this.examsContainer.Size = new System.Drawing.Size(749, 281);
            this.examsContainer.TabIndex = 0;
            this.examsContainer.TabStop = false;
            this.examsContainer.Text = "Examinations";
            // 
            // splitPanel
            // 
            this.splitPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitPanel.Location = new System.Drawing.Point(0, 0);
            this.splitPanel.Name = "splitPanel";
            this.splitPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitPanel.Panel1
            // 
            this.splitPanel.Panel1.Controls.Add(this.patientsContainer);
            // 
            // splitPanel.Panel2
            // 
            this.splitPanel.Panel2.Controls.Add(this.examsContainer);
            this.splitPanel.Size = new System.Drawing.Size(749, 529);
            this.splitPanel.SplitterDistance = 244;
            this.splitPanel.TabIndex = 1;
            // 
            // DataEntryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 529);
            this.Controls.Add(this.splitPanel);
            this.Name = "DataEntryForm";
            this.Text = "GastrOS - data entry";
            this.splitPanel.Panel1.ResumeLayout(false);
            this.splitPanel.Panel2.ResumeLayout(false);
            this.splitPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox patientsContainer;
        private System.Windows.Forms.GroupBox examsContainer;
        private System.Windows.Forms.SplitContainer splitPanel;

    }
}