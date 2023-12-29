using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Support;
using NUnit.Framework;

namespace GastrOs.Sde.Test
{
    [TestFixture]
    public class TestDirectivesParsing
    {
        [Test]
        public void TestAlternateStyle()
        {
            AlternateStyleDirective dir = new AlternateStyleDirective();
            Assert.AreEqual("alternateStyle", dir.Name);

            dir.ParseParameters("align=horizontal", "border=true", "label=true");
            Assert.AreEqual(ItemAlignment.Horizontal, dir.Alignment);
            Assert.AreEqual(true, dir.ShowBorder);
            Assert.AreEqual(true, dir.ShowLabel);

            dir = new AlternateStyleDirective();
            dir.ParseParameters("align=vertical", "border=true");
            Assert.AreEqual(ItemAlignment.Vertical, dir.Alignment);
            Assert.AreEqual(true, dir.ShowBorder);
            Assert.AreEqual(false, dir.ShowLabel);

            dir = new AlternateStyleDirective();
            dir.ParseParameters("border=false", "label=true");
            Assert.AreEqual(ItemAlignment.Vertical, dir.Alignment);
            Assert.AreEqual(false, dir.ShowBorder);
            Assert.AreEqual(true, dir.ShowLabel);
        }

        //TODO other directives
        [Test]
        public void TestGridDirective()
        {
            GridDirective grid = new GridDirective();
            Assert.AreEqual("grid", grid.Name);

            grid.ParseParameters("cellEditable", "detailedEditor","column{name=Systolic;path=/content/items[at0021]/items[at0023];showCell}", "composite{name=Cholesterol;separator=/;component=/content/items[at0018];component=/content/items[at0019]}");
            Assert.IsTrue(grid.CellEditable);
            Assert.IsTrue(grid.DetailedEditor);
            Assert.AreEqual(2, grid.Attributes.Count);
            ColumnDescriptor descriptor = grid.Attributes[0] as ColumnDescriptor;
            Assert.AreEqual("Systolic", descriptor.Name);
            Assert.AreEqual("/content/items[at0021]/items[at0023]", descriptor.ConstraintPath);
            Assert.IsTrue(descriptor.ShowCell);

            CompositeDescriptor composite = grid.Attributes[1] as CompositeDescriptor;
            Assert.AreEqual("Cholesterol", composite.Name);
            Assert.AreEqual("/", composite.Separator);
            Assert.AreEqual("/content/items[at0018]", composite.ComponentPaths[0]);
            Assert.AreEqual("/content/items[at0019]", composite.ComponentPaths[1]);
        }
    }
}
