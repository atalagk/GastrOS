using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views;
using GastrOs.Sde.Views.WinForms;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.RM.Composition.Content.Navigation;

namespace GastrOs.Sde.Test.GuiTests
{
    [TestFixture]
    public class GridViewTest : ModelViewTestBase
    {
        [Test]
        public void GridEventsTest()
        {
            DataTable dt = new DataTable("haha");
            dt.Columns.Add(new DataColumn("aaa"));
            dt.Columns.Add(new DataColumn("bbb"));

            DataGridView grid = new DataGridView();

            grid.DataSource = dt;

            grid.AllowUserToAddRows = true;
            grid.AllowUserToDeleteRows = true;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            grid.Name = "grid";
            grid.ReadOnly = false;
            grid.EditMode = DataGridViewEditMode.EditOnKeystroke;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

            grid.RowsAdded += rowAdded;
            grid.RowsRemoved += rowRemoved;
            grid.CellValueChanged += propagateCellValueChangeEvent;
            grid.UserAddedRow += userAdded;
            grid.UserDeletingRow += userDeleting;

            grid.Size = new Size(400, 300);
            Form f = new Form();
            f.Controls.Add(grid);
            f.Size = new Size(500, 400);

            /*grid.Columns.Add(new DataGridViewTextBoxColumn
                                 {
                                     Name = "A0",
                                     HeaderText = "A0",
                                     AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                                     SortMode = DataGridViewColumnSortMode.Automatic,
                                     ValueType = typeof (string)
                                 });
            grid.Columns.Add(new DataGridViewTextBoxColumn
                                 {
                                     Name = "B1",
                                     HeaderText = "B1",
                                     AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                                     SortMode = DataGridViewColumnSortMode.Automatic,
                                     ValueType = typeof (string)
                                 });*/

            dt.TableNewRow += new DataTableNewRowEventHandler(dt_TableNewRow);
            dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);
            dt.ColumnChanging += new DataColumnChangeEventHandler(dt_ColumnChanging);
            dt.RowChanged += new DataRowChangeEventHandler(dt_RowChanged);
            dt.RowChanging += new DataRowChangeEventHandler(dt_RowChanging);
            dt.RowDeleting += new DataRowChangeEventHandler(dt_RowDeleting);
            dt.RowDeleted += new DataRowChangeEventHandler(dt_RowDeleted);

            f.ShowDialog();
        }

