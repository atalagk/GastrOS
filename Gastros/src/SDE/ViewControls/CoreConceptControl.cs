using System;
using System.Collections.Generic;
using System.Linq;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;

namespace GastrOs.Sde.ViewControls
{
    public class CoreConceptControl : TypedViewControl<Cluster, ICoreConceptView>
    {
        private CComplexObject presenceConstraint;
        private Element presenceElement;

        public CoreConceptControl(CComplexObject constraint)
            : base(constraint)
        {
            presenceConstraint = Constraint.GetPresenceConstraint();
            Check.Invariant(presenceConstraint != null, "Core concept (path: " + constraint.Path +
                           ") must have an element indicating presence");
        }

        ~CoreConceptControl()
        {
            //Release event handlers to avoid memory leak
            View.PresenceChanged -= UpdatePresence;
            View.ChildAdded -= ChildAdded;
        }

        public Element PresenceElement
        {
            get { return presenceElement; }
        }

        protected override void SetModelPostexecute(Cluster oldModel)
        {
            presenceElement = Model.PresenceElement(Constraint);
            Check.Invariant(presenceElement != null, "Core concept " + Model +
                           " must have an element indicating presence");
        }

        protected override void SetViewPostexecute(ICoreConceptView oldView)
        {
            if (oldView != null)
            {
                oldView.PresenceChanged -= UpdatePresence;
                oldView.ChildAdded -= ChildAdded;
            }

            //Update the view so that it only displays the necessary presence states
            IDictionary<PresenceState, string> presenceSemantics = presenceConstraint.GetPresenceSemantics();
            View.SetAvailablePresenceStates(presenceSemantics.Keys.Aggregate((s1, s2) => s1 | s2));

            View.PresenceChanged += UpdatePresence;
            View.ChildAdded += ChildAdded;
        }

        private void UpdatePresence(object sender, EventArgs e)
        {
            Model.SetPresence(Constraint, View.Presence);
        }

        private void ChildAdded(object sender, ChildEventArgs e)
        {
            //ensure added child's visibility matches this core concept's presence
            e.Child.Visible = View.Presence != PresenceState.Null;
        }

        public override bool AllowsChildren
        {
            get { return true; }
        }

        public override void UpdateViewFromModel()
        {
            View.Title = TitleFunction();
            View.Presence = Model.GetPresence(Constraint);
        }
    }
}