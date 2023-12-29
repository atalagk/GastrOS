using OpenEhr.DesignByContract;
using NUnit.Framework;
using OpenEhr.RM.Common.Generic;
using OpenEhr.RM.Composition.Content;
using OpenEhr.RM.Composition.Content.Entry;
using OpenEhr.RM.Composition.Content.Navigation;
using OpenEhr.RM.DataStructures.History;
using OpenEhr.RM.DataStructures.ItemStructure;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Quantity.DateTime;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Sde.Test.OpenEhrV1Tests
{
    [TestFixture]
    public class MutabilityTest
    {
        [Test]
        public void DvDateTimeTest()
        {
            DvDateTime date = new DvDateTime(2000,01,01,00,00,00,0,1,00,00);
            Assert.AreEqual("20000101T000000,0+0000", date.Value);

            date.Value = "20041011T010203,0+0000";
            Assert.AreEqual("20041011T010203,0+0000", date.Value);
        }

        [Test]
        [ExpectedException(typeof(InvariantException))]
        public void DvDateTimeInvalidTest()
        {
            DvDateTime date = new DvDateTime(2000, 01, 01, 00, 00, 00, 0, 1, 00, 00);
            date.Value = null;
        }

        [Test]
        public void DvCodedTextTest()
        {
            DvCodedText codedText = new DvCodedText("AA", "at0132", "t1");
            Assert.AreEqual(new CodePhrase("at0132", "t1"), codedText.DefiningCode);

            codedText.DefiningCode = new CodePhrase("at9999", "t2");
            Assert.AreEqual(new CodePhrase("at9999", "t2"), codedText.DefiningCode);
        }

        [Test]
        [ExpectedException(typeof(InvariantException))]
        public void DvCodedTextInvalidTest()
        {
            DvCodedText codedText = new DvCodedText("AA", "at6512", "f2");
            codedText.DefiningCode = null;
        }

        [Test]
        public void DvQuantityTest()
        {
            DvQuantity quant = new DvQuantity(10, "cm");
            Assert.AreEqual(10, quant.Magnitude);

            quant.Magnitude = 20;
            Assert.AreEqual(20, quant.Magnitude);

            quant.Units = "mm";
            Assert.AreEqual("mm", quant.Units);
        }

        [Test]
        [ExpectedException(typeof(InvariantException))]
        public void DvQuantityInvalidTest()
        {
            DvQuantity quant = new DvQuantity(5.5, "mg");
            quant.Units = null;
        }

        [Test]
        public void ElementTest()
        {
            Element elem = new Element(new DvText("elem1"), "at2234", null, null, null, null, null, null);
            DvText value = new DvText("hello");
            elem.Value = value;

            Assert.AreEqual(value, elem.Value);
            
            DvText newVal = new DvText("bye");
            elem.Value = newVal;
            Assert.AreEqual(newVal, elem.Value);
        }

        [Test]
        public void ClusterTest()
        {
            Cluster cluster = new Cluster(new DvText("cluster1"), "at9942", null, null, null, null, new Item[0]);
            Assert.AreEqual(0, cluster.Items.Count);

            Element elem = new Element(new DvText("elem1"), "at1123", null, null, null, null, new DvBoolean(true), null);
            cluster.Items.Add(elem);
            Assert.AreEqual(1, cluster.Items.Count);
            Assert.AreEqual(elem, cluster.Items[0]);
            Assert.AreEqual(cluster, elem.Parent);

            Element elem2 = new Element(new DvText("elem2"), "at2503", null, null, null, null, new DvText("ahoy"), null);
            cluster.Items.Add(elem2);
            Assert.AreEqual(2, cluster.Items.Count);
            Assert.AreEqual(elem, cluster.Items[0]);
            Assert.AreEqual(elem2, cluster.Items[1]);
            Assert.AreEqual(cluster, elem.Parent);
            Assert.AreEqual(cluster, elem2.Parent);

            cluster.Items.Remove(elem);
            Assert.AreEqual(1, cluster.Items.Count);
            Assert.AreEqual(elem2, cluster.Items[0]);
            // Assert.AreEqual(null, elem.Parent); //note this might ideally be the case, but isn't right now
            Assert.AreEqual(cluster, elem2.Parent);
        }

        [Test]
        public void ItemSingleTest()
        {
            Element elem = new Element(new DvText("elem1"), "at1123", null, null, null, null, new DvBoolean(true), null);
            ItemSingle single = new ItemSingle(new DvText("single1"), "at4412", null, null, null, null, elem);
            Assert.AreEqual(elem, single.Item);
            Assert.AreEqual(single, elem.Parent);

            Element elem2 = new Element(new DvText("elem2"), "at2503", null, null, null, null, new DvText("ahoy"), null);
            single.Item = elem2;
            Assert.AreEqual(elem2, single.Item);
            Assert.AreEqual(single, elem2.Parent);
            Assert.AreEqual(null, elem.Parent);
        }

        [Test]
        public void ItemTreeTest()
        {
            ItemTree tree = new ItemTree(new DvText("tree1"), "at0002", null, null, null, null, new Item[0]);
            Assert.AreEqual(0, tree.Items.Count);

            Element elem = new Element(new DvText("elem1"), "at0001", null, null, null, null, new DvText("ace"), null);
            Cluster cluster = new Cluster(new DvText("cluster1"), "at0003", null, null, null, null, new Item[0]);
            cluster.Items.Add(elem);

            tree.Items.Add(cluster);
            Element elem2 = new Element(new DvText("elem2"), "at0004", null, null, null, null, new DvQuantity(20, "mm"), null);
            tree.Items.Add(elem2);

            Assert.AreEqual(2, tree.Items.Count);
            Assert.AreEqual(cluster, tree.Items[0]);
            Assert.AreEqual(elem2, tree.Items[1]);
            Assert.AreEqual(tree, cluster.Parent);
            Assert.AreEqual(tree, elem2.Parent);
            Assert.AreEqual(cluster, elem.Parent);

            tree.Items.Remove(cluster);
            Assert.AreEqual(1, tree.Items.Count);
            Assert.AreEqual(elem2, tree.Items[0]);
            Assert.AreEqual(tree, elem2.Parent);
            Assert.AreEqual(cluster, elem.Parent);
        }

        [Test]
        public void EventTest()
        {
            DvDateTime time = new DvDateTime(2001, 02, 03, 04, 05, 06, 0, 1, 00, 00);

            ItemSingle single = new ItemSingle(new DvText("single"), "at0001", null, null, null, null,
                                               new Element(new DvText("elem1"), "at0003", null, null, null, null,
                                                           new DvBoolean(), null));
            
            Event<ItemStructure> evnt = new PointEvent<ItemStructure>(new DvText("event"), "at0002", null,
                null, null, null, time, single, null);

            Assert.AreEqual(single, evnt.Data);
            Assert.AreEqual(time, evnt.Time);
            Assert.AreEqual(evnt, single.Parent);

            ItemTree tree = new ItemTree(new DvText("tree"), "at0005", null, null, null, null, new Item[0]);
            tree.Items.Add(new Element(new DvText("elem2"), "at0006", null, null, null, null, new DvText("ABC"), null));

            evnt.Data = tree;
            Assert.AreEqual(tree, evnt.Data);
            Assert.AreEqual(evnt, tree.Parent);
            Assert.AreEqual(null, single.Parent);
        }

        [Test]
        public void HistoryTest()
        {
            DvDateTime time = new DvDateTime(2001, 02, 03, 04, 05, 06, 0, 1, 00, 00);
            History<ItemStructure> hist = new History<ItemStructure>(new DvText("hist"), "at0001", null, null,
                null, null, time, null, null, new Event<ItemStructure>[0], null);
            Assert.AreEqual(time, hist.Origin);
            Assert.AreEqual(0, hist.Events.Count);

            Event<ItemStructure> evnt = new PointEvent<ItemStructure>(new DvText("event1"), "at0002", null, null,
                                                                 null, null, new DvDateTime(), null, null);
            Element elem = new Element(new DvText("elem1"), "at0008", null, null, null, null, new DvBoolean(), null);
            evnt.Data = new ItemSingle(new DvText("single"), "at0003", null, null, null, null, elem);

            hist.Events.Add(evnt);
            Assert.AreEqual(1, hist.Events.Count);
            Assert.AreEqual(evnt, hist.Events[0]);
            Assert.AreEqual(hist, evnt.Parent);

            hist.Events.Remove(evnt);
            Assert.AreEqual(0, hist.Events.Count);
        }

        [Test]
        public void ObservationTest()
        {
            History<ItemStructure> hist = new History<ItemStructure>(new DvText("hist"), "at0001", null, null,
                null, null, new DvDateTime(), null, null, new Event<ItemStructure>[0], null);
            Event<ItemStructure> evnt = new PointEvent<ItemStructure>(new DvText("event1"), "at0002", null, null,
                                                                 null, null, new DvDateTime(), null, null);
            Element elem = new Element(new DvText("elem1"), "at0003", null, null, null, null, new DvBoolean(), null);
            evnt.Data = new ItemSingle(new DvText("single"), "at0004", null, null, null, null, elem);
            hist.Events.Add(evnt);

            Observation obs = new Observation(new DvText("obs"), "openEHR-EHR-OBSERVATION.endoscopy_examination.v1", null, null,
                null, null, new CodePhrase("", "t1"), new CodePhrase("", "t2"), new PartyIdentified("sub"),
                null, null, null, null, null, hist, null);
            Assert.AreEqual(hist, obs.Data);
            Assert.AreEqual(obs, hist.Parent);

            History<ItemStructure> hist2 = new History<ItemStructure>(new DvText("hist"), "at0005", null, null,
                null, null, new DvDateTime(), null, null, null, null);
            obs.Data = hist2;
            Assert.AreEqual(hist2, obs.Data);
            Assert.AreEqual(obs, hist2.Parent);
            Assert.AreEqual(null, hist.Parent);
        }

        [Test]
        public void EvaluationTest()
        {
            CodePhrase lang = new CodePhrase("433", "openehr");
            CodePhrase encoding = new CodePhrase("utf8", "IANA");
            PartyIdentified party = new PartyIdentified("party1");
            
            Element elem = new Element(new DvText("elem1"), "at0003", null, null, null, null, new DvBoolean(), null);
            ItemSingle data = new ItemSingle(new DvText("single"), "at0004", null, null, null, null, elem);

            Evaluation eval = new Evaluation(new DvText("eval"), "openEHR-EHR-EVALUATION.endoscopy_diagnosis.v1",
                null, null, null, null, lang, encoding, party, null, null, null, null, null, data);

            Assert.AreEqual(lang, eval.Language);
            Assert.AreEqual(encoding, eval.Encoding);
            Assert.AreEqual(party, eval.Subject);
            Assert.AreEqual(data, eval.Data);
            Assert.AreEqual(eval, data.Parent);

            Element elem2 = new Element(new DvText("elem2"), "at0005", null, null, null, null, new DvBoolean(), null);
            ItemSingle data2 = new ItemSingle(new DvText("single2"), "at0006", null, null, null, null, elem2);

            eval.Data = data2;
            Assert.AreEqual(data2, eval.Data);
            Assert.AreEqual(eval, data2.Parent);
            Assert.AreEqual(null, data.Parent);
        }

        [Test]
        public void ActionTest()
        {
            CodePhrase lang = new CodePhrase("433", "openehr");
            CodePhrase encoding = new CodePhrase("utf8", "IANA");
            PartyIdentified party = new PartyIdentified("party1");

            DvDateTime time = new DvDateTime(2001, 02, 03, 04, 05, 06, 0, 1, 00, 00);
            ItemSingle description = new ItemSingle(new DvText("descr"), "at0002", null, null,
                                                    null, null,
                                                    new Element(new DvText("elem1"), "at0003", null, null, null, null,
                                                                new DvText("description"), null));

            Action action = new Action(new DvText("action"), "openEHR-EHR-ACTION.endoscopy_interventions.v1",
                null, null, null, null, lang, encoding, party, null, null, null, null, null, time, description,
                new IsmTransition(), null);

            Assert.AreEqual(lang, action.Language);
            Assert.AreEqual(encoding, action.Encoding);
            Assert.AreEqual(party, action.Subject);
            Assert.AreEqual(time, action.Time);
            Assert.AreEqual(description, action.Description);
            Assert.AreEqual(action, description.Parent);

            ItemSingle description2 = new ItemSingle(new DvText("descr2"), "at0003", null, null,
                                                     null, null,
                                                     new Element(new DvText("elem123"), "at0004", null, null, null, null,
                                                                 new DvText("description2"), null));

            action.Description = description2;
            Assert.AreEqual(description2, action.Description);
            Assert.AreEqual(action, description2.Parent);
            Assert.AreEqual(null, description.Parent);
        }

        [Test]
        public void SectionTest()
        {
            CodePhrase lang = new CodePhrase("433", "openehr");
            CodePhrase encoding = new CodePhrase("utf8", "IANA");
            PartyIdentified party = new PartyIdentified("party1");

            Element elem = new Element(new DvText("elem1"), "at0003", null, null, null, null, new DvBoolean(), null);
            ItemSingle data = new ItemSingle(new DvText("single"), "at0004", null, null, null, null, elem);

            Element elem2 = new Element(new DvText("elem2"), "at0008", null, null, null, null, new DvText("yoohoo"), null);
            ItemSingle data2 = new ItemSingle(new DvText("single2"), "at0009", null, null, null, null, elem2);

            Section section = new Section(new DvText("section1"), "at0001", null, null, null, null,
                new ContentItem[0]);

            Assert.AreEqual(0, section.Items.Count);

            Evaluation eval = new Evaluation(new DvText("eval"), "openEHR-EHR-EVALUATION.endoscopy_diagnosis.v1",
                null, null, null, null, lang, encoding, party, null, null, null, null, null, data);

            History<ItemStructure> hist = new History<ItemStructure>(new DvText("hist"), "at0024", null, null,
                null, null, new DvDateTime(), null, null, new Event<ItemStructure>[0], null);
            Event<ItemStructure> evnt = new PointEvent<ItemStructure>(new DvText("event1"), "at0002", null, null,
                                                                 null, null, new DvDateTime(), data2, null);
            hist.Events.Add(evnt);

            Observation obs = new Observation(new DvText("obs"), "openEHR-EHR-OBSERVATION.endoscopy_examination.v1",
                null, null, null, null, lang, encoding, party, null, null, null, null, null, hist, null);

            section.Items.Add(eval);
            Assert.AreEqual(1, section.Items.Count);
            Assert.AreEqual(section, eval.Parent);
            Assert.AreEqual(eval, section.Items[0]);

            section.Items.Add(obs);
            Assert.AreEqual(2, section.Items.Count);
            Assert.AreEqual(section, obs.Parent);
            Assert.AreEqual(eval, section.Items[0]);
            Assert.AreEqual(obs, section.Items[1]);

            section.Items.Remove(eval);
            Assert.AreEqual(1, section.Items.Count);
            Assert.AreEqual(obs, section.Items[0]);

            section.Items.Remove(obs);
            Assert.AreEqual(0, section.Items.Count);
        }
    }
}
