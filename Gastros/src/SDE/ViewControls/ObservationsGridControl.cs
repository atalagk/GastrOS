using System;
using System.Collections.Generic;
using System.Linq;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.AssumedTypes;
using OpenEhr.DesignByContract;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.Composition.Content.Entry;
using OpenEhr.RM.Composition.Content.Navigation;
using OpenEhr.RM.DataStructures.History;
using OpenEhr.RM.DataStructures.ItemStructure;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Quantity.DateTime;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.ViewControls
{
    /// <summary>
    /// A rather special-purpose viewcontrol for representing a section of multiple observations
    /// as a grid, synchronised by event times. The following restrictions apply:
    /// 1) section must contain one or more types of Observation entries, each of which can ONLY occur ONCE
    /// 2) each Event in an Observation entry can contain a hierarchy of Items, BUT each item may only occur at most once
    /// 3) the children to display as columns MUST be of ELEMENT RM type
    /// </summary>
    public class ObservationsGridControl : GridControlBase<Section>
    {
        private const string DateTimeColumn = "Date/Time";
        
        private Dictionary<CComplexObject, Observation> observationMap = new Dictionary<CComplexObject, Observation>();
        //Keeps track of which observation maps to which row of the grid
        private Dictionary<Event<ItemStructure>, Guid> eventRowMap = new Dictionary<Event<ItemStructure>, Guid>();

        public ObservationsGridControl(CComplexObject constraint, IEnumerable<AttributeDescriptor> attributeDescriptors)
            : base(constraint, attributeDescriptors)
        {
            //Check the restrictions - for now just check the minimum required to work...
            ICollection<CObject> obsConstraints = constraint.ExtractChildConstraints("items");
            Check.Require(obsConstraints.Count > 0);
            foreach (CComplexObject obsConstraint in obsConstraints)
            {
                Check.Require(obsConstraint.RmTypeMatches<Observation>());
                observationMap[obsConstraint] = null;
                GetEventConstraint(obsConstraint); //call for invariant checking
            }
        }

        protected override void SetViewPostexecute(IGridView oldView)
        {
            View.InplaceEditable = false;
            View.RowEditorEnabled = true;

            //Add date/time column for sync'd event times
            View.AddAttribute(new ColumnAttribute(DateTimeColumn, GridCellType.Date, true, true, new DvDateTimeProvider()));
            
            base.SetViewPostexecute(oldView);
        }

        protected override void SetModelPostexecute(Section oldModel)
        {
            //Must have one Observation instance per constraint
            Check.Require(Model.Items.Count == observationMap.Count);
            System.Collections.Generic.List<CComplexObject> obsConstraints = new System.Collections.Generic.List<CComplexObject>(observationMap.Keys);
            foreach (CComplexObject obsConstraint in obsConstraints)
            {
                CComplexObject o = obsConstraint;
                Observation obs = Model.Items.Where(i => i.LightValidate(o)).FirstOrDefault() as Observation;
                Check.Require(obs != null);
                observationMap[obsConstraint] = obs;
            }
        }

        protected override void RemoveItem(object sender, GridViewEventArgs e)
        {
            foreach (PointEvent<ItemStructure> eventForRow in GetEventsForRow(e.Row))
            {
                History<ItemStructure> parentHistory = eventForRow.Parent as History<ItemStructure>;
                Check.Assert(parentHistory != null);
                parentHistory.Events.Remove(eventForRow);
                eventRowMap.Remove(eventForRow);
            }
        }

        protected override void AddItem(object sender, GridViewEventArgs e)
        {
            foreach (CComplexObject obsConstraint in observationMap.Keys)
            {
                Observation obs = observationMap[obsConstraint];
                CComplexObject eventConstraint = GetEventConstraint(obsConstraint);
                PointEvent<ItemStructure> pEvent = RmFactory.InstantiatePointEvent(eventConstraint);
                obs.Data.AddChild(pEvent, eventConstraint); //generate unique name and add to history
                eventRowMap[pEvent] = e.Row;
            }
            //View.Size = View.IdealSize;
        }

        protected override void UpdateModelWithCellValue(object sender, GridViewEventArgs e)
        {
            //Treat the Date/Time field specially - update across all Obs's
            if (e.Column.Equals(DateTimeColumn))
            {
                foreach (Event<ItemStructure> evt in GetEventsForRow(e.Row))
                {
                    evt.Time = new DvDateTime((DateTime)e.Value);
                }
                return;
            }

            CComplexObject constraintForColumn = GetColumnByName(e.Column).Constraint;
            Element cellElement = FindElement(constraintForColumn, e.Row);
            setDataValue(cellElement, constraintForColumn, e.Value);
        }

        private void RowEditing(object sender, GridViewEventArgs e)
        {
            //Populate editor values
            foreach (ColumnDescriptor column in Columns)
            {
                
                //Will return null if e.Row points to an empty new record
                Element cellElement = FindElement(column.Constraint, e.Row);
                IScalarView editorView = View.EditorViewFor(column.Name);
                editorView.Value = editorView.DataValueProvider.ToRawValue(cellElement != null ? cellElement.Value : null);
            }
            //Below should always return an event
            IEnumerable<Event<ItemStructure>> eventsForRow = GetEventsForRow(e.Row);
            IScalarView dateView = View.EditorViewFor(DateTimeColumn);
            dateView.Value = eventsForRow.Count() > 0
                                 ? dateView.DataValueProvider.ToRawValue(eventsForRow.First().Time)
                                 : DateTime.Now;
        }

        private void RowEdited(object sender, GridViewEventArgs e)
        {
            //Save editor values back to model
            Guid row = e.Row;
            foreach (ColumnDescriptor column in Columns)
            {
                Element cellElement = FindElement(column.Constraint, row);
                IScalarView editorView = View.EditorViewFor(column.Name);
                cellElement.Value = editorView.DataValueProvider.ToDataValue(editorView.Value);
            }
            //Update event time
            IDateView dateView = View.EditorViewFor(DateTimeColumn) as IDateView;
            Check.Assert(dateView != null);
            DateTime newDate = dateView.Date ?? new DateTime(); //a cheat??
            DvDateTime newDvDate = dateView.DataValueProvider.ToDataValue(dateView.Date) as DvDateTime ?? new DvDateTime(newDate); //a cheat??
            foreach (Event<ItemStructure> evt in GetEventsForRow(row))
            {
                evt.Time = newDvDate;
            }

            //Finally, manually update grid cell values
            View.CellUpdated -= UpdateModelWithCellValue;
            UpdateGridRowFromModel(row);
            View.CellUpdated += UpdateModelWithCellValue;
        }

        public override void RefreshViewFromModel()
        {
            View.CellUpdated -= UpdateModelWithCellValue;
            View.RowAddRequest -= AddItem;
            View.RowDeleteRequest -= RemoveItem;
            View.WholeRowUpdated -= RowEdited;
            View.WholeRowEditing -= RowEditing;

            //Clear any previously filled rows
            View.Reset();
            eventRowMap.Clear();
            
            //This gets really tricky - we have to group events across observations by their times
            
            //First, for each observation order the events according to time - while also collating
            //the times across all observations and ordering them
            Dictionary<Observation, System.Collections.Generic.List<Event<ItemStructure>>> eventsMap = new Dictionary<Observation, System.Collections.Generic.List<Event<ItemStructure>>>();
            IEnumerable<Observation> observations = Model.Items.Cast<Observation>();
            foreach (Observation obs in observations)
            {
                eventsMap[obs] = new System.Collections.Generic.List<Event<ItemStructure>>(obs.Data.Events);
                eventsMap[obs].Sort(compareEventTimes);
            }
            //NOTE assume, for now, that for each event in an observation there is exactly one
            //other event in each of the other observations with the same time stamp
            Observation firstObs = observations.FirstOrDefault();
            Check.Assert(firstObs != null);
            IEnumerable<DvDateTime> eventTimes = eventsMap[firstObs].Select(e => e.Time);
            //Making sure the events for all observations are exactly aligned in terms of time
            Check.Assert(observations.All(o => eventsMap[o].Select(e => e.Time).SequenceEqual(eventTimes)));

            System.Collections.Generic.List<Guid> rowIds = new System.Collections.Generic.List<Guid>();
            int eventIndex = 0;
            //Now scan through the events across all observations one by one and add to grid
            //First populate the grid with as many rows as there are events
            foreach (DvDateTime eventTime in eventTimes)
            {
                Guid rowId = View.AddRow();
                rowIds.Add(rowId);
                foreach (Observation obs in observations)
                {
                    Event<ItemStructure> nextEvent = eventsMap[obs][eventIndex];
                    Check.Assert(nextEvent.Time.Equals(eventTime));
                    eventRowMap[nextEvent] = rowId;
                }
                eventIndex++;
            }
            //Then populate the cells
            foreach (Guid rowId in rowIds)
            {
                UpdateGridRowFromModel(rowId);
            }

            View.CellUpdated += UpdateModelWithCellValue;
            View.RowAddRequest += AddItem;
            View.RowDeleteRequest += RemoveItem;
            View.WholeRowUpdated += RowEdited;
            View.WholeRowEditing += RowEditing;
        }

        private void UpdateGridRowFromModel(Guid row)
        {
            IEnumerable<Event<ItemStructure>> eventsForRow = GetEventsForRow(row);
            foreach (Event<ItemStructure> evt in eventsForRow)
            {
                CComplexObject eventConst = ConstraintFor(evt);
                //Go through all the columns that apply to this particular event constraint and set values
                foreach (ColumnDescriptor column in Columns)
                {
                    if (!eventConst.IsParentOf(column.Constraint)) continue; //column doesn't belong to this event - skip
                    string relativePath = column.Constraint.RelativePath(eventConst);
                    if (column.ShowCell)
                    {
                        View.UpdateCell(row, column.Name, convertDataValue(evt.ItemAtPath(relativePath) as Element));
                    }
                }
            }
            //Set the composite columns
            foreach (CompositeDescriptor composite in Composites)
            {
                //Quite a mouthful! Basically, "stringify" each element that corresponds to the 
                //constraint in the composite's components array
                IEnumerable<string> componentElements =
                    composite.Components.Select(
                        c => DataValueSimplifiedString(FindElement(c, row).Value, c.GetArchetypeRoot()));
                string value = componentElements.ToPrettyString(composite.Separator);
                View.UpdateCell(row, composite.Name, value);
            }

            //Set the date/time column
            View.UpdateCell(row, DateTimeColumn, Iso8601DateTime.ToDateTime(new Iso8601DateTime(eventsForRow.First().Time.Value)));
        }

        private int compareEventTimes(Event<ItemStructure> x, Event<ItemStructure> y)
        {
            return Math.Sign(x.Time.Diff(y.Time).Magnitude);
        }

        private string DataValueSimplifiedString(DataValue value, CArchetypeRoot archRoot)
        {
            if (value == null)
                return null;
            if (value is DvCodedText)
                return AomHelper.ExtractOntologyText((value as DvCodedText).Value, archRoot);
            if (value is DvQuantity)
                return (value as DvQuantity).Magnitude.ToString();
            if (value is DvDateTime)
                return (value as DvDateTime).ToString("dd/MM/yyyy hh:mm:ss");
            return value.ToString();
        }

        private IEnumerable<Event<ItemStructure>> GetEventsForRow(Guid row)
        {
            return new System.Collections.Generic.List<Event<ItemStructure>>(eventRowMap.Keys.Where(o => eventRowMap[o] == row));
        }

        /// <summary>
        /// Finds the model Element that corresponds to the given column at given row in view
        /// </summary>
        /// <param name="constraintForColumn"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private Element FindElement(CComplexObject constraintForColumn, Guid row)
        {
            Locatable parent;
            CComplexObject parentConstraint = GetParentEventconstraint(constraintForColumn);
            if (parentConstraint != null)
            {
                parent = GetEventsForRow(row).Where(ev => ev.LightValidate(parentConstraint)).FirstOrDefault();
            }
            else
            {
                //Otherwise, must be within observation
                parentConstraint = GetParentObservationConstraint(constraintForColumn);
                Check.Assert(parentConstraint != null);
                parent = Model.Items.Where(i => i.LightValidate(parentConstraint)).FirstOrDefault();
            }
            if (parent == null)
                return null;
            
            string relativePath = constraintForColumn.RelativePath(parentConstraint);
            Element cellElement = parent.ItemAtPath(relativePath) as Element;
            Check.Assert(cellElement != null);
            return cellElement;
        }

        /// <summary>
        /// Gets the constraint satisfied by the given event
        /// </summary>
        /// <param name="pEvent"></param>
        /// <returns></returns>
        private CComplexObject ConstraintFor(Event<ItemStructure> pEvent)
        {
            foreach (CComplexObject obsConstraint in observationMap.Keys)
            {
                CComplexObject eventConst = GetEventConstraint(obsConstraint);
                if (pEvent.LightValidate(eventConst))
                    return eventConst;
            }
            return null;
        }

        /// <summary>
        /// Given a column contsraint, retrieve the parent event constraint that houses it
        /// </summary>
        /// <param name="columnConstraint"></param>
        /// <returns></returns>
        private CComplexObject GetParentEventconstraint(CComplexObject columnConstraint)
        {
            foreach (CComplexObject observationConstraint in observationMap.Keys)
            {
                CComplexObject eventConstraint = GetEventConstraint(observationConstraint);
                if (eventConstraint.IsParentOf(columnConstraint))
                    return eventConstraint;
            }
            return null;
        }

        private CComplexObject GetParentObservationConstraint(CComplexObject columnConstraint)
        {
            foreach (CComplexObject observationConstraint in observationMap.Keys)
            {
                if (observationConstraint.IsParentOf(columnConstraint))
                {
                    try
                    {
                        //Due to a current error with openEHR.NET paths handling, can't reliably determine the 
                        //parent observation of a given column constraint. So must perform this rather odd step.
                        observationConstraint.ConstraintAtPath(columnConstraint.RelativePath(observationConstraint));
                        return observationConstraint;
                    } catch {}
                }
            }
            return null;
        }

        /// <summary>
        /// Given an observation constraint, extract the child event constraint
        /// </summary>
        /// <param name="obsConstraint"></param>
        /// <returns></returns>
        private CComplexObject GetEventConstraint(CComplexObject obsConstraint)
        {
            CComplexObject dataConst = obsConstraint.ExtractChildConstraints("data").FirstOrDefault() as CComplexObject;
            Check.Require(dataConst != null);
            CComplexObject eventsConst = dataConst.ExtractChildConstraints("events").FirstOrDefault() as CComplexObject;
            Check.Require(eventsConst != null);
            return eventsConst;
        }
    }
}