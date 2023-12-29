namespace GastrOs.Wrapper.Forms
{
    partial class EntryScreen
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnDataEntry = new System.Windows.Forms.Button();
            this.btnSysops = new System.Windows.Forms.Button();
            this.lnkDisclaimer = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.lnkWebsite = new System.Windows.Forms.LinkLabel();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(151, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(481, 62);
            this.label1.TabIndex = 0;
            this.label1.Text = "GastrOS\r\nopenEHR based Endoscopy Application";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDataEntry
            // 
            this.btnDataEntry.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDataEntry.Location = new System.Drawing.Point(225, 231);
            this.btnDataEntry.Name = "btnDataEntry";
            this.btnDataEntry.Size = new System.Drawing.Size(312, 37);
            this.btnDataEntry.TabIndex = 1;
            this.btnDataEntry.Text = "&DATA ENTRY/UPDATE";
            this.btnDataEntry.UseVisualStyleBackColor = true;
            this.btnDataEntry.Click += new System.EventHandler(this.btnDataEntry_Click);
            // 
            // btnSysops
            // 
            this.btnSysops.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSysops.Location = new System.Drawing.Point(225, 332);
            this.btnSysops.Name = "btnSysops";
            this.btnSysops.Size = new System.Drawing.Size(312, 37);
            this.btnSysops.TabIndex = 2;
            this.btnSysops.Text = "&SYSTEM OPERATIONS";
            this.btnSysops.UseVisualStyleBackColor = true;
            // 
            // lnkDisclaimer
            // 
            this.lnkDisclaimer.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lnkDisclaimer.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkDisclaimer.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lnkDisclaimer.Location = new System.Drawing.Point(247, 118);
            this.lnkDisclaimer.Name = "lnkDisclaimer";
            this.lnkDisclaimer.Size = new System.Drawing.Size(264, 96);
            this.lnkDisclaimer.TabIndex = 3;
            this.lnkDisclaimer.TabStop = true;
            this.lnkDisclaimer.Text = "This software is based on Minimal Standard Terminology For Data Processing In Dig" +
                                      "estive Endoscopy (MST 2.0). For more information about copyright issues, please " +
                                      "click on this text...";
            this.lnkDisclaimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkDisclaimer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDisclaimer_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label2.Location = new System.Drawing.Point(487, 518);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Copyright © 2009 - University of Auckland";
            // 
            // lnkWebsite
            // 
            this.lnkWebsite.AutoSize = true;
            this.lnkWebsite.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkWebsite.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lnkWebsite.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lnkWebsite.Location = new System.Drawing.Point(49, 510);
            this.lnkWebsite.Name = "lnkWebsite";
            this.lnkWebsite.Size = new System.Drawing.Size(235, 23);
            this.lnkWebsite.TabIndex = 5;
            this.lnkWebsite.TabStop = true;
            this.lnkWebsite.Text = "http://gastros.sourceforge.net";
            this.lnkWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWebsite_LinkClicked);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(225, 428);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(312, 37);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "&EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // EntryScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(794, 574);
            this.Controls.Add(this.lnkWebsite);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lnkDisclaimer);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSysops);
            this.Controls.Add(this.btnDataEntry);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "EntryScreen";
            this.Text = "GastrOS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDataEntry;
        private System.Windows.Forms.Button btnSysops;
        private System.Windows.Forms.LinkLabel lnkDisclaimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel lnkWebsite;
        private System.Windows.Forms.Button btnExit;
    }
}