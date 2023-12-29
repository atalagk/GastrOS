using System.Collections.Generic;
using System.Linq;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views;
using NUnit.Framework;
using OpenEhr.AM.Archetype.ConstraintModel;
using Microsoft.Practices.Unity;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.Test.GuiTests
{
    internal class SimpleOntologyComparer : IEqualityComparer<OntologyItem>
    {
        public bool Equals(OntologyItem x, OntologyItem y)
        {
            return string.Equals(x.ID, y.ID);
        }

        public int GetHashCode(OntologyItem obj)
        {
            return obj.ID.GetHashCode();
        }
    }

    [TestFixture]
    public class CodedTextElementViewTest : ModelViewTestBase
    {
        protected CComplexObject constraint;
        private Element model;
        private IListView view;
        private CodedTextElementControl control;
        private string termId;

        private SimpleOntologyComparer SimpleOntologyComparer = new SimpleOntologyComparer();

        [SetUp]
        public void Setup()
        {
            constraint = root.ConstraintAtPath("/items[at6000]/items[at6200]/items[at6250]") as CComplexObject;
            Assert.IsNotNull(constraint);

            control = new CodedTextElementControl(constraint);

            termId = ((CComplexObject)constraint.ExtractElemValueConstraint()).ExtractCodePhrase().TerminologyId.Value;
            model = new Element(new DvCodedText("text1"), "at6250", null, null, constraint.ExtractArchetyped(), null, new DvCodedText("at6251", "", termId), null);
            view = container.Resolve<IListView>("default");
        }

        [Test]
        public void InitialConstructionTest()
        {
            Assert.IsTrue(string.IsNullOrEmpty(view.Text));
            Assert.IsTrue(string.IsNullOrEmpty(view.Title));
            Assert.IsTrue(view.Visible);

            //Test initial construction
            control.Model = model;
            Assert.AreEqual(model, control.Model);
            control.View = view;
            Assert.AreEqual(view, control.View);

            Assert.AreEqual("at6251", view.Text);
            Assert.AreEqual("Stigmata of bleeding", view.Title);
            Assert.IsTrue(view.Visible);
            Assert.AreEqual(5, view.ChoiceList.Count);
            Assert.IsTrue(view.ChoiceList.Contains(new OntologyItem("atxxxx"), SimpleOntologyComparer));
            Assert.IsTrue(view.ChoiceList.Contains(new OntologyItem("at6251"), SimpleOntologyComparer));
            Assert.IsTrue(view.ChoiceList.Contains(new OntologyItem("at6252"), SimpleOntologyComparer));
            Assert.IsTrue(view.ChoiceList.Contains(new OntologyItem("at6253"), SimpleOntologyComparer));
            Assert.IsTrue(view.ChoiceList.Contains(new OntologyItem("at6254"), SimpleOntologyComparer));
        }

        [Test]
        public void SwitchModelViewTest()
        {
            InitialConstructionTest();

            //Introduce new view and model
            Element textModel2 = new Element(new DvText("text2"), "at6250", null, null, constraint.ExtractArchetyped(), null, new DvCodedText("at6252", "", termId), null);
            IListView textView2 = container.Resolve<IListView>("default");

            Assert.IsTrue(string.IsNullOrEmpty(textView2.Text));
            Assert.IsTrue(string.IsNullOrEmpty(textView2.Title));
            Assert.IsTrue(textView2.Visible);

            //Test switching of model
            control.Model = textModel2;
            Assert.AreEqual("at6252", view.Text);
            Assert.AreEqual("Stigmata of bleeding", view.Title);
            Assert.IsTrue(view.Visible);

            //Test switching of view
            control.View = textView2;
            Assert.AreEqual("at6252", textView2.Text);
            Assert.AreEqual("Stigmata of bleeding", textView2.Title);
            Assert.IsTrue(textView2.Visible);
        }

        [Test]
        public void ViewUpdateTest()
        {
            InitialConstructionTest();

            view.Text = "at6253";

            Assert.AreEqual("at6253", model.ValueAs<DvText>().Value);
        }
    }
}
