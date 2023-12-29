using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// A custom user control to display multiple instances of entities that
    /// have scalar attributes (i.e. all of its properties are single-valued)
    /// as grids - each row representing a single entity and each column
    /// representing a specific attribute of that entity. Supports the ability
    /// to edit the attributes of an entity through a separate pop-up dialog
    /// (called the "detailed editor").
    /// </summary>
    public class GridWidget : WidgetBase, IGridView
    {
        public event EventHandler<GridViewEventArgs> RowAddRequest;
        public event EventHandler<GridViewEventArgs> RowDeleteRequest;
        public event EventHandler<GridViewEventArgs> CellUpdated;
        public event EventHandler<GridViewEventArgs> WholeRowEditing;
        public event EventHandler<GridViewEventArgs> WholeRowUpdated;
        
        private IContainer components;

        private DataGridView grid;
        private GroupBox titleBorder;
        private Dictionary<string, ColumnAttribute> columnAttributes;
        private Dictionary<string, ElementWidgetBase> columnEditors;
        private bool detailedEditorEnabled;
        private RowMode rowMode = RowMode.AllowAddition | RowMode.AllowDeletion;
        private Dictionary<Guid, int> canonicalRowMap;
        private Guid replaceGuid;

        private RowEditor rowEditor;

        public GridWidget()
        {
            columnAttributes = new Dictionary<string, ColumnAttribute>();
            columnEditors = new Dictionary<string, ElementWidgetBase>();
            canonicalRowMap = new Dictionary<Guid, int>();
            InitializeComponent();
        }

        /// <summary>
        /// Add a column attribute to the entity represented in this view.
        /// </summary>
        public void AddAttribute(ColumnAttribute attr)
        {
            if (columnAttributes.ContainsKey(attr.Name))
                throw new ArgumentException("Attribute with name "+attr.Name+" already exists");

            columnAttributes[attr.Name] = attr;
            if (attr.ShowOnGrid)
            {
                RegisterColumn(attr);
            }
            if (attr.Editable)
            {
                RegisterEditorAttribute(attr);
            }
        }

        public void UpdateCell(Guid rowId, string columnName, object value)
        {
            int rowIndex = GetActualRowIndex(rowId);
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                throw new ArgumentException("Invalid row number "+rowIndex);
            if (!columnAttributes.ContainsKey(columnName))
                throw new ArgumentException("Unregistered column "+columnName);
            if (!columnAttributes[columnName].ShowOnGrid)
                throw new ArgumentException("Attribute " + columnName + " does not show on grid");

            DataGridViewRow row = grid.Rows[rowIndex];
            if (Equals(row.Cells[columnName].Value, value))
                return;

            row.Cells[columnName].Value = value;
            OnCellUpdated(new GridViewEventArgs(rowId, columnName, value));
        }

        public object GridValueAt(Guid rowId, string columnName)
        {
            int rowIndex = GetActualRowIndex(rowId);
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                throw new ArgumentException("Invalid row number " + rowIndex);
            if (!columnAttributes.ContainsKey(columnName))
                throw new ArgumentException("Unregistered column " + columnName);
            if (!columnAttributes[columnName].ShowOnGrid)
                throw new ArgumentException("Attribute " + columnName + " does not show on grid");

            object cellValue = grid.Rows[rowIndex].Cells[columnName].Value;
            return cellValue is DBNull ? null : cellValue;
        }

        public IScalarView EditorViewFor(string attributeName)
        {
            if (!columnAttributes.ContainsKey(attributeName))
                throw new ArgumentException("Unregistered attribute " + attributeName);
            ColumnAttribute attribute = columnAttributes[attributeName];
            if (!attribute.Editable)
                throw new ArgumentException("Attribute " + attributeName + " is not editable");

            return columnEditors[attributeName];
        }

        public Guid AddRow()
        {
            int newRowIndex = grid.Rows.Add();
            if (grid.Rows.Count > 1 && newRowIndex >= grid.Rows.Count - 1)
                newRowIndex --;
            Guid rowId = (Guid)grid.Rows[newRowIndex].Tag;
            OnRowAddRequest(new GridViewEventArgs(rowId, null, null));
            return rowId;
        }

        public void DeleteRow(Guid rowId)
        {
            int rowIndex = GetActualRowIndex(rowId);
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count)
                throw new ArgumentException("Invalid row number " + rowIndex);
            grid.Rows.RemoveAt(rowIndex);
            OnRowDeleteRequest(new GridViewEventArgs(rowId, null, null));
        }

        public int RowCount
        {
            get { return grid.Rows.Count; }
        }

        public ColumnAttribute[] Columns
        {
            get { return columnAttributes.Values.ToArray(); }
        }

        protected void RegisterColumn(ColumnAttribute attribute)
        {
            DataGridViewColumn column;
            if (InplaceEditable && attribute.Editable)
            {
                switch (attribute.GridCellType)
                {
                    case GridCellType.Check:
                        column = new DataGridViewCheckBoxColumn(true) {ValueType = typeof(bool)};
                        break;
                    case GridCellType.Choice:
                        column = new DataGridViewComboBoxColumn
                                     {
                                         DataSource = attribute.ValueRanges,
                                         ValueMember = "ID",
                                         DisplayMember = "Text",
                                         DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                                         ValueType = typeof(string)
                                     };
                        break;
                    case GridCellType.Date:
                        column = new DataGridViewTextBoxColumn { ValueType = typeof(DateTime?) };
                        column.DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
                        break;
                    case GridCellType.Int:
                        column = new DataGridViewTextBoxColumn { ValueType = typeof(long) };
                        break;
                    case GridCellType.Float:
                        column = new DataGridViewTextBoxColumn { ValueType = typeof(double) };
                        break;
                    default:
                        column = new DataGridViewTextBoxColumn { ValueType = typeof(string) };
                        break;
                }
            }
            else
            {
                column = new DataGridViewColumn(new DataGridViewTextBoxCell()) { ReadOnly = true };
            }
            column.Name = attribute.Name;
            column.HeaderText = attribute.Name;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.SortMode = DataGridViewColumnSortMode.Automatic;
            grid.Columns.Add(column);
        }

        protected void RegisterEditorAttribute(ColumnAttribute attribute)
        {
            ElementWidgetBase widget;
            switch (attribute.GridCellType)
            {
                case GridCellType.Check:
                    widget = new CheckboxWidget();
                    break;
                case GridCellType.Choice:
                    widget = new CodedTextWidget { ChoiceList = attribute.ValueRanges.ToList() };
                    break;
                case GridCellType.Date:
                    widget = new DateWidget();
                    break;
                case GridCellType.Int:
                case GridCellType.Float:
                    widget = new NumericWidget();
                    break;
                default:
                    widget = new TextWidget();
                    break;
            }
            widget.Title = attribute.Name;
            widget.DataValueProvider = attribute.DataValueProvider;

            rowEditor.Content.Children.Add(widget);
            columnEditors[attribute.Name] = widget;
        }

        public bool InplaceEditable
        {
            get { return !grid.ReadOnly; }
            set
            {
                grid.ReadOnly = !value;
                //TODO Modify column appearances
            }
        }

        public bool RowEditorEnabled
        {
            get { return detailedEditorEnabled; }
            set { detailedEditorEnabled = value; }
        }

        public RowMode RowMode
        {
            get { return rowMode; }
            set { rowMode = value; }
        }

        /// <summary>
        /// Get the list of attributes for the entity represented by this grid
        /// </summary>
        public ICollection<ColumnAttribute> Attributes
        {
            get { return columnAttributes.Values; }
        }

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

        protected void InitializeComponent()
        {
            grid = new DataGridView();
            titleBorder = new GroupBox();

            ((ISupportInitialize)grid).BeginInit();
            SuspendLayout();

            titleBorder.Dock = DockStyle.Fill;

            grid.AllowUserToAddRows = true;
            grid.AllowUserToDeleteRows = true;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            
            grid.Dock = DockStyle.Fill;
            grid.Name = "grid";
            grid.ReadOnly = false;
            grid.EditMode = DataGridViewEditMode.EditOnKeystroke;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            titleBorder.Controls.Add(grid);

            grid.RowsAdded += rowsAdded;
            grid.RowsRemoved += rowsRemoved;
            grid.CellDoubleClick += tryRowEdit;
            grid.CellValueChanged += propagateCellValueChangeEvent;
            grid.UserAddedRow += propagateUserRowAddEvent;
            grid.UserDeletingRow += interceptRowDeletion;
            grid.UserDeletedRow += propagateUserRowRemoveEvent;
            grid.CellBeginEdit += interceptCellEditing;
            grid.DataError += OnInvalidInput;

            grid.Sorted += delegate { UpdateRowIndexMap(); };

            rowEditor = new RowEditor();
            rowEditor.Size = new Size(350, 400);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(titleBorder);
            Name = "EntityGridWidget";
            Size = DefaultSize;

            ((ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }

        private void UpdateRowIndexMap()
        {
            canonicalRowMap.Clear();
            foreach (DataGridViewRow row in grid.Rows)
            {
                canonicalRowMap[(Guid) row.Tag] = row.Index;
            }
        }

        private void OnInvalidInput(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Woops! That's not a valid value for '" + grid.Columns[e.ColumnIndex].Name+"'");
        }

        /// <summary>
        /// Intercepts just as user attempts to edit a cell - if it's a non-editable
        /// attribute, then disable editing!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void interceptCellEditing(object sender, DataGridViewCellCancelEventArgs e)
        {
            string column = grid.Columns[e.ColumnIndex].Name;
            ColumnAttribute attribute = columnAttributes[column];
            if (!attribute.Editable)
                e.Cancel = true;
        }

        /// <summary>
        /// Intercepts just as user presses "delete" on a row. Only lets the event
        /// carry through as long as 1) an instance can be removed at this state,
        /// and 2) user confirms it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void interceptRowDeletion(object sender, DataGridViewRowCancelEventArgs e)
        {
            if ((RowMode & RowMode.AllowDeletion) != 0)
            {
                DialogResult result = MessageBox.Show(this, "Are you sure you want to delete this record?",
                                                      "Confirm delete",
                                                      MessageBoxButtons.YesNo, MessageBoxIcon.None,
                                                      MessageBoxDefaultButton.Button2);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
            else
            {
                MessageBox.Show(this, "Sorry, there is a minimum number of records that must be present",
                                "Can't delete any more records");
                e.Cancel = true;
            }
        }

        private void propagateCellValueChangeEvent(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            object cellValue = grid.Rows[row].Cells[col].Value;
            if (cellValue is DBNull) 
                cellValue = null;
            OnCellUpdated(new GridViewEventArgs(GetCanonicalRowId(row), grid.Columns[col].Name, cellValue));
        }

        private void tryRowEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!RowEditorEnabled) return;
            
            int editingRow = e.RowIndex;
            if (editingRow < 0) return;

            //Fire event - about to edit
            Guid rowId = GetCanonicalRowId(editingRow);
            if (WholeRowEditing != null)
            {
                WholeRowEditing(this, new GridViewEventArgs(rowId, null, null));
            }

            //bring up editor dialog; if commit, then fire row edit event
            if (rowEditor.ShowDialog() == DialogResult.OK)
            {
                if (editingRow >= RowCount - 1)
                {
                    //If user clicked on empty row, then create that row
                    rowId = AddRow();
                }

                if (WholeRowUpdated != null)
                {
                    WholeRowUpdated(this, new GridViewEventArgs(rowId, null, null));
                }
            }
        }

        private void rowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int actualRow = e.RowIndex;
            DataGridViewRow addedRow = grid.Rows[actualRow];
            Guid newRowId = Guid.NewGuid();
            addedRow.Tag = newRowId;
            UpdateRowIndexMap();
        }

        private void rowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            replaceGuid = Guid.Empty;
            if (grid.Rows.Count > 0 && e.RowIndex >= grid.Rows.Count)
            {
                //Switch the last two row id's
                Guid removedRowId = GetCanonicalRowId(e.RowIndex);
                replaceGuid = (Guid)grid.Rows[grid.RowCount - 1].Tag;
                grid.Rows[grid.RowCount - 1].Tag = removedRowId;
            }
            UpdateRowIndexMap();
        }

        private void propagateUserRowAddEvent(object sender, DataGridViewRowEventArgs e)
        {
            DataGridViewRow addedRow = e.Row;
            if (grid.Rows.Count > 1 && addedRow.Index >= grid.Rows.Count - 1)
                addedRow = grid.Rows[addedRow.Index - 1];
            OnRowAddRequest(new GridViewEventArgs((Guid)addedRow.Tag, null, null));
        }

        private void propagateUserRowRemoveEvent(object sender, DataGridViewRowEventArgs e)
        {
            DataGridViewRow removedRow = e.Row;
            OnRowDeleteRequest(new GridViewEventArgs(replaceGuid != Guid.Empty ? replaceGuid : (Guid)removedRow.Tag, null, null));
        }
        
        protected void OnRowDeleteRequest(GridViewEventArgs args)
        {
            if (RowDeleteRequest != null)
            {
                RowDeleteRequest(this, args);
            }
        }

        private void OnRowAddRequest(GridViewEventArgs args)
        {
            if (RowAddRequest != null)
            {
                RowAddRequest(this, args);
            }
        }
        
        protected void OnCellUpdated(GridViewEventArgs args)
        {
            if (CellUpdated != null)
            {
                CellUpdated(this, args);
            }
        }

        /// <summary>
        /// Translates the given row index to that of the row in the original unsorted grid.
        /// </summary>
        /// <param name="canonicalIndex"></param>
        /// <returns></returns>
        private int GetActualRowIndex(Guid canonicalIndex)
        {
            return canonicalRowMap[canonicalIndex];
        }

        private Guid GetCanonicalRowId(int actualIndex)
        {
            return canonicalRowMap.Keys.FirstOrDefault(i => canonicalRowMap[i] == actualIndex);
        }
        
        public override string Title
        {
            get { return titleBorder.Text; }
            set
            {
                if (string.Equals(titleBorder.Text, value))
                    return;
                titleBorder.Text = value;
                OnTitleChanged(EventArgs.Empty);
            }
        }

        public override Size IdealSize
        {
            get
            {
                //TODO temporary heuristics
                int width = grid.RowHeadersWidth + 10;
                foreach (DataGridViewColumn col in grid.Columns)
                {
                    width += col.Width;
                }

                int rowHeight = grid.Rows.Count > 0 ? grid.Rows[0].Height : 15;
                int height = grid.ColumnHeadersHeight + rowHeight * (grid.Rows.Count + 1);

                return new Size(Math.Min(width, 500), Math.Max(height, 250));
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(244, 170);
            }
        }

        public override void Reset()
        {
            grid.Rows.Clear();
        }
        
        public class RowEditor : Form
        {
            private SimpleContainerWidget content;
            
            public RowEditor()
            {
                TableLayoutPanel basePanel = new TableLayoutPanel { RowCount = 2, ColumnCount = 3 };
                basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); //horizontal "glue"
                basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                basePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                basePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

                content = new SimpleContainerWidget();
                content.Dock = DockStyle.Fill;
                basePanel.Controls.Add(content, 0, 0);
                basePanel.SetColumnSpan(content, 3);

                Button ok = new Button { Text = "OK" };
                Button cancel = new Button { Text = "Cancel" };
                ok.Click += delegate
                {
                    DialogResult = DialogResult.OK;
                    Close();
                };
                cancel.Click += delegate
                                    {
                                        DialogResult = DialogResult.Cancel;
                                        Close();
                                    };

                basePanel.Controls.Add(ok, 1, 1);
                basePanel.Controls.Add(cancel, 2, 1);

                basePanel.Dock = DockStyle.Fill;
                Controls.Add(basePanel);
            }

            public SimpleContainerWidget Content { get { return content; } }
        }
    }
}
