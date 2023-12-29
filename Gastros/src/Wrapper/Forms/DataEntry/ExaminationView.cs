using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GastrOs.Sde;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views.WinForms.Generic;
using GastrOs.Sde.Views.WinForms.Support;
using GastrOs.Wrapper.DataObjects;
using GastrOs.Wrapper.Helpers;
using GastrOs.Wrapper.Reports;
using GastrOs.Wrapper.Reports.Representation;
using NHibernate;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;

namespace GastrOs.Wrapper.Forms.DataEntry
{
    public partial class ExaminationView : UserControl
    {
        private ISession session;
        private Examination examination;
        private IList<Department> availableDepts;
        private IList<EndoscopyDevice> availableDevices;
        private IList<EndoscopyType> endoTypes;
        private IList<SdeConcept> sdeConcepts;
        private IReportFormatter reportFormatter;

        private bool editable = true;

        private ExaminationView() {}
        
        public ExaminationView(Examination examination, ISession session, IReportFormatter reportFormatter)
        {
            this.examination = examination;
            this.session = session;
            this.reportFormatter = reportFormatter;

            endoTypes = PopulateListFromDb<EndoscopyType>(session);

            if (endoTypes.Count == 0)
            {
                throw new InvalidOperationException("There must be at least one endoscopy type registered in the database.");
            }

            availableDepts = PopulateListFromDb<Department>(session);
            availableDevices = PopulateListFromDb<EndoscopyDevice>(session);
            sdeConcepts = PopulateListFromDb<SdeConcept>(session);

            InitializeComponent();

            endoDateChooser.MaxDate = DateTime.Now;

            deptChooser.DataSource = availableDepts;
            deptChooser.DisplayMember = "Name";

            deviceChooser.DataSource = availableDevices;
            deviceChooser.DisplayMember = "Name";

            typeChooser.DataSource = endoTypes;
            typeChooser.DisplayMember = "Name";
            Layout += delegate
                          {
                              Size minSize = basePanel.PreferredSize;
                              AutoScrollMinSize = new Size(minSize.Width + basePanel.Margin.Horizontal,
                                                           minSize.Height + basePanel.Margin.Vertical);
                              typeChooser.SelectedItem = examination.EndoscopyType;
                          };
        }

        private static IList<T> PopulateListFromDb<T>(ISession session)
        {
            return session.CreateQuery("from " + typeof(T).Name).List<T>();
        }

        private void ExaminationView_Load(object sender, EventArgs e)
        {
            examinationBindingSource.Add(examination);
            examinationBindingSource.MoveFirst();

            if (!examination.ReportDate.HasValue)
                reportDateChooser.Checked = false; 
            else
                reportDateChooser.Checked=true;
            
            reportDateChooser.MaxDate = DateTime.Now;

            // We don't want null endoscopy date
            if (examination.EndoscopyDate.Equals(new DateTime()))
                examination.EndoscopyDate = DateTime.Now;
        }

        public EndoscopyType SelectedEndoscopyType
        {
            get
            {
                EndoscopyType type = typeChooser.SelectedItem as EndoscopyType;
                if (type == null)
                    return new EndoscopyType {Name = "(unselected examination type)"};
                return type;
            }
        }

        public void SetEditable (Boolean editStatus)
        {
            editable = editStatus;
            examDetailsBox.SetEditable(editable);
            reportBox.SetEditable(editable);
        }

        private LockEditForm InvokeLockEditForm(string conceptKey, bool formEditable, ButtonsConfig buttonsConfig)
        {
            if (SelectedEndoscopyType == null || string.IsNullOrEmpty(SelectedEndoscopyType.OpTemplate))
            {
                MessageBox.Show(this, "You must select an examination type.");
                return null;
            }
            /*if (string.IsNullOrEmpty(SelectedEndoscopyType.OpTemplate))
            {
                MessageBox.Show(this, "Can't find operational template associated with " + SelectedEndoscopyType.Name);
                return null;
            }*/

            SdeConcept concept = RetrieveConcept(conceptKey);
            if (concept == null)
            {
                MessageBox.Show(this, "Can't locate archetype for a concept named '" + conceptKey +
                                      "' from operational template " +
                                      SelectedEndoscopyType.OpTemplate +
                                      ". Please make sure it is defined in the database");
                return null;
            }

            //Two possibilities: 1) singular concept, in which case generate a single form,
            //or 2) concept has children, in which case generate an amalgamated form with tabs
            if (concept.Children != null && !concept.Children.IsEmpty)
            {
                return InvokeLockEditForm(SelectedEndoscopyType.OpTemplate, concept, concept.Children, formEditable,
                                          buttonsConfig);
            }
            return InvokeLockEditForm(SelectedEndoscopyType.OpTemplate, concept, formEditable,
                                      buttonsConfig);
        }

