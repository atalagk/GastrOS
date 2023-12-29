using System.Collections.Generic;
using System.Linq;
using GastrOs.Sde.Engine;
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
    public class SplasherViewTest : ModelViewTestBase
    {
        private CComplexObject constraint, elemConstraint;
        private Cluster model;
        private SplasherControl control;
        private ISplasherView<IMultiChoiceView> view;

        [SetUp]
        public void SetUp()
        {
            constraint = root.ConstraintAtPath("/items[at6000]/items[at6200]") as CComplexObject;
            elemConstraint = root.ConstraintAtPath("/items[at6000]/items[at6200]/items[at0500]") as CComplexObject;
            Assert.IsNotNull(constraint);

            model = new Cluster(new DvText("Ulcer"), "at6200", null, null, constraint.ExtractArchetyped(), null, new Item[0]);
            control = new SplasherControl(constraint, elemConstraint);
            view = container.Resolve<ISplasherView<IMultiChoiceView>>();
            IMultiChoiceView innerView = container.Resolve<IMultiChoiceView>();
            view.SplashedView = innerView;
        }

        [Test]
        public void InitialConstructionTest()
        {
            //view properties
            Assert.IsTrue(string.IsNullOrEmpty(view.Title));
            //model properties
            Assert.AreEqual(0, model.Items.Count);

            control.Model = model;
            control.View = view;
            Assert.AreEqual(model, control.Model);
            Assert.AreEqual(view, control.View);

            //view properties
            Assert.AreEqual("Site(s)", view.Title);
            Assert.IsFalse(view.SplashedView.Visible);
            //model properties
            Assert.AreEqual(0, model.Items.Count);
        }

        [Test]
        public void SplashTest()
        {
            InitialConstructionTest();

            view.OpenSplash();
            Assert.IsTrue(view.SplashedView.Visible);
            view.CloseSplash();
            Assert.IsFalse(view.SplashedView.Visible);
        }

        [Test]
        public void SelectionTest()
        {
            InitialConstructionTest();

            view.OpenSplash();
            Assert.IsTrue(view.SplashedView.SetSelected("at0501", true));
            Assert.IsTrue(view.SplashedView.SetSelected("at0512", true));
            Assert.IsTrue(view.SplashedView.SetSelected("at0526", true));
            Assert.IsFalse(view.SplashedView.SetSelected("at0512", true));
            Assert.IsFalse(view.SplashedView.SetSelected("at0501", true));

            //verify view records selected items properly
            Assert.AreEqual(3, view.SplashedView.SelectedItems.Count());
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0501"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0512"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0526"));
            //verify model updated
            IEnumerable<string> selectedCodes =
                model.Items.Cast<Element>().Select(elem => elem.ValueAs<DvCodedText>().Value);
            Assert.AreEqual(3, selectedCodes.Count());
            Assert.IsTrue(selectedCodes.Contains("at0501"));
            Assert.IsTrue(selectedCodes.Contains("at0512"));
            Assert.IsTrue(selectedCodes.Contains("at0526"));

            //change selection (#1)
            Assert.IsTrue(view.SplashedView.SetSelected("at0501", false));
            Assert.IsTrue(view.SplashedView.SetSelected("at0512", false));
            Assert.IsFalse(view.SplashedView.SetSelected("at0512", false));

            //verify view records selected items properly
            Assert.AreEqual(1, view.SplashedView.SelectedItems.Count());
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0526"));
            //verify model updated
            selectedCodes = model.Items.Cast<Element>().Select(elem => elem.ValueAs<DvCodedText>().Value);
            Assert.AreEqual(1, selectedCodes.Count());
            Assert.IsTrue(selectedCodes.Contains("at0526"));

            //change selection (#2)
            Assert.IsTrue(view.SplashedView.SetSelected("at0501", true));
            Assert.IsTrue(view.SplashedView.SetSelected("at0526", false));
            Assert.IsTrue(view.SplashedView.SetSelected("at0512", true));
            Assert.IsTrue(view.SplashedView.SetSelected("at0513", true));
            Assert.IsTrue(view.SplashedView.SetSelected("at0518", true));

            //verify view records selected items properly
            Assert.AreEqual(4, view.SplashedView.SelectedItems.Count());
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0501"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0512"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0513"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0518"));
            //verify model updated
            selectedCodes =
                model.Items.Cast<Element>().Select(elem => elem.ValueAs<DvCodedText>().Value);
            Assert.AreEqual(4, selectedCodes.Count());
            Assert.IsTrue(selectedCodes.Contains("at0501"));
            Assert.IsTrue(selectedCodes.Contains("at0512"));
            Assert.IsTrue(selectedCodes.Contains("at0513"));
            Assert.IsTrue(selectedCodes.Contains("at0518"));
        }

        [Test]
        public void TestLoadFromExistingModel()
        {
            //initial view properties
            Assert.IsTrue(string.IsNullOrEmpty(view.Title));
            Assert.AreEqual(0, view.SplashedView.SelectedItems.Count());

            //initial model properties
            Assert.AreEqual(0, model.Items.Count);

            Element site1 = RmFactory.InstantiateElement(elemConstraint);
            site1.Name = new DvText("Site(s)at0501");
            site1.ValueAs<DvCodedText>().Value = "at0501";
            Element site2 = RmFactory.InstantiateElement(elemConstraint);
            site2.Name = new DvText("Site(s)at0512");
            site2.ValueAs<DvCodedText>().Value = "at0512";
            Element site3 = RmFactory.InstantiateElement(elemConstraint);
            site3.Name = new DvText("Site(s)at0518");
            site3.ValueAs<DvCodedText>().Value = "at0518";

            model.Items.Add(site1);
            model.Items.Add(site2);
            model.Items.Add(site3);
            
            control.Model = model;
            control.View = view;
            Assert.AreEqual(model, control.Model);
            Assert.AreEqual(view, control.View);

            //View properties
            Assert.AreEqual("Site(s)", view.Title);
            Assert.IsFalse(view.SplashedView.Visible);
            //View should now be updated to reflect the model
            Assert.AreEqual(3, view.SplashedView.SelectedItems.Count());
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0501"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0512"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0518"));

            //Deselect an item from view
            view.SplashedView.SetSelected("at0512", false);
            Assert.AreEqual(2, view.SplashedView.SelectedItems.Count());
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0501"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0518"));
            //Model should be updated to reflect view
            IEnumerable<string> selectedCodes =
                model.Items.Cast<Element>().Select(elem => elem.ValueAs<DvCodedText>().Value);
            Assert.AreEqual(2, selectedCodes.Count());
            Assert.IsTrue(selectedCodes.Contains("at0501"));
            Assert.IsTrue(selectedCodes.Contains("at0518"));

            //Select a different item from view
            view.SplashedView.SetSelected("at0523", true);
            Assert.AreEqual(3, view.SplashedView.SelectedItems.Count());
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0501"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0518"));
            Assert.IsTrue(view.SplashedView.SelectedItems.Contains("at0523"));
            //Model should be updated to reflect view
            selectedCodes = model.Items.Cast<Element>().Select(elem => elem.ValueAs<DvCodedText>().Value);
            Assert.AreEqual(3, selectedCodes.Count());
            Assert.IsTrue(selectedCodes.Contains("at0501"));
            Assert.IsTrue(selectedCodes.Contains("at0518"));
            Assert.IsTrue(selectedCodes.Contains("at0523"));
        }
    }
}
