using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GastrOs.Sde.Directives;
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
    }
}
