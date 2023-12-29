using System.Linq;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views;
using NUnit.Framework;
using OpenEhr.AM.Archetype.ConstraintModel;
using Microsoft.Practices.Unity;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.Test.GuiTests
{
    [TestFixture]
    public class CoreConceptViewTest : ModelViewTestBase
    {
        private CComplexObject constraint;
        private CComplexObject stigConstraint;
        private CComplexObject presenceConstraint;
        private CoreConceptControl _control;
        private CodedTextElementControl _stigControl;
        private ICoreConceptView view;
        private IListView stigView;
        private Cluster model;
        private Element presenceModel, stigModel;

        [SetUp]
        public void SetUp()
        {
            constraint = root.ConstraintAtPath("/items[at6000]/items[at6200]") as CComplexObject;
            Assert.IsNotNull(constraint);
            stigConstraint = root.ConstraintAtPath("/items[at6000]/items[at6200]/items[at6250]") as CComplexObject;
            Assert.IsNotNull(stigConstraint);
            presenceConstraint = root.ConstraintAtPath("/items[at6000]/items[at6200]/items[at0105]") as CComplexObject;
            Assert.IsNotNull(presenceConstraint);

            string presenceTermId =
                ((CComplexObject)presenceConstraint.ExtractElemValueConstraint()).ExtractCodePhrase().TerminologyId.
                    Value;
            string stigTermId =
                ((CComplexObject)stigConstraint.ExtractElemValueConstraint()).ExtractCodePhrase().TerminologyId.Value;

            model = new Cluster(new DvText("Ulcer"), "at6200", null, null, constraint.ExtractArchetyped(), null, new Item[0]);
            presenceModel = new Element(new DvText("Present?"), "at0105", null, null, presenceConstraint.ExtractArchetyped(), null,
                new DvCodedText("at1121", "", presenceTermId), null);
            stigModel = new Element(new DvText("Stigmata of bleeding"), "at6250", null, null, stigConstraint.ExtractArchetyped(),
                null, new DvCodedText("at6251", "", stigTermId), null);
            model.Items.Add(presenceModel);
            model.Items.Add(stigModel);

            view = container.Resolve<ICoreConceptView>();
            stigView = container.Resolve<IListView>("default");

            _stigControl = new CodedTextElementControl(stigConstraint);
            _control = new CoreConceptControl(constraint);
        }

        [Test]
        public void InitialConstructionTest()
        {
            //view properties
            Assert.AreEqual(PresenceState.Null, view.Presence);
            Assert.IsTrue(string.IsNullOrEmpty(view.Title));
            Assert.AreEqual(0, view.Children.Count());
            Assert.IsTrue(view.Visible);
            Assert.IsTrue(string.IsNullOrEmpty(stigView.Title));
            Assert.IsTrue(stigView.Visible);
            //model properties
            Assert.AreEqual(PresenceState.Null, model.GetPresence(constraint));
            Assert.AreEqual("at6251", stigModel.ValueAs<DvCodedText>().Value);

            _stigControl.Model = stigModel;
            _stigControl.View = stigView;

            view.Children.Add(stigView);

            _control.Model = model;
            _control.View = view;

            //view properties
            Assert.AreEqual(PresenceState.Null, view.Presence);
            Assert.AreEqual("Ulcer", view.Title);
            Assert.AreEqual(1, view.Children.Count());
            Assert.AreEqual(stigView, view.Children.First());
            Assert.IsTrue(view.Visible);
            Assert.AreEqual("Stigmata of bleeding", stigView.Title);
            Assert.IsFalse(stigView.Visible);
            //model properties
            Assert.AreEqual(PresenceState.Null, model.GetPresence(constraint));
            Assert.AreEqual("at6251", stigModel.ValueAs<DvCodedText>().Value);
        }

        [Test]
        public void ChangePresenceTest()
        {
            InitialConstructionTest();

            view.Presence = PresenceState.Present;

            //view properties
            Assert.AreEqual(PresenceState.Present, view.Presence);
            Assert.AreEqual("Ulcer", view.Title);
            Assert.AreEqual(1, view.Children.Count());
            Assert.AreEqual(stigView, view.Children.First());
            Assert.IsTrue(view.Visible);
            Assert.AreEqual("Stigmata of bleeding", stigView.Title);
            Assert.IsTrue(stigView.Visible);
            //model properties
            Assert.AreEqual(PresenceState.Present, model.GetPresence(constraint));
            Assert.AreEqual("at6251", stigModel.ValueAs<DvCodedText>().Value);

            view.Presence = PresenceState.Null;

            //view properties
            Assert.AreEqual(PresenceState.Null, view.Presence);
            Assert.AreEqual("Ulcer", view.Title);
            Assert.AreEqual(1, view.Children.Count());
            Assert.AreEqual(stigView, view.Children.First());
            Assert.IsTrue(view.Visible);
            Assert.AreEqual("Stigmata of bleeding", stigView.Title);
            Assert.IsFalse(stigView.Visible);
            //model properties
            Assert.AreEqual(PresenceState.Null, model.GetPresence(constraint));
            Assert.AreEqual("at6251", stigModel.ValueAs<DvCodedText>().Value);
        }
        //TODO test more variations on presence
    }
}