        private LockEditForm InvokeLockEditForm(string opTemplate, SdeConcept concept, bool formEditable,
            ButtonsConfig buttonsConfig)
        {
            ViewControl widget = SdeUtils.TryGenerateView(examination, opTemplate, concept);
            if (widget != null)
            {
                LockEditForm form = FormInvoker.InvokeLockEditForm(widget, formEditable, buttonsConfig);
                Dictionary<SdeConcept, ViewControl> conceptToWidgetMap = new Dictionary<SdeConcept, ViewControl>();
                conceptToWidgetMap[concept] = widget;
                //Specially "tag" this widget with the information necessary for later saving, reporting and deleting
                form.Tag = conceptToWidgetMap;

                form.Save += SaveAndPersist;
                form.Delete += PromptDelete;
                form.Report += PromptAndReport;
                form.Closing += PromptSaveOnClosing;

                return form;
            }
            return null;
        }

        private LockEditForm InvokeLockEditForm(string opTemplate, SdeConcept parentConcept,
            IEnumerable<SdeConcept> childConcepts, bool formEditable, ButtonsConfig buttonsConfig)
        {
            Dictionary<SdeConcept, ViewControl> conceptToWidgetMap = new Dictionary<SdeConcept, ViewControl>();
            
            AutoTabPanel contentPanel = new AutoTabPanel();
            int i = 0;
            foreach (SdeConcept child in childConcepts)
            {
                //TODO hard-wired to modify instance generation for these specific archetypes
                RmFactory.RecursivelyGenerateSections = !child.ArchetypeId.Equals("openEHR-EHR-SECTION.adverse_reactions.v1");
                RmFactory.RecursivelyGenerateHistory = !child.ArchetypeId.Equals("openEHR-EHR-SECTION.vital_signs.v1");
                
                ViewControl childWidget = SdeUtils.TryGenerateView(examination, opTemplate, child);

                RmFactory.RecursivelyGenerateSections = true;
                RmFactory.RecursivelyGenerateHistory = true;

                if (childWidget == null)
                    return null;
                CArchetypeRoot childArchetype = GastrOsService.OperationalTemplate.Definition.LocateArchetypeById(child.ArchetypeId);
                BreakDirective breakDirective = GastrOsService.OperationalTemplate.FindDirectiveOfType<BreakDirective>(childArchetype);
                BreakStyle breakStyle = breakDirective != null ? breakDirective.BreakStyle : BreakStyle.None;
                contentPanel.AddChild(childWidget.View as Control, breakStyle);
                if (breakStyle == BreakStyle.Tab)
                    contentPanel.SetTabTitle(++i, child.Term);
                //TODO find out why doing this before the above add screws size up
                childWidget.View.Size = childWidget.View.IdealSize;
                conceptToWidgetMap[child] = childWidget;
            }

            LockEditForm form = FormInvoker.InvokeLockEditForm(contentPanel, formEditable, parentConcept.Term, false,
                                                               buttonsConfig);
            //Specially "tag" this widget with the information necessary for later saving, reporting and deleting
            form.Tag = conceptToWidgetMap;

            form.Save += SaveAndPersist;
            form.Delete += PromptDelete;
            form.Report += PromptAndReport;
            form.Closing += PromptSaveOnClosing;

            //TODO CR really a temporary workaround to manually getting directives in order to set form aspects
            if (parentConcept.ArchetypeId != null)
            {
                CArchetypeRoot examRoot = GastrOsService.OperationalTemplate.Definition.LocateArchetypeById(parentConcept.ArchetypeId);
                if (examRoot != null)
                {
                    FormAspectsDirective formAspects =
                        GastrOsService.OperationalTemplate.FindDirectiveOfType<FormAspectsDirective>(examRoot);
                    if (formAspects != null)
                        form.Size = new Size(formAspects.Width, formAspects.Height);
                } //end of temporary workaround
            }

            return form;
        }

        private void ShowInterventionsForm(object sender, EventArgs e)
        {
            LockEditForm form = InvokeLockEditForm("Interventions", editable,
                                                   ButtonsConfig.Report | ButtonsConfig.EditSave | ButtonsConfig.Delete);
            if (form != null)
            {
                form.ShowDialog(this);
                form.Dispose();
            }
        }

