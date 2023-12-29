using OpenEhr.AM.Archetype.ConstraintModel;

namespace GastrOs.Sde.Support
{
    public class AttributeDescriptor
    {
        public string Name { get; set; }
        public AttributeDescriptor(string name)
        {
            Name = name;
        }
    }

    public class ColumnDescriptor : AttributeDescriptor
    {
        public bool ShowCell { get; set; }
        public CComplexObject Constraint { get; set; }
        public string ConstraintPath { get; set; }

        public ColumnDescriptor(string name, bool showCell, CComplexObject constraint, string path)
            : base(name)
        {
            ShowCell = showCell;
            Constraint = constraint;
            ConstraintPath = path;
        }
    }

    public class CompositeDescriptor : AttributeDescriptor
    {
        public CComplexObject[] Components { get; set; }
        public string[] ComponentPaths { get; set; }
        public string Separator { get; set; }

        public CompositeDescriptor(string name, CComplexObject[] components, string[] componentPaths, string separator)
            : base(name)
        {
            Components = components;
            ComponentPaths = componentPaths;
            Separator = separator;
        }
    }
}
