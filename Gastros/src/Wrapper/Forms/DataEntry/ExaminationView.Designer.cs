using GastrOs.Wrapper.DataObjects;

namespace GastrOs.Wrapper.Forms.DataEntry
{
    partial class ExaminationView
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
            this.btnProcedure = new System.Windows.Forms.Button();
            this.btnDiagnoses = new System.Windows.Forms.Button();
            this.btnInterventions = new System.Windows.Forms.Button();
            this.basePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.examDetailsBox = new System.Windows.Forms.GroupBox();
            this.examDetailsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPremed = new System.Windows.Forms.TextBox();
            this.examinationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.endoDateChooser = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.typeChooser = new System.Windows.Forms.ComboBox();
            this.reportDateChooser = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.deviceChooser = new System.Windows.Forms.ComboBox();
            this.deptChooser = new System.Windows.Forms.ComboBox();
            this.txtDoc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.reportBox = new System.Windows.Forms.GroupBox();
            this.txtReport = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.basePanel.SuspendLayout();
            this.examDetailsBox.SuspendLayout();
            this.examDetailsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.examinationBindingSource)).BeginInit();
            this.reportBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnProcedure
            // 
            this.btnProcedure.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcedure.Location = new System.Drawing.Point(9, 2);
            this.btnProcedure.Name = "btnProcedure";
            this.btnProcedure.Size = new System.Drawing.Size(106, 26);
            this.btnProcedure.TabIndex = 0;
            this.btnProcedure.Text = "PROCEDURE";
            this.btnProcedure.UseVisualStyleBackColor = true;
            this.btnProcedure.Click += new System.EventHandler(this.ShowIntermediateForm);
            // 
            // btnDiagnoses
            // 
            this.btnDiagnoses.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDiagnoses.Location = new System.Drawing.Point(142, 2);
            this.btnDiagnoses.Name = "btnDiagnoses";
            this.btnDiagnoses.Size = new System.Drawing.Size(95, 26);
            this.btnDiagnoses.TabIndex = 0;
            this.btnDiagnoses.Text = "DIAGNOSIS";
            this.btnDiagnoses.UseVisualStyleBackColor = true;
            this.btnDiagnoses.Click += new System.EventHandler(this.ShowDiagnosesForm);
            // 
            // btnInterventions
            // 
            this.btnInterventions.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInterventions.Location = new System.Drawing.Point(264, 2);
            this.btnInterventions.Name = "btnInterventions";
            this.btnInterventions.Size = new System.Drawing.Size(125, 26);
            this.btnInterventions.TabIndex = 0;
            this.btnInterventions.Text = "INTERVENTIONS";
            this.btnInterventions.UseVisualStyleBackColor = true;
            this.btnInterventions.Click += new System.EventHandler(this.ShowInterventionsForm);
            // 
            // basePanel
            // 
            this.basePanel.AutoScroll = true;
            this.basePanel.AutoSize = true;
            this.basePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.basePanel.Controls.Add(this.examDetailsBox);
            this.basePanel.Controls.Add(this.reportBox);
            this.basePanel.Controls.Add(this.panel1);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(772, 314);
            this.basePanel.TabIndex = 4;
            this.basePanel.WrapContents = false;
            // 
            // examDetailsBox
            // 
            this.examDetailsBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.examDetailsBox.Controls.Add(this.examDetailsPanel);
            this.examDetailsBox.Location = new System.Drawing.Point(2, 2);
            this.examDetailsBox.Margin = new System.Windows.Forms.Padding(2);
            this.examDetailsBox.Name = "examDetailsBox";
            this.examDetailsBox.Padding = new System.Windows.Forms.Padding(2);
            this.examDetailsBox.Size = new System.Drawing.Size(763, 125);
            this.examDetailsBox.TabIndex = 2;
            this.examDetailsBox.TabStop = false;
            this.examDetailsBox.Text = "Examination details";
            // 
            // examDetailsPanel
            // 
            this.examDetailsPanel.AutoSize = true;
            this.examDetailsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.examDetailsPanel.ColumnCount = 4;
            this.examDetailsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 114F));
            this.examDetailsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.examDetailsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.examDetailsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 309F));
            this.examDetailsPanel.Controls.Add(this.label5, 2, 0);
            this.examDetailsPanel.Controls.Add(this.label1, 0, 3);
            this.examDetailsPanel.Controls.Add(this.txtPremed, 1, 3);
            this.examDetailsPanel.Controls.Add(this.endoDateChooser, 3, 0);
            this.examDetailsPanel.Controls.Add(this.label7, 0, 0);
            this.examDetailsPanel.Controls.Add(this.label2, 2, 1);
            this.examDetailsPanel.Controls.Add(this.typeChooser, 1, 0);
            this.examDetailsPanel.Controls.Add(this.reportDateChooser, 3, 1);
            this.examDetailsPanel.Controls.Add(this.label6, 0, 1);
            this.examDetailsPanel.Controls.Add(this.deviceChooser, 1, 1);
            this.examDetailsPanel.Controls.Add(this.deptChooser, 3, 2);
            this.examDetailsPanel.Controls.Add(this.txtDoc, 1, 2);
            this.examDetailsPanel.Controls.Add(this.label3, 0, 2);
            this.examDetailsPanel.Controls.Add(this.label4, 2, 2);
            this.examDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.examDetailsPanel.Location = new System.Drawing.Point(2, 15);
            this.examDetailsPanel.Margin = new System.Windows.Forms.Padding(1);
            this.examDetailsPanel.Name = "examDetailsPanel";
            this.examDetailsPanel.RowCount = 4;
            this.examDetailsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.examDetailsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.examDetailsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.examDetailsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.examDetailsPanel.Size = new System.Drawing.Size(759, 108);
            this.examDetailsPanel.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(337, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 24);
            this.label5.TabIndex = 0;
            this.label5.Text = "Endoscopy Date:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Premedication:";
            // 
            // txtPremed
            // 
            this.examDetailsPanel.SetColumnSpan(this.txtPremed, 2);
            this.txtPremed.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.examinationBindingSource, "Premedication", true));
            this.txtPremed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPremed.Location = new System.Drawing.Point(117, 84);
            this.txtPremed.Name = "txtPremed";
            this.txtPremed.Size = new System.Drawing.Size(330, 20);
            this.txtPremed.TabIndex = 3;
            // 
            // examinationBindingSource
            // 
            this.examinationBindingSource.DataSource = typeof(Examination);
            // 
            // endoDateChooser
            // 
            this.endoDateChooser.CustomFormat = "";
            this.endoDateChooser.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.examinationBindingSource, "EndoscopyDate", true));
            this.endoDateChooser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.endoDateChooser.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.endoDateChooser.Location = new System.Drawing.Point(453, 3);
            this.endoDateChooser.Name = "endoDateChooser";
            this.endoDateChooser.Size = new System.Drawing.Size(303, 20);
            this.endoDateChooser.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 3);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 24);
            this.label7.TabIndex = 0;
            this.label7.Text = "Examination type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(337, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 24);
            this.label2.TabIndex = 0;
            this.label2.Text = "Report Date:";
            // 
            // typeChooser
            // 
            this.typeChooser.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.examinationBindingSource, "EndoscopyType", true));
            this.typeChooser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeChooser.FormattingEnabled = true;
            this.typeChooser.Location = new System.Drawing.Point(117, 3);
            this.typeChooser.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.typeChooser.Name = "typeChooser";
            this.typeChooser.Size = new System.Drawing.Size(207, 21);
            this.typeChooser.TabIndex = 3;
            // 
            // reportDateChooser
            // 
            this.reportDateChooser.Checked = false;
            this.reportDateChooser.CustomFormat = "";
            this.reportDateChooser.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.examinationBindingSource, "ReportDate", true));
            this.reportDateChooser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportDateChooser.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.reportDateChooser.Location = new System.Drawing.Point(453, 30);
            this.reportDateChooser.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.reportDateChooser.Name = "reportDateChooser";
            this.reportDateChooser.ShowCheckBox = true;
            this.reportDateChooser.Size = new System.Drawing.Size(303, 20);
            this.reportDateChooser.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 30);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 24);
            this.label6.TabIndex = 0;
            this.label6.Text = "Device:";
            // 
            // deviceChooser
            // 
            this.deviceChooser.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.examinationBindingSource, "Device", true));
            this.deviceChooser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceChooser.FormattingEnabled = true;
            this.deviceChooser.Location = new System.Drawing.Point(117, 30);
            this.deviceChooser.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.deviceChooser.Name = "deviceChooser";
            this.deviceChooser.Size = new System.Drawing.Size(207, 21);
            this.deviceChooser.TabIndex = 3;
            // 
            // deptChooser
            // 
            this.deptChooser.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.examinationBindingSource, "Dept", true));
            this.deptChooser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deptChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deptChooser.FormattingEnabled = true;
            this.deptChooser.Location = new System.Drawing.Point(453, 57);
            this.deptChooser.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.deptChooser.Name = "deptChooser";
            this.deptChooser.Size = new System.Drawing.Size(303, 21);
            this.deptChooser.TabIndex = 3;
            // 
            // txtDoc
            // 
            this.txtDoc.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.examinationBindingSource, "Doctor", true));
            this.txtDoc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDoc.Location = new System.Drawing.Point(117, 57);
            this.txtDoc.Margin = new System.Windows.Forms.Padding(3, 3, 10, 2);
            this.txtDoc.Name = "txtDoc";
            this.txtDoc.Size = new System.Drawing.Size(207, 20);
            this.txtDoc.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 57);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 24);
            this.label3.TabIndex = 0;
            this.label3.Text = "Doctor:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(337, 57);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 24);
            this.label4.TabIndex = 0;
            this.label4.Text = "Department:";
            // 
            // reportBox
            // 
            this.reportBox.Controls.Add(this.txtReport);
            this.reportBox.Location = new System.Drawing.Point(3, 132);
            this.reportBox.Name = "reportBox";
            this.reportBox.Size = new System.Drawing.Size(762, 141);
            this.reportBox.TabIndex = 3;
            this.reportBox.TabStop = false;
            this.reportBox.Text = "Report";
            // 
            // txtReport
            // 
            this.txtReport.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.examinationBindingSource, "ReportText", true));
            this.txtReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReport.Location = new System.Drawing.Point(3, 16);
            this.txtReport.Multiline = true;
            this.txtReport.Name = "txtReport";
            this.txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReport.Size = new System.Drawing.Size(756, 122);
            this.txtReport.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnInterventions);
            this.panel1.Controls.Add(this.btnDiagnoses);
            this.panel1.Controls.Add(this.btnProcedure);
            this.panel1.Location = new System.Drawing.Point(2, 278);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(2);
            this.panel1.Size = new System.Drawing.Size(763, 33);
            this.panel1.TabIndex = 5;
            // 
            // ExaminationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.basePanel);
            this.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.Name = "ExaminationView";
            this.Size = new System.Drawing.Size(772, 314);
            this.Load += new System.EventHandler(this.ExaminationView_Load);
            this.basePanel.ResumeLayout(false);
            this.examDetailsBox.ResumeLayout(false);
            this.examDetailsBox.PerformLayout();
            this.examDetailsPanel.ResumeLayout(false);
            this.examDetailsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.examinationBindingSource)).EndInit();
            this.reportBox.ResumeLayout(false);
            this.reportBox.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource examinationBindingSource;
        private System.Windows.Forms.Button btnProcedure;
        private System.Windows.Forms.Button btnInterventions;
        private System.Windows.Forms.Button btnDiagnoses;
        private System.Windows.Forms.FlowLayoutPanel basePanel;
        private System.Windows.Forms.GroupBox examDetailsBox;
        private System.Windows.Forms.TableLayoutPanel examDetailsPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPremed;
        private System.Windows.Forms.DateTimePicker endoDateChooser;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox typeChooser;
        private System.Windows.Forms.DateTimePicker reportDateChooser;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox deviceChooser;
        private System.Windows.Forms.ComboBox deptChooser;
        private System.Windows.Forms.TextBox txtDoc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox reportBox;
        private System.Windows.Forms.TextBox txtReport;
        private System.Windows.Forms.Panel panel1;
    }
}