        private void ShowDiagnosesForm(object sender, EventArgs e)
        {
            LockEditForm form = InvokeLockEditForm("Diagnoses", editable, ButtonsConfig.None);
            if (form != null)
            {
                form.Save += delegate { PromptAndReport(form, new CancelEventArgs()); };
                form.ShowDialog(this);
                form.Dispose();
            }
        }

        private void ShowIntermediateForm(object sender, EventArgs e)
        {
            if (SelectedEndoscopyType == null || string.IsNullOrEmpty(SelectedEndoscopyType.OpTemplate))
            {
                MessageBox.Show(this, "You must select an examination type");
                return;
            }

            IntermediateForm imf = new IntermediateForm();
            foreach (SdeConcept organ in SelectedEndoscopyType.OrgansSortedByOrdinal)
            {
                imf.AddOrganButton(organ.Term, organ);
            }
            imf.ReportRequest += PromptAndReport;
            imf.ExamInfoClick += ShowExamInfoForm;
            imf.FindingsClick += ShowFindingsForm;
            imf.ShowDialog(ParentForm);
            imf.Dispose();
        }

        private void ShowFindingsForm(object sender, SdeConceptEventArgs e)
        {
            LockEditForm form = InvokeLockEditForm(e.Concept.Term, true, ButtonsConfig.EditSave | ButtonsConfig.Delete);
            if (form != null)
            {
                form.ShowDialog(sender as Control);
                form.Dispose();
            }
        }

        private void ShowExamInfoForm(object sender, EventArgs e)
        {
            LockEditForm form = InvokeLockEditForm("Examination", true, ButtonsConfig.EditSave | ButtonsConfig.Delete);
            if (form != null)
            {
                form.ShowDialog(sender as Control);
                form.Dispose();
            }
        }

        /// <summary>
        /// Save the serialised value instances into the examination object, and persist it to DB.
        /// </summary>
        private void SaveAndPersist(object sender, EventArgs e)
        {
            Control sourceForm = sender as Control;
            if (sourceForm == null)
                return;
            //This is a special tag that maps each concept to associated widget
            Dictionary<SdeConcept, ViewControl> tag = sourceForm.Tag as Dictionary<SdeConcept, ViewControl>;
            if (tag == null)
                return;
            foreach (SdeConcept concept in tag.Keys)
            {
                ViewControl associatedWidget = tag[concept];
                examination.SetSerialisedValue(concept,
                                               EhrSerialiser.PruneAndSave(associatedWidget.Model,
                                                                          associatedWidget.Constraint));
                PersistHelper.Save(this, examination, session);
            }
        }

        /// <summary>
        /// Invoked when a form is about to be closed manually - prompt users to save
        /// </summary>
        private void PromptSaveOnClosing(object sender, CancelEventArgs e)
        {
            Control sourceForm = sender as Control;
            if (sourceForm == null)
                return;
            //This is a special tag that maps each concept to associated widget
            Dictionary<SdeConcept, ViewControl> tag = sourceForm.Tag as Dictionary<SdeConcept, ViewControl>;
            if (tag == null)
                return;

            bool dirty = false;

            foreach (SdeConcept concept in tag.Keys)
            {
                ViewControl associatedWidget = tag[concept];
                Locatable model = associatedWidget.Model;
                CComplexObject constraint = associatedWidget.Constraint;
                //A rather cheeky method of comparing value instances - by serialised strings!
                //TODO should ideally be able to compare on the object instance level instead.
                string currentInstance = EhrSerialiser.PruneAndSave(model, constraint) ?? "";
                string originalInstance = examination.GetSerialisedValue(concept) ?? "";

                if (!string.Equals(currentInstance, originalInstance))
                {
                    dirty = true;
                    break;
                }
            }

            //If "dirty" (i.e. current instance is different from saved value) then prompt saving
            if (dirty)
            {
                DialogResult result = MessageBox.Show(sourceForm, "Do you want to save before closing?", "Save?",
                                                      MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information,
                                                      MessageBoxDefaultButton.Button3);
                switch (result)
                {
                    case DialogResult.Yes:
                        //if "Yes", then save
                        SaveAndPersist(sourceForm, EventArgs.Empty);
                        return;
                    case DialogResult.Cancel:
                        //if "Cancel", then tell the form to cancel its close operation
                        e.Cancel = true;
                        return;
                }
            }
        }

