namespace GastrOs.Wrapper.Reports.Representation
{
    /// <summary>
    /// Represents an MST attribute. Basically can be used to represent any
    /// name-value mapping
    /// </summary>
    public class Attribute : MstItem
    {
        private string value;

        public Attribute(string attrName, string value) : base(attrName)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", DisplayName, Value);
        }

        /*
        public override bool Equals(object obj)
        {
            Attribute other = obj as Attribute;
            if (other == null)
                return false;
            return base.Equals(other) && string.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }
        */
    }
}