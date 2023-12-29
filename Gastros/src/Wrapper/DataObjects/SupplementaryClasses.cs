using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;

namespace GastrOs.Wrapper.DataObjects
{
    public class Department : DataObject
    {
        public virtual long ID { get; set; }
        public virtual string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    internal class OrganOrdinalComparer : IComparer<KeyValuePair<SdeConcept, int>>
    {
        public int Compare(KeyValuePair<SdeConcept, int> x, KeyValuePair<SdeConcept, int> y)
        {
            return x.Value.CompareTo(y.Value);
        }
    }

    public class EndoscopyType : DataObject
    {
        public virtual long ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string OpTemplate { get; set; }
        public virtual IDictionary<SdeConcept, int> OrganOrdinals { get; set; }
        public override string ToString()
        {
            return Name;
        }
        public virtual IEnumerable<SdeConcept> OrgansSortedByOrdinal
        {
            get
            {
                List<KeyValuePair<SdeConcept, int>> list = OrganOrdinals.ToList();
                list.Sort(new OrganOrdinalComparer());
                return list.Select(kvp => kvp.Key);
            }
        }
    }

    public class SdeConcept : DataObject
    {
        public virtual long ID { get; set; }
        public virtual string Term { get; set; }
        public virtual SdeConcept Parent { get; set; }
        public virtual ISet<SdeConcept> Children { get; set; }
        public virtual string ArchetypeId { get; set; }
        public override string ToString()
        {
            return Term;
        }
    }

    public class EndoscopyDevice : DataObject
    {
        public virtual long ID { get; set; }
        public virtual string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class SignoutEndoscopist : DataObject
    {
        public virtual long ID { get; set; }
        public virtual string Name { get; set; }
        public override bool Equals(object obj)
        {
            SignoutEndoscopist other = obj as SignoutEndoscopist;
            if (other != null)
            {
                return ID == other.ID && string.Equals(Name, other.Name);
            }
            return false;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