        void dt_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            
        }

        void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            
        }

        void dt_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            
        }

        void dt_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            
        }

        void dt_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            
        }

        void dt_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            
        }

        void dt_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            
        }

        private void userDeleting(object sender, DataGridViewRowCancelEventArgs e)
        {
            
        }

        private void userAdded(object sender, DataGridViewRowEventArgs e)
        {
            
        }

        private void propagateCellValueChangeEvent(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void rowRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            
        }

        private void rowAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            
        }

        [Test]
        public void Test1()
        {
            GridWidget rw = new GridWidget();
            rw.Title = "gasfdfd";
            rw.AddAttribute(new ColumnAttribute("col1", GridCellType.Text, true, true, new DvTextProvider()));
            rw.AddAttribute(new ColumnAttribute("col2", GridCellType.Text, true, true, new DvTextProvider()));
            rw.AddAttribute(new ColumnAttribute("col3", GridCellType.Text, false, true, new DvTextProvider()));
            rw.InplaceEditable = true;
            rw.CanAddNewInstance = true;
            rw.CanRemoveInstance = true;

            rw.CellUpdated += UpdateModelWithCellValue;
            rw.RowAddRequest += AddItem;
            rw.RowDeleteRequest += RemoveItem;

            Form f = new Form() {Width = 300, Height = 300};
            f.Load += delegate { rw.Size = rw.IdealSize; };
            f.Controls.Add(rw);
            f.ShowDialog();
            f.Dispose();
        }

        private void RemoveItem(object sender, GridViewEventArgs e)
        {
            
        }

        private void AddItem(object sender, GridViewEventArgs e)
        {
            
        }

        private void UpdateModelWithCellValue(object sender, GridViewEventArgs e)
        {
            
        }

        [Test]
        public void Test2()
        {
            OperationalTemplate opt = EhrSerialiser.Load<OperationalTemplate>("TestTemplate1.opt");
            CComplexObject constraint = opt.Definition.ExtractChildConstraints("content").First() as CComplexObject;
            Section newInstance = RmFactory.InstantiateSection(constraint);

            List<AttributeDescriptor> attributes = new List<AttributeDescriptor>
                                   {
                                       new ColumnDescriptor("Description", true, null, "items[openEHR-EHR-EVALUATION.adverse.v1]/data[at0002]/items[at0003]"),
                                       new ColumnDescriptor("Cause", true, null, "items[openEHR-EHR-EVALUATION.adverse.v1]/data[at0002]/items[at0019]/items[at0022]"),
                                       new ColumnDescriptor("Date/time", true, null, "items[openEHR-EHR-EVALUATION.adverse.v1]/data[at0002]/items[at0019]/items[at0027]")
                                   };
            ViewControl control = new EvaluationGridControl(constraint, attributes);
            control.Model = newInstance;
            control.View = container.Resolve<IGridView>();
            
            Form f = new Form() { Width = 600, Height = 600 };
            f.Load += delegate { control.View.Size = control.View.IdealSize; };
            f.Controls.Add(control.View as Control);
            f.ShowDialog();
            f.Dispose();
        }

        [Test]
        public void Test3()
        {
            OperationalTemplate opt = EhrSerialiser.Load<OperationalTemplate>("TestTemplate2.opt");
            CComplexObject constraint = opt.Definition.ExtractChildConstraints("content").First() as CComplexObject;
            Section newInstance = RmFactory.InstantiateSection(constraint);

            List<AttributeDescriptor> attributes = new List<AttributeDescriptor>
                                                       {
                                                           new CompositeDescriptor("BP", null,
                                                               new[]
                                                                   {
                                                                       "items[openEHR-EHR-OBSERVATION.blood_pressure.v1]/data[at0001]/events[at0006]/data[at0003]/items[at0004]",
                                                                       "items[openEHR-EHR-OBSERVATION.blood_pressure.v1]/data[at0001]/events[at0006]/data[at0003]/items[at0005]"
                                                                   }, "/"),
                                                           new ColumnDescriptor("Systolic", false, null, "items[openEHR-EHR-OBSERVATION.blood_pressure.v1]/data[at0001]/events[at0006]/data[at0003]/items[at0004]"),
                                                           new ColumnDescriptor("Diastolic", false, null, "items[openEHR-EHR-OBSERVATION.blood_pressure.v1]/data[at0001]/events[at0006]/data[at0003]/items[at0005]"),
                                                           new ColumnDescriptor("Position", false, null, "items[openEHR-EHR-OBSERVATION.blood_pressure.v1]/data[at0001]/events[at0006]/state[at0007]/items[at0008]"),
                                                           new ColumnDescriptor("Cuff size", false, null, "items[openEHR-EHR-OBSERVATION.blood_pressure.v1]/protocol[at0011]/items[at0013]"),
                                                           new ColumnDescriptor("Pulse rate", true, null, "items[openEHR-EHR-OBSERVATION.heart_rate.v1]/data[at0002]/events[at0003]/data[at0001]/items[at0004]"),
                                                           new ColumnDescriptor("Description", true, null, "items[openEHR-EHR-OBSERVATION.vital_signs_generic.v1]/data[at0001]/events[at0002]/data[at0003]/items[at0004]")
                                                       };

            ViewControl control = new ObservationsGridControl(constraint, attributes);
            control.Model = newInstance;
            control.View = container.Resolve<IGridView>();

            Form f = new Form() { Width = 600, Height = 600 };
            f.Load += delegate { control.View.Size = control.View.IdealSize; };
            f.Controls.Add(control.View as Control);
            f.ShowDialog();
            f.Dispose();
        }
    }
}
