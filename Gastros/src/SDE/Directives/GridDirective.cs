using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GastrOs.Sde.Support;
using OpenEhr.DesignByContract;

namespace GastrOs.Sde.Directives
{
    public class GridDirective : IDirective
    {
        private const string PcellEditable = "cellEditable";
        private const string PdetailedEditor = "detailedEditor";

        private static readonly Regex columnExpr = new Regex(@"column\{name=([^\{\}=;\(\),]+);path=([^\{\}=;\(\),]+)(?:;(showCell))?\}", RegexOptions.IgnoreCase);
        private static readonly Regex compositeExpr = new Regex(@"composite\{name=([^\{\}=;\(\),]+);separator=([^\{\}=;\(\),]+)(?:;component=([^\{\}=;\(\),]+))+\}", RegexOptions.IgnoreCase);

        private bool cellEditable;
        private bool detailedEditor;
        private IList<AttributeDescriptor> attributes = new List<AttributeDescriptor>();

        public bool DetailedEditor
        {
            get { return detailedEditor; }
        }

        public bool CellEditable
        {
            get { return cellEditable; }
        }

        public IList<AttributeDescriptor> Attributes
        {
            get { return attributes; }
        }

        public string Name
        {
            get { return "grid"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            attributes.Clear();

            foreach (string param in parameters)
            {
                if (Equals(param, PcellEditable))
                {
                    cellEditable = true;
                    continue;
                }
                if (Equals(param, PdetailedEditor))
                {
                    detailedEditor = true;
                    continue;
                }
            
                Match match = columnExpr.Match(param);
                if (match.Success)
                {
                    Check.Assert(match.Groups.Count == 4);
                    //first capturing group = 'name'
                    string colName = match.Groups[1].Value;
                    //second capturing group = 'path'
                    string colPath = match.Groups[2].Value;
                    //third capturing group = 'showCell' (optional)
                    bool showCell = match.Groups[3].Success;
                    attributes.Add(new ColumnDescriptor(colName, showCell, null, colPath));
                    continue;
                }

                match = compositeExpr.Match(param);
                if (match.Success)
                {
                    Check.Assert(match.Groups.Count == 4);
                    //first capturing group = 'name'
                    string name = match.Groups[1].Value;
                    //second capturing group = 'separator'
                    string separator = match.Groups[2].Value;
                    //third capturing group = components (multiple)
                    string[] components = new string[match.Groups[3].Captures.Count];
                    for (int i = 0; i < match.Groups[3].Captures.Count; i++ )
                    {
                        components[i] = match.Groups[3].Captures[i].Value;
                    }
                    attributes.Add(new CompositeDescriptor(name, null, components, separator));
                }
            }
        }
    }
}
