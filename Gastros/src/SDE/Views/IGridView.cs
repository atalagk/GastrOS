using System;
using System.Collections.Generic;
using System.ComponentModel;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;

namespace GastrOs.Sde.Views
{
    public class GridViewEventArgs : CancelEventArgs
    {
        public Guid Row { get; private set; }
        public string Column { get; private set; }
        public object Value { get; private set; }
        public GridViewEventArgs(Guid row, string col, object value)
        {
            Row = row;
            Column = col;
            Value = value;
        }
    }

    /// <summary>
    /// Displays multiple instances of entities that have scalar attributes
    /// (i.e. all of its properties are single-valued)
    /// as grids - each row representing a single entity and each column
    /// representing a specific attribute of that entity.
    /// 
    /// Note that the index numbers of rows are with respect to the original 
    /// ordering of the grid. i.e. if, for example,
    /// a row that belongs to index 1 of the unsorted grid moves to index 3 after
    /// sorting, then giving row index 1 (instead of 3) will delete that row.
    /// </summary>
    public interface IGridView : IView
    {
        /// <summary>
        /// Triggered when user wants to add a new row
        /// </summary>
        event EventHandler<GridViewEventArgs> RowAddRequest;
        /// <summary>
        /// Triggered when user wants to delete a specific row
        /// </summary>
        event EventHandler<GridViewEventArgs> RowDeleteRequest;
        /// <summary>
        /// Triggered when a user directly modified a value in a specific
        /// cell (only applies when <see cref="InplaceEditable"/> is true).
        /// </summary>
        event EventHandler<GridViewEventArgs> CellUpdated;
        /// <summary>
        /// Triggered when the user has opened the row editor
        /// </summary>
        event EventHandler<GridViewEventArgs> WholeRowEditing;
        /// <summary>
        /// Triggered when the user modified the contents of the row in bulk
        /// via the row editor
        /// </summary>
        event EventHandler<GridViewEventArgs> WholeRowUpdated;

        /// <summary>
        /// Add a column attribute to the entity represented in this view.
        /// </summary>
        void AddAttribute(ColumnAttribute attr);
        
        /// <summary>
        /// Update given cell with given values.
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        void UpdateCell(Guid rowId, string columnName, object value);
        
        /// <summary>
        /// Add a new row.
        /// </summary>
        Guid AddRow();
        
        /// <summary>
        /// Delete given row from the grid.
        /// </summary>
        /// <param name="rowId">The index of the row to delete</param>
        void DeleteRow(Guid rowId);

        int RowCount { get; }

        ColumnAttribute[] Columns { get; }

        object GridValueAt(Guid rowId, string columnName);

        IScalarView EditorViewFor(string columnName);

        /// <summary>
        /// Whether or not the cells are directly editable
        /// </summary>
        bool InplaceEditable { get; set; }

        /// <summary>
        /// Whether or not the grid should be sensitive to row
        /// edit requests - in case of enabling detailed editor for record.
        /// </summary>
        bool RowEditorEnabled { get; set; }

        /// <summary>
        /// What kinds of row operations are currently supported (allow add and/or delete?)
        /// </summary>
        RowMode RowMode { get; set; }
    }

    [Flags]
    public enum RowMode
    {
        None = 0x00, AllowAddition = 0x01, AllowDeletion = 0x10
    }

    public class ColumnAttribute
    {
        private string name;
        private GridCellType gridCellType;
        private bool editable;
        private bool showOnGrid;
        private IEnumerable<OntologyItem> valueRanges;
        private IDataValueProvider dataValueProvider;

        /// <summary>
        /// 
        /// </summary>
        /// /// <param name="name">name of the attrbiute</param>
        /// <param name="gridCellType">data type of the attribute</param>
        /// <param name="editable">whether this attribute is directly editable on the grid.
        /// <param name="showOnGrid"></param>
        /// <param name="dataValueProvider"></param>
        /// If it is, then the cell will be rendered with an appropriate editor control</param>
        public ColumnAttribute(string name, GridCellType gridCellType, bool editable, bool showOnGrid, IDataValueProvider dataValueProvider) 
            : this(name, gridCellType, editable, showOnGrid, dataValueProvider, null)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// /// <param name="name">name of the attrbiute</param>
        /// <param name="gridCellType">data type of the attribute</param>
        /// <param name="editable">whether this attribute is directly editable on the grid.
        /// If it is, then the cell will be rendered with an appropriate editor control</param>
        /// <param name="showOnGrid"></param>
        /// <param name="dataValueProvider"></param>
        /// <param name="valueRanges">Only applies for data type GridCellType.Choice</param>
        public ColumnAttribute(string name, GridCellType gridCellType, bool editable, bool showOnGrid, 
            IDataValueProvider dataValueProvider, IEnumerable<OntologyItem> valueRanges)
        {
            this.name = name;
            this.gridCellType = gridCellType;
            this.editable = editable;
            this.showOnGrid = showOnGrid;
            this.dataValueProvider = dataValueProvider;
            this.valueRanges = valueRanges;
        }

        public string Name { get { return name; } }
        public GridCellType GridCellType { get { return gridCellType; } }
        public bool Editable {get { return editable; }}
        public bool ShowOnGrid { get { return showOnGrid; } }
        public IEnumerable<OntologyItem> ValueRanges { get { return valueRanges; } }
        public IDataValueProvider DataValueProvider { get { return dataValueProvider; } }
        
        public override string ToString()
        {
            return name;
        }
    }

    public enum GridCellType
    {
        Text, Int, Float, Date, Choice, Check
    }
}