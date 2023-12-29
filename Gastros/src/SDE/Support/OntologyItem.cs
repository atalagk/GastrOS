namespace GastrOs.Sde.Support
{
    public class OntologyItem
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public float Ordinal { get; set; }
        public string Description { get; set; }
        public string Annotation { get; set; }
        public OntologyItem(string id)
        {
            ID = id;
            Text = "";
            Description = "";
            Annotation = "";
        }
        public override bool Equals(object obj)
        {
            OntologyItem other = obj as OntologyItem;
            if (other == null)
                return false;
            return ID.Equals(other.ID) && Text.Equals(other.Text) && Ordinal == other.Ordinal &&
                   Description.Equals(other.Description) && Annotation.Equals(other.Annotation);
        }
        public override int GetHashCode()
        {
            int hash = 31 * ID.GetHashCode();
            hash = hash * 31 ^ Text.GetHashCode();
            hash = hash * 31 ^ Ordinal.GetHashCode();
            hash = hash * 31 ^ Description.GetHashCode();
            hash = hash * 31 ^ Annotation.GetHashCode();
            return hash;
        }
        public override string ToString()
        {
            return string.Format("[{0}] = <text: \"{1}\", description: \"{2}\">", ID, Text, Description);
        }
    }
}