using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;

namespace GastrOs.Sde.ViewControls
{
    public class SimpleContainerControl<T> : TypedViewControl<T, IContainerView> where T : Locatable
    {
        private bool framed;

        public SimpleContainerControl(CComplexObject constraint, bool framed)
            : base(constraint)
        {
            this.framed = framed;
        }

        protected override void SetModelPostexecute(T oldModel)
        {
            // anything??
        }

        protected override void SetViewPostexecute(IContainerView oldView)
        {
            View.Framed = framed;
        }

        public override void RefreshViewFromModel()
        {
            View.Title = TitleFunction();
        }

        public override bool AllowsChildren
        {
            get { return true; }
        }
    }
}