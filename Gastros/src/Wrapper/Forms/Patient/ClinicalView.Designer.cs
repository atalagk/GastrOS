using GastrOs.Wrapper.DataObjects;

namespace GastrOs.Wrapper.Forms.Patient
{
    partial class ClinicalView
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
            this.components = new System.ComponentModel.Container();
            this.clinicalBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.chkCRF = new System.Windows.Forms.CheckBox();
            this.chkHBV = new System.Windows.Forms.CheckBox();
            this.chkHCV = new System.Windows.Forms.CheckBox();
            this.chkHDV = new System.Windows.Forms.CheckBox();
            this.chkHIV = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.clinicalBindingSource)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // clinicalBindingSource
            // 
            this.clinicalBindingSource.DataSource = typeof(GastrOs.Wrapper.DataObjects.Clinical);
            // 
            // chkCRF
            // 
            this.chkCRF.AutoSize = true;
            this.chkCRF.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.clinicalBindingSource, "CRF", true));
            this.chkCRF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCRF.Location = new System.Drawing.Point(3, 3);
            this.chkCRF.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.chkCRF.Name = "chkCRF";
            this.chkCRF.Size = new System.Drawing.Size(47, 17);
            this.chkCRF.TabIndex = 0;
            this.chkCRF.Text = "CRF";
            this.chkCRF.UseVisualStyleBackColor = true;
            // 
            // chkHBV
            // 
            this.chkHBV.AutoSize = true;
            this.chkHBV.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.clinicalBindingSource, "HBV", true));
            this.chkHBV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHBV.Location = new System.Drawing.Point(68, 3);
            this.chkHBV.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.chkHBV.Name = "chkHBV";
            this.chkHBV.Size = new System.Drawing.Size(48, 17);
            this.chkHBV.TabIndex = 0;
            this.chkHBV.Text = "HBV";
            this.chkHBV.UseVisualStyleBackColor = true;
            // 
            // chkHCV
            // 
            this.chkHCV.AutoSize = true;
            this.chkHCV.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.clinicalBindingSource, "HCV", true));
            this.chkHCV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHCV.Location = new System.Drawing.Point(134, 3);
            this.chkHCV.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.chkHCV.Name = "chkHCV";
            this.chkHCV.Size = new System.Drawing.Size(48, 17);
            this.chkHCV.TabIndex = 0;
            this.chkHCV.Text = "HCV";
            this.chkHCV.UseVisualStyleBackColor = true;
            // 
            // chkHDV
            // 
            this.chkHDV.AutoSize = true;
            this.chkHDV.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.clinicalBindingSource, "HDV", true));
            this.chkHDV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHDV.Location = new System.Drawing.Point(200, 3);
            this.chkHDV.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.chkHDV.Name = "chkHDV";
            this.chkHDV.Size = new System.Drawing.Size(49, 17);
            this.chkHDV.TabIndex = 0;
            this.chkHDV.Text = "HDV";
            this.chkHDV.UseVisualStyleBackColor = true;
            // 
            // chkHIV
            // 
            this.chkHIV.AutoSize = true;
            this.chkHIV.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.clinicalBindingSource, "HIV", true));
            this.chkHIV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHIV.Location = new System.Drawing.Point(267, 3);
            this.chkHIV.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.chkHIV.Name = "chkHIV";
            this.chkHIV.Size = new System.Drawing.Size(44, 17);
            this.chkHIV.TabIndex = 0;
            this.chkHIV.Text = "HIV";
            this.chkHIV.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(452, 31);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.chkCRF);
            this.flowLayoutPanel2.Controls.Add(this.chkHBV);
            this.flowLayoutPanel2.Controls.Add(this.chkHCV);
            this.flowLayoutPanel2.Controls.Add(this.chkHDV);
            this.flowLayoutPanel2.Controls.Add(this.chkHIV);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(5, 5);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(442, 21);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // ClinicalView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ClinicalView";
            this.Size = new System.Drawing.Size(452, 31);
            this.Load += new System.EventHandler(this.ClinicalView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.clinicalBindingSource)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkCRF;
        private System.Windows.Forms.CheckBox chkHBV;
        private System.Windows.Forms.CheckBox chkHCV;
        private System.Windows.Forms.CheckBox chkHDV;
        private System.Windows.Forms.CheckBox chkHIV;
        private System.Windows.Forms.BindingSource clinicalBindingSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}