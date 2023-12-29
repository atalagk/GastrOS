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
    public class CountElementViewTest : ModelViewTestBase
    {
        protected CComplexObject constraint;
        private Element model;
        private INumericView view;
        private CountElementControl _control;

        [SetUp]
        public void SetUp()
        {
            constraint = root.ConstraintAtPath("/items[at6000]/items[at6200]/items[at6210]") as CComplexObject;
            Assert.IsNotNull(constraint);

            _control = new CountElementControl(constraint);

            model = new Element(new DvText("count1"), "at6210", null, null, null, null, new DvCount(99), null);
            view = container.Resolve<INumericView>();
        }

        [Test]
        public void InitialConstructionTest()
        {
            //initial state
            Assert.AreEqual(0, view.Value);
            Assert.IsTrue(string.IsNullOrEmpty(view.Title));
            Assert.AreEqual(decimal.MinValue, view.MinValue);
            Assert.AreEqual(decimal.MaxValue, view.MaxValue);

            _control.Model = model;
            _control.View = view;
            Assert.AreEqual(model, _control.Model);
            Assert.AreEqual(view, _control.View);

            //view must match model
            Assert.AreEqual(99, view.Value);
            Assert.AreEqual("Number", view.Title);
            Assert.AreEqual(0, view.MinValue);
            Assert.AreEqual(100, view.MaxValue);
        }

        [Test]
        public void ViewUpdateTest()
        {
            InitialConstructionTest();

            view.Value = 65;
            //ensure model is updated
            Assert.AreEqual(65, view.Value);
            Assert.AreEqual(65, model.ValueAs<DvCount>().Magnitude);

            view.Value = 10000;
            Assert.AreEqual(65, view.Value);
            Assert.AreEqual(65, model.ValueAs<DvCount>().Magnitude);
        }
        
        [Test]
        public void SwitchModelViewTest()
        {
            InitialConstructionTest();

            Element model2 = new Element(new DvText("count2"), "at6210", null, null, null, null, new DvCount(11), null);
            INumericUnitView view2 = container.Resolve<INumericUnitView>();

            _control.Model = model2;
            Assert.AreEqual(model2, _control.Model);

            //view must match switched model
            Assert.AreEqual(11, view.Value);
            Assert.AreEqual("Number", view.Title);
            Assert.AreEqual(0, view.MinValue);
            Assert.AreEqual(100, view.MaxValue);

            _control.View = view2;
            //switched view should be filled with values from switched model
            Assert.AreEqual(view2, _control.View);
            Assert.AreEqual(11, view2.Value);
            Assert.AreEqual("Number", view2.Title);
            Assert.AreEqual(0, view2.MinValue);
            Assert.AreEqual(100, view2.MaxValue);

            view2.Value = 29;
            //model2 should be updated; model remains the same
            Assert.AreEqual(29, model2.ValueAs<DvCount>().Magnitude);
            Assert.AreEqual(99, model.ValueAs<DvCount>().Magnitude);
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