        private void PromptDelete(object sender, CancelEventArgs eventArgs)
        {
            Control sourceForm = sender as Control;
            if (sourceForm == null)
                return;
            //This is a special tag that maps each concept to associated widget
            Dictionary<SdeConcept, ViewControl> tag = sourceForm.Tag as Dictionary<SdeConcept, ViewControl>;
            if (tag == null)
                return;

            DialogResult result = MessageBox.Show(sourceForm,
                                                  GuiDictionary.Lookup("deletePrompt"), "Delete?", MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                PersistHelper.Delete(examination, tag.Keys.ToArray());
            }
            else
            {
                //This will prevent the form from closing
                eventArgs.Cancel = true;
            }
        }

        private void PromptAndReport(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(examination.ReportText))
            {
                DialogResult result = MessageBox.Show(sender as Control, "Do you want to overwrite the existing report?",
                                                      "Confirm overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    //this will tell the component that raised this event to cancel what it
                    //was about to do (e.g. disposing)
                    e.Cancel = true;
                    return;
                }
            }
            
            GastrOsService.OperationalTemplateName = SelectedEndoscopyType.OpTemplate;
            OperationalTemplate opt = GastrOsService.OperationalTemplate;
            
            //1. Exam
            SdeConcept examConcept = RetrieveConcept("Exam Characteristics");
            if (examConcept != null)
            {
                examination.ExamInfoText = GenerateReportFromValue<ExamInfo>(examConcept, opt,
                                                                             ReportExtractor.ExtractExamInfo,
                                                                             reportFormatter.FormatExamInfo);
            }

            //2. Findings
            List<string> findingsTexts = new List<string>();
            foreach (SdeConcept organ in SelectedEndoscopyType.OrgansSortedByOrdinal)
            {
                //Treat "composite" organs specially (i.e. ERCP) - ignore composite organ and report child organs
                if (organ.Children != null && !organ.Children.IsEmpty)
                {
                    foreach (SdeConcept childOrgan in organ.Children)
                    {
                        findingsTexts.Add(GenerateReportFromValue<Findings>(childOrgan, opt,
                            ReportExtractor.ExtractFindingsOrInterventions, reportFormatter.FormatFindings));
                    }
                }
                else
                {
                    findingsTexts.Add(GenerateReportFromValue<Findings>(organ, opt,
                        ReportExtractor.ExtractFindingsOrInterventions, reportFormatter.FormatFindings));
                }
            }
            examination.FindingsText = findingsTexts.ToPrettyString("\r\n", false, true);

            //3. Interventions
            SdeConcept interventionsConcept =
                sdeConcepts.FirstOrDefault(c => string.Equals(c.Term, "Interventions"));
            if (interventionsConcept != null)
            {
                examination.InterventionsText = GenerateReportFromValue<Findings>(interventionsConcept, opt,
                    ReportExtractor.ExtractFindingsOrInterventions, reportFormatter.FormatInterventions);
            }

            //4. Diagnoses
            SdeConcept diagConcept = sdeConcepts.FirstOrDefault(c => string.Equals(c.Term, "Diagnoses"));
            if (diagConcept != null)
            {
                examination.DiagnosesText = GenerateReportFromValue<Diagnoses>(diagConcept, opt,
                    ReportExtractor.ExtractDiagnoses, reportFormatter.FormatDiagnoses);
            }

            examination.ReportText = examination.ExamInfoText + "\r\n" + examination.FindingsText + "\r\n" +
                                     examination.InterventionsText;
            txtReport.Text = examination.ReportText;
        }

        private string GenerateReportFromValue<T>(SdeConcept term, OperationalTemplate opt,
            Func<CArchetypeRoot, Cluster, SdeConcept, T> reportFunc, 
            Func<T, string> formatFunc)
            where T : class
        {
            string serialised = examination.GetSerialisedValue(term);
            //try loading exams value instance and generating report text
            if (!string.IsNullOrEmpty(serialised))
            {
                Cluster value = new Cluster();
                try
                {
                    EhrSerialiser.LoadFromXmlString(value, serialised);
                    CArchetypeRoot constraint = opt.Definition.LocateArchetypeById(term.ArchetypeId);
                    if (constraint == null)
                    {
                        MessageBox.Show(this, "Can't locate archetype with id '" + term.ArchetypeId +
                                              "' from operational template " + SelectedEndoscopyType.OpTemplate + 
                                              ". Most likely a mistake in the database mapping.");
                        return null;
                    }
                    T reportObject = reportFunc(constraint, value, term);
                    if (reportObject != null)
                        return formatFunc(reportObject);
                } catch (Exception ex)
                {
                    Logger.Error("Error while loading value instance for report generation.", ex);
                }
            }
            return null;
        }

        private SdeConcept RetrieveConcept(string conceptKey)
        {
            return sdeConcepts.FirstOrDefault(c => string.Equals(c.Term, conceptKey));
        }
    }
}