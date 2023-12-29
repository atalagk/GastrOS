using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;

namespace GastrOs.Sde.ViewControls
{
    public class SimpleContainerControl : TypedViewControl<Cluster, IContainerView>
    {
        private bool framed;

        public SimpleContainerControl(CComplexObject constraint, bool framed)
            : base(constraint)
        {
            this.framed = framed;
        }

        protected override void SetModelPostexecute(Cluster oldModel)
        {
            // anything??
        }

        protected override void SetViewPostexecute(IContainerView oldView)
        {
            View.Framed = framed;
        }

        public override void UpdateViewFromModel()
        {
            View.Title = TitleFunction();
        }

        public override bool AllowsChildren
        {
            get { return true; }
        }
    }
}