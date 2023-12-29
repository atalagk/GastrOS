namespace GastrOs.Wrapper.Forms.DataEntry
{
    partial class IntermediateForm
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
            this.btnExam = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.buttonsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnReport = new System.Windows.Forms.Button();
            this.buttonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExam
            // 
            this.btnExam.Location = new System.Drawing.Point(3, 50);
            this.btnExam.Name = "btnExam";
            this.btnExam.Size = new System.Drawing.Size(127, 30);
            this.btnExam.TabIndex = 0;
            this.btnExam.Text = "Examination";
            this.btnExam.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(269, 50);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(128, 30);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.AutoSize = true;
            this.buttonsPanel.ColumnCount = 3;
            this.buttonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.buttonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.buttonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.buttonsPanel.Controls.Add(this.btnExam, 0, 1);
            this.buttonsPanel.Controls.Add(this.btnReport, 1, 1);
            this.buttonsPanel.Controls.Add(this.btnBack, 2, 1);
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsPanel.Location = new System.Drawing.Point(0, 0);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.RowCount = 2;
            this.buttonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buttonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buttonsPanel.Size = new System.Drawing.Size(400, 83);
            this.buttonsPanel.TabIndex = 2;
            // 
            // btnReport
            // 
            this.btnReport.Location = new System.Drawing.Point(136, 50);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(127, 30);
            this.btnReport.TabIndex = 0;
            this.btnReport.Text = "Report";
            this.btnReport.UseVisualStyleBackColor = true;
            // 
            // IntermediateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(400, 83);
            this.ControlBox = false;
            this.Controls.Add(this.buttonsPanel);
            this.Name = "IntermediateForm";
            this.Text = "Procedure";
            this.buttonsPanel.ResumeLayout(false);
            this.buttonsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExam;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.TableLayoutPanel buttonsPanel;
        private System.Windows.Forms.Button btnReport;
    }
}