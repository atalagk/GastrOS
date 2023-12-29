namespace GastrOs.Wrapper.Reports.Representation
{
    /// <summary>
    /// Represents the most basic MST item - has a display text
    /// </summary>
    public class MstItem
    {
        public string DisplayName { get; set; }
        public MstItem(string displayName)
        {
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}