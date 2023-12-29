using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.DesignByContract;
using OpenEhr.RM.Composition.Content;
using OpenEhr.RM.Composition.Content.Entry;
using OpenEhr.RM.Composition.Content.Navigation;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;

namespace GastrOs.Sde.ViewControls
{
    /// <summary>
    /// A rather special-purpose viewcontrol for representing a section of evaluations
    /// as a grid. The following restrictions apply:
    /// 1) section must contain a single type of Evaluation entry, which can be multiply-occurring
    /// 2) the Evaluation entry can contain a hierarchy of Items, BUT each item may only occur at most once
    /// 3) the children to display as column MUST be of ELEMENT RM type
    /// </summary>
    public class EvaluationGridControl : GridControlBase<Section>
    {
        private CComplexObject evalConstraint;
        private Dictionary<Guid, Evaluation> rowEvalMap;

        public EvaluationGridControl(CComplexObject constraint, IEnumerable<AttributeDescriptor> attributeDescriptors)
            : base(constraint, attributeDescriptors)
        {
            //Check the restrictions - for now just check the minimum required to work...
            evalConstraint = constraint.ExtractChildConstraints("items").FirstOrDefault() as CComplexObject;
            Check.Require(evalConstraint != null && evalConstraint.RmTypeMatches<Evaluation>());

            rowEvalMap = new Dictionary<Guid, Evaluation>();
        }

        protected override void SetModelPostexecute(Section oldModel)
        {
            //Verify that the section contains evaluations that all match the constraint
            foreach (ContentItem item in Model.Items)
            {
                Evaluation eval = item as Evaluation;
                Check.Invariant(eval != null && eval.LightValidate(evalConstraint));
            }
        }

        protected override void RemoveItem(object sender, GridViewEventArgs e)
        {
            Evaluation eval = rowEvalMap[e.Row];
            Model.Items.Remove(eval);
            rowEvalMap.Remove(e.Row);
        }

        protected override void AddItem(object sender, GridViewEventArgs e)
        {
            Evaluation eval = RmFactory.InstantiateEvaluation(evalConstraint);
            Model.AddChild(eval, evalConstraint);
            rowEvalMap[e.Row] = eval;
        }

        protected override void UpdateModelWithCellValue(object sender, GridViewEventArgs e)
        {
            Evaluation eval = rowEvalMap[e.Row];
            CComplexObject columnConstraint = GetColumnByName(e.Column).Constraint;
            string relativePath = columnConstraint.RelativePath(evalConstraint);
            Element cellElement = eval.ItemAtPath(relativePath) as Element;
            setDataValue(cellElement, columnConstraint, e.Value);
        }

        public override void RefreshViewFromModel()
        {
            View.CellUpdated -= UpdateModelWithCellValue;
            View.RowAddRequest -= AddItem;
            View.RowDeleteRequest -= RemoveItem;

            //Clear any previously filled rows
            View.Reset();
            //We've already established that the section (Model) contains only evaluation entries
            foreach (Evaluation eval in Model.Items)
            {
                //For each evaluation, add a corresponding row on the grid
                //And for each column, set the column value
                Guid row = View.AddRow();
                rowEvalMap[row] = eval;
                foreach (ColumnDescriptor column in Columns)
                {
                    CComplexObject columnConstraint = column.Constraint;
                    string relativePath = columnConstraint.RelativePath(evalConstraint);
                    Element cellElement = eval.ItemAtPath(relativePath) as Element;
                    Check.Assert(cellElement != null);
                    View.UpdateCell(row, column.Name, convertDataValue(cellElement));
                }
            }

            View.CellUpdated += UpdateModelWithCellValue;
            View.RowAddRequest += AddItem;
            View.RowDeleteRequest += RemoveItem;
        }
    }
}
