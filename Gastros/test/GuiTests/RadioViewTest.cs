using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views.WinForms;
using NUnit.Framework;

namespace GastrOs.Sde.Test.GuiTests
{
    [TestFixture]
    public class RadioViewTest
    {
        [Test]
        public void CreateRadioView()
        {
            RadioWidget rw = new RadioWidget();
            rw.ChoiceList = new List<OntologyItem>
                                {
                                    new OntologyItem("atxxxx") {Text = ""},
                                    new OntologyItem("at0001") {Text = "Hi"},
                                    new OntologyItem("at0002") {Text = "there"},
                                    new OntologyItem("at0003") {Text = "asf"},
                                    new OntologyItem("at0004") {Text = "dsfadf322fawfdsf"},
                                    new OntologyItem("at0005") {Text = "fdsavs"}
                                };
            rw.Title = "Hello";

            rw.CanAddNewInstance = true;
            rw.CanRemoveInstance = false;

            Form f = new Form() {Width = 300, Height = 300};
            f.Load += delegate { rw.Size = rw.IdealSize; };
            f.Controls.Add(rw);
            f.ShowDialog();
            f.Dispose();
        }
    }
}
