using GastrOs.Sde.Views;
using Microsoft.Practices.Unity;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.Common.Archetyped.Impl;

namespace GastrOs.Sde.ViewControls
{
    /// <summary>
    /// Provides a type-safe template for presenters
    /// </summary>
    /// <typeparam name="TM">type of the model object</typeparam>
    /// <typeparam name="TV">type of the view object</typeparam>
    public abstract class TypedViewControl<TM,TV> : ViewControl
        where TM : Locatable
        where TV : class, IView
    {
        protected TypedViewControl(CComplexObject constraint)
            : base(constraint)
        {
        }

        [Dependency]
        public new virtual TV View
        {
            get
            {
                return base.View as TV;
            }
            set
            {
                base.View = value;
            }
        }

        public new virtual TM Model
        {
            get
            {
                return base.Model as TM;
            }
            set
            {
                base.Model = value;
            }
        }

        protected sealed override void SetModelPostexecute(Locatable oldModel)
        {
            SetModelPostexecute(oldModel as TM);
        }

        protected sealed override void SetViewPostexecute(IView oldView)
        {
            SetViewPostexecute(oldView as TV);
        }

        /// <summary>
        /// A type-safe override
        /// </summary>
        /// <param name="oldModel"></param>
        protected virtual void SetModelPostexecute(TM oldModel) {}

        /// <summary>
        /// A type-safe override
        /// </summary>
        /// <param name="oldView"></param>
        protected virtual void SetViewPostexecute(TV oldView) {}
    }
}