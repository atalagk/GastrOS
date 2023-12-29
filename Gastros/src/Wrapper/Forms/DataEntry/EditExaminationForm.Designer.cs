using System.Windows.Forms;

namespace GastrOs.Wrapper.Forms.DataEntry
{
    partial class EditExaminationForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.mainInputPanel = new System.Windows.Forms.TableLayoutPanel();
            this.patientBox = new System.Windows.Forms.GroupBox();
            this.clinicalBox = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnToggleEdit = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.mainInputPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.mainInputPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 564);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtNotes);
            this.groupBox2.Location = new System.Drawing.Point(2, 498);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(665, 63);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Notes";
            // 
            // txtNotes
            // 
            this.txtNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNotes.Location = new System.Drawing.Point(3, 16);
            this.txtNotes.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(659, 44);
            this.txtNotes.TabIndex = 0;
            // 
            // mainInputPanel
            // 
            this.mainInputPanel.ColumnCount = 1;
            this.tableLayoutPanel1.SetColumnSpan(this.mainInputPanel, 2);
            this.mainInputPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainInputPanel.Controls.Add(this.patientBox, 0, 0);
            this.mainInputPanel.Controls.Add(this.clinicalBox, 0, 1);
            this.mainInputPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainInputPanel.Location = new System.Drawing.Point(3, 3);
            this.mainInputPanel.Name = "mainInputPanel";
            this.mainInputPanel.RowCount = 3;
            this.mainInputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainInputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainInputPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainInputPanel.Size = new System.Drawing.Size(791, 490);
            this.mainInputPanel.TabIndex = 1;
            // 
            // patientBox
            // 
            this.patientBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.patientBox.Location = new System.Drawing.Point(3, 3);
            this.patientBox.Name = "patientBox";
            this.patientBox.Size = new System.Drawing.Size(772, 120);
            this.patientBox.TabIndex = 0;
            this.patientBox.TabStop = false;
            this.patientBox.Text = "Patient details";
            // 
            // clinicalBox
            // 
            this.clinicalBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.clinicalBox.Location = new System.Drawing.Point(3, 129);
            this.clinicalBox.Name = "clinicalBox";
            this.clinicalBox.Padding = new System.Windows.Forms.Padding(0);
            this.clinicalBox.Size = new System.Drawing.Size(772, 40);
            this.clinicalBox.TabIndex = 0;
            this.clinicalBox.TabStop = false;
            this.clinicalBox.Text = "Clinical details";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnToggleEdit);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Location = new System.Drawing.Point(672, 499);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(112, 65);
            this.panel1.TabIndex = 5;
            // 
            // btnToggleEdit
            // 
            this.btnToggleEdit.Location = new System.Drawing.Point(19, 5);
            this.btnToggleEdit.Margin = new System.Windows.Forms.Padding(1);
            this.btnToggleEdit.Name = "btnToggleEdit";
            this.btnToggleEdit.Size = new System.Drawing.Size(75, 26);
            this.btnToggleEdit.TabIndex = 2;
            this.btnToggleEdit.Text = "Edit";
            this.btnToggleEdit.UseVisualStyleBackColor = true;
            this.btnToggleEdit.Click += new System.EventHandler(this.btnToggleEdit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(19, 33);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(1);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 26);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 564);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MainForm";
            this.Text = "Examination";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.mainInputPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel mainInputPanel;
        private System.Windows.Forms.GroupBox patientBox;
        private GroupBox clinicalBox;
        private GroupBox groupBox2;
        private TextBox txtNotes;
        private Panel panel1;
        private Button btnToggleEdit;
        private Button btnPrint;
    }
}