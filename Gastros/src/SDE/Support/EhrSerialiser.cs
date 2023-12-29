using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Engine;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.Impl;
using OpenEhr.Serialisation;
using XmlSerializer=System.Xml.Serialization.XmlSerializer;

namespace GastrOs.Sde.Support
{
    public static class EhrSerialiser
    {
        private static string _knowledgePath = @".";

        public static string KnowledgePath
        {
            get
            {
                return _knowledgePath;
            }
            set
            {
                _knowledgePath = value;
            }
        } 

        public static string FindResource(string fileName)
        {
            if (new FileInfo(fileName).Exists)
                return fileName;
            DirectoryInfo d = new DirectoryInfo(KnowledgePath);
            if (!d.Exists)
            {
                Console.Error.WriteLine("Warning: knowledge path "+KnowledgePath+" doesn't exist");
                return null;
            }   
            FileInfo[] files = d.GetFiles(fileName, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                return files[0].FullName;
            }
            return null;
        }

        public static T Load<T>(string fileName) where T : class
        {
            string actualFile = FindResource(fileName);
            if (actualFile == null)
                return null;

            XmlSerializer deserialiser = new XmlSerializer(
                typeof(T),
                new XmlAttributeOverrides(),
                new Type[] { },
                new XmlRootAttribute("template"),
                "http://schemas.openehr.org/v1");

            using (XmlReader reader = XmlReader.Create(actualFile))
            {
                T ot = (T)deserialiser.Deserialize(reader);
                Check.Ensure(ot != null);
                return ot;
            }
        }

        public static void LoadValueInstance<T>(T vInstance, string fileName) where T : RmType
        {
            string actualFile = FindResource(fileName);
            Check.Require(!String.IsNullOrEmpty(actualFile));

            LoadValueInstance(vInstance, new StreamReader(actualFile));
        }

        public static void LoadValueInstance<T>(T instance, TextReader textReader) where T :RmType
        {
            using (XmlReader reader = XmlReader.Create(textReader))
            {
                (instance as IXmlSerializable).ReadXml(reader);
            }
        }

        public static void LoadFromXmlString(RmType obj, string xmlString)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (xmlString == null)
                throw new ArgumentNullException("xmlString");
            LoadValueInstance(obj, new StringReader(xmlString));
        }

        public static string SaveToXml(object obj)
        {
            StringBuilder builder = new StringBuilder();
            if (obj is IXmlSerializable)
            {
                XmlWriterSettings settings = new XmlWriterSettings {ConformanceLevel = ConformanceLevel.Auto};
                using (XmlWriter writer = XmlWriter.Create(builder, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("valueinstance", RmXmlSerializer.OpenEhrNamespace);
                    (obj as IXmlSerializable).WriteXml(writer);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Prunes given value instance and returns its XML representation.
        /// Returns an empty string if the pruned value instance is essentially
        /// empty.
        /// </summary>
        /// <param name="valueInstance"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static string PruneAndSave(Locatable valueInstance, CComplexObject constraint)
        {
            if (valueInstance == null)
                throw new ArgumentNullException("valueInstance");
            if (constraint == null)
                throw new ArgumentNullException("constraint");
            if (!valueInstance.LightValidate(constraint))
                throw new ArgumentException("Given reference model value instance must conform to given constraint");
            if (InstancePruner.Prune(valueInstance, constraint))
                return "";
            return SaveToXml(valueInstance);
        }
    }
}