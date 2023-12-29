using System;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views;
using NUnit.Framework;
using OpenEhr.AM.Archetype.ConstraintModel;
using Microsoft.Practices.Unity;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.Test.GuiTests
{
    [TestFixture]
    public class TextElementViewTest : ModelViewTestBase
    {
        protected CComplexObject textConstraint;
        private Element model;
        private ITextView view;
        private TextElementControl _control;

        [SetUp]
        public void Setup()
        {
            textConstraint = root.ConstraintAtPath("/items[at6000]/items[at6200]/items[at6230]") as CComplexObject;
            Assert.IsNotNull(textConstraint);

            _control = new TextElementControl(textConstraint);
            
            model = new Element(new DvText("text1"), "at6230", null, null, textConstraint.ExtractArchetyped(), null, new DvText("Square"), null);
            view = container.Resolve<ITextView>("default");
        }

        [Test]
        public void InitialConstructionTest()
        {
            Assert.IsTrue(string.IsNullOrEmpty(view.Text));
            Assert.IsTrue(string.IsNullOrEmpty(view.Title));
            Assert.IsTrue(view.Visible);

            //Test initial construction
            _control.Model = model;
            Assert.AreEqual(model, _control.Model);
            _control.View = view;
            Assert.AreEqual(view, _control.View);

            Assert.AreEqual("Square", view.Text);
            Assert.AreEqual("Shape", view.Title);
            Assert.IsTrue(view.Visible);
        }

        [Test]
        public void SwitchModelViewTest()
        {
            InitialConstructionTest();

            //Introduce new view and model
            Element textModel2 = new Element(new DvText("text2"), "at6230", null, null, textConstraint.ExtractArchetyped(), null, new DvText("Triangle"), null);
            ITextView textView2 = container.Resolve<ITextView>("multi");

            Assert.IsTrue(string.IsNullOrEmpty(textView2.Text));
            Assert.IsTrue(string.IsNullOrEmpty(textView2.Title));
            Assert.IsTrue(textView2.Visible);

            //Test switching of model
            _control.Model = textModel2;
            Assert.AreEqual("Triangle", view.Text);
            Assert.AreEqual("Shape", view.Title);
            Assert.IsTrue(view.Visible);

            //Test switching of view
            _control.View = textView2;
            Assert.AreEqual("Triangle", textView2.Text);
            Assert.AreEqual("Shape", textView2.Title);
            Assert.IsTrue(textView2.Visible);
        }

        [Test]
        public void ViewUpdateTest()
        {
            InitialConstructionTest();

            view.Text = "Changed!";

            Assert.AreEqual("Changed!", model.ValueAs<DvText>().Value);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidModelTest()
        {
            InitialConstructionTest();

            _control.Model = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidModelTest2()
        {
            InitialConstructionTest();

            _control.Model = new Element(new DvText("text3"), "at0000", null, null, textConstraint.ExtractArchetyped(), null, new DvText("Invalid"), null);
        }

        [Test]
        [ExpectedException(typeof(PreconditionException))]
        public void InvalidModelTest3()
        {
            InitialConstructionTest();

            _control.Model = new Element(new DvText("text3"), "at6230", null, null, textConstraint.ExtractArchetyped(), null, new DvBoolean(true), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidViewTest()
        {
            InitialConstructionTest();

            _control.View = null;
        }
    }
}
