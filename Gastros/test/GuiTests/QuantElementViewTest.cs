using System;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views;
using NUnit.Framework;
using OpenEhr.AM.Archetype.ConstraintModel;
using Microsoft.Practices.Unity;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.Test.GuiTests
{
    [TestFixture]
    public class QuantElementViewTest : ModelViewTestBase
    {
        protected CComplexObject quantConstraint;
        private Element model;
        private INumericUnitView view;
        private QuantElementControl _control;

        [SetUp]
        public void SetUp()
        {
            quantConstraint = root.ConstraintAtPath("/items[at6000]/items[at6200]/items[at6220]") as CComplexObject;
            Assert.IsNotNull(quantConstraint);

            _control = new QuantElementControl(quantConstraint);

            model = new Element(new DvText("quantity1"), "at6220", null, null, null, null, new DvQuantity(10, "mm"), null);
            view = container.Resolve<INumericUnitView>();
        }

        [Test]
        public void InitialConstructionTest()
        {
            Assert.AreEqual(0, view.Value);
            Assert.IsTrue(string.IsNullOrEmpty(view.Title));
            Assert.IsNull(view.Unit);
            Assert.IsTrue(view.Visible);

            //Test initial construction
            _control.Model = model;
            Assert.AreEqual(model, _control.Model);
            _control.View = view;
            Assert.AreEqual(view, _control.View);

            Assert.AreEqual(10, view.Value);
            Assert.AreEqual("Size", view.Title);
            Assert.AreEqual("mm", view.Unit);
            Assert.IsTrue(view.Visible);

            Assert.AreEqual(2, view.AvailableUnits.Count);
            Assert.IsTrue(view.AvailableUnits.Contains("mm"));
            Assert.IsTrue(view.AvailableUnits.Contains("in"));
        }

        [Test]
        public void SwitchModelViewTest()
        {
            InitialConstructionTest();

            //Introduce new view and model
            Element model2 = new Element(new DvText("quantity2"), "at6220", null, null, null, null, new DvQuantity(1, "in"), null);
            INumericUnitView view2 = container.Resolve<INumericUnitView>();

            Assert.AreEqual(0, view2.Value);
            Assert.IsTrue(string.IsNullOrEmpty(view2.Title));
            Assert.IsNull(view2.Unit);
            Assert.IsTrue(view2.Visible);

            //Test switching of model
            _control.Model = model2;
            Assert.AreEqual(1, view.Value);
            Assert.AreEqual("Size", view.Title);
            Assert.AreEqual("in", view.Unit);
            Assert.IsTrue(view.Visible);

            //Test switching of view
            _control.View = view2;
            Assert.AreEqual(1, view2.Value);
            Assert.AreEqual("Size", view2.Title);
            Assert.AreEqual("in", view2.Unit);
            Assert.IsTrue(view2.Visible);
        }

        [Test]
        public void ViewUpdateTest()
        {
            InitialConstructionTest();

            view.Value = 30;

            Assert.AreEqual(30, model.ValueAs<DvQuantity>().Magnitude);
            Assert.AreEqual("mm", model.ValueAs<DvQuantity>().Units);

            view.Unit = "in";

            Assert.AreEqual(30, model.ValueAs<DvQuantity>().Magnitude);
            Assert.AreEqual("in", model.ValueAs<DvQuantity>().Units);
        }

        [Test]
        public void InvalidViewUpdateTest()
        {
            InitialConstructionTest();

            view.MaxValue = 100;

            view.Value = 100000;

            Assert.AreEqual(10, model.ValueAs<DvQuantity>().Magnitude);
            Assert.AreEqual("mm", model.ValueAs<DvQuantity>().Units);
        }

        [Test]
        public void ChangeUnitTest()
        {
            InitialConstructionTest();

            view.Unit = "in";

            Assert.AreEqual(10, model.ValueAs<DvQuantity>().Magnitude);
            Assert.AreEqual("in", model.ValueAs<DvQuantity>().Units);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidModelTest()
        {
            InitialConstructionTest();

            _control.Model = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidViewTest()
        {
            InitialConstructionTest();

            _control.View = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidModelTest2()
        {
            InitialConstructionTest();

            _control.Model = new Element(new DvText("text3"), "at0000", null, null, null, null, new DvText("Invalid"), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidModelTest3()
        {
            InitialConstructionTest();

            _control.Model = new Element(new DvText("text3"), "at6230", null, null, null, null, new DvBoolean(true), null);
        }
    }
}
