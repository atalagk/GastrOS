using GastrOs.Sde.Properties;

namespace GastrOs.Sde.Views.WinForms.Generic
{
    partial class MultiStateCheck
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkButton = new System.Windows.Forms.Button();
            this.checklabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkButton
            // 
            this.checkButton.Image = global::GastrOs.Sde.Properties.Resources.empty;
            this.checkButton.Location = new System.Drawing.Point(1, 1);
            this.checkButton.Margin = new System.Windows.Forms.Padding(1);
            this.checkButton.Name = "checkButton";
            this.checkButton.Size = new System.Drawing.Size(16, 16);
            this.checkButton.TabIndex = 0;
            this.checkButton.UseVisualStyleBackColor = false;
            // 
            // checklabel
            // 
            this.checklabel.Location = new System.Drawing.Point(22, 4);
            this.checklabel.Margin = new System.Windows.Forms.Padding(0);
            this.checklabel.Name = "checklabel";
            this.checklabel.Size = new System.Drawing.Size(45, 15);
            this.checklabel.TabIndex = 1;
            // 
            // MultiStateCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checklabel);
            this.Controls.Add(this.checkButton);
            this.Name = "MultiStateCheck";
            this.Size = new System.Drawing.Size(70, 22);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button checkButton;
        private System.Windows.Forms.Label checklabel;
    }
}