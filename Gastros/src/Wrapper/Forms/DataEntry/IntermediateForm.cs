using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Wrapper.DataObjects;

namespace GastrOs.Wrapper.Forms.DataEntry
{
    public partial class IntermediateForm : Form
    {
        public static Size DefaultButtonSize = new Size(127, 30);
        
        /// <summary>
        /// Raised when one of the buttons corresponding to an organ (findings) is clicked
        /// </summary>
        public event EventHandler<SdeConceptEventArgs> FindingsClick;
        /// <summary>
        /// Raised when the "examination info" button is clicked
        /// </summary>
        public event EventHandler ExamInfoClick;
        /// <summary>
        /// Raised when the "report" button is clicked
        /// </summary>
        public event EventHandler<CancelEventArgs> ReportRequest;
        
        private int organs;

        public IntermediateForm()
        {
            InitializeComponent();
            btnReport.Size = DefaultButtonSize;
            btnExam.Size = DefaultButtonSize;
            btnBack.Size = DefaultButtonSize;

            Load += delegate 
                        {
                            CenterToScreen();
                        };
            btnReport.Click += doReport;
            btnExam.Click += OnExamInfoClick;
            btnBack.Click += delegate { Dispose(); };
        }

        public void AddOrganButton(string text, SdeConcept organ)
        {
            Button organButton = new Button();
            //Tag the button with the appropriate object that contains the archetype ID
            organButton.Tag = organ;
            organButton.Text = text;
            organButton.Click += OnOrganClick;
            organButton.Size = DefaultButtonSize;

            //There should really be a better implementation of a GridPanel.
            //Now resort to dynamically expanding a TableLayoutPanel (row-wise).

            int col = organs % buttonsPanel.ColumnCount;
            int row = organs / buttonsPanel.ColumnCount;

            if (row == buttonsPanel.RowCount - 1)
            {
                buttonsPanel.RowStyles.Insert(buttonsPanel.RowCount - 1, new RowStyle());
                buttonsPanel.RowCount++;
                buttonsPanel.Controls.Remove(btnExam);
                buttonsPanel.Controls.Remove(btnReport);
                buttonsPanel.Controls.Remove(btnBack);
                buttonsPanel.Controls.Add(btnExam, 0, buttonsPanel.RowCount - 1);
                buttonsPanel.Controls.Add(btnReport, 1, buttonsPanel.RowCount - 1);
                buttonsPanel.Controls.Add(btnBack, 2, buttonsPanel.RowCount - 1);
            }

            buttonsPanel.Controls.Add(organButton, col, row);
            organs++;
        }
        
        private void OnExamInfoClick(object sender, EventArgs e)
        {
            if (ExamInfoClick != null)
            {
                ExamInfoClick(this, e);
            }
        }

        private void OnOrganClick(object sender, EventArgs e)
        {
            Button organButton = sender as Button;
            if (organButton == null) return;
            //retrieve the organ info tagged with the button
            SdeConcept organInfo = organButton.Tag as SdeConcept;
            if (organInfo == null) return;

            if (FindingsClick != null)
            {
                FindingsClick(this, new SdeConceptEventArgs(organInfo));
            }
        }

        private void doReport(object sender, EventArgs e)
        {
            if (ReportRequest != null)
            {
                CancelEventArgs args = new CancelEventArgs(false);
                //Raise event - the handlers of this event have the option of canceling the event
                ReportRequest(this, args);
                if (!args.Cancel)
                    Dispose();
            }
        }
    }

    public class SdeConceptEventArgs : EventArgs
    {
        public SdeConcept Concept { get; set; }
        public SdeConceptEventArgs(SdeConcept concept)
        {
            Concept = concept;
        }
    }
}