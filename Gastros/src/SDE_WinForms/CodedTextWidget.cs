using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Windows forms based coded text view implementation. Displays coded texts
    /// via combo boxes.
    /// </summary>
    public class CodedTextWidget : ElementWidgetBase, IListView
    {
        /// <summary>
        /// Compares OntologyItems by description, then by ID.
        /// </summary>
        class OrdinalIdBasedComparer : Comparer<OntologyItem>
        {
            public override int Compare(OntologyItem x, OntologyItem y)
            {
                if (y == null && x == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;

                int comp = x.Ordinal.CompareTo(y.Ordinal);
                if (comp == 0)
                {
                    if (x.ID == null && y.ID == null)
                        return 0;
                    if (x.ID == null)
                        return -1;
                    if (y.ID == null)
                        return 1;
                    return x.ID.CompareTo(y.ID);
                }
                return comp;
            }
        }

        public static readonly int[] ComboSizes = {
                                                      (GastrOsConfig.LayoutConfig.InputWidth - 5) / 3,
                                                      (GastrOsConfig.LayoutConfig.InputWidth - 5) * 2 / 3,
                                                      GastrOsConfig.LayoutConfig.InputWidth - 5
                                                  };

        private ComboBox combo;
        private ShowValueContextMode showValueContext;
        private IList<OntologyItem> choiceList;

        public CodedTextWidget()
        {
            combo = new ComboBox();
            combo.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.SelectedValueChanged += delegate { OnTextChanged(EventArgs.Empty); };
            combo.MouseWheel += SuppressMouseWheel;
            AddField(combo, 0, 100, SizeType.Percent);
        }

        public IList<OntologyItem> ChoiceList
        {
            get { return choiceList; }
            set
            {
                choiceList = value;
                UpdateComboDisplay();
            }
        }

        public override string Text
        {
            get
            {
                return combo.SelectedValue as string;
            }
            set
            {
                if (string.Equals(Text, value))
                    return;
                //this will implicitly fire OnTextChanged
                combo.SelectedValue = value;
            }
        }

        public ShowValueContextMode ShowValueContext
        {
            get
            {
                return showValueContext;
            }
            set
            {
                if (showValueContext == value)
                    return;
                showValueContext = value;
                UpdateComboDisplay();
            }
        }

        private void UpdateComboDisplay()
        {
            List<OntologyItem> dataSource = new List<OntologyItem>();

            if (showValueContext == ShowValueContextMode.Normal)
            {
                dataSource.AddRange(choiceList);
            }
            else
            {
                //Group ontology items by description. I.e. key = description, value = ontology items with same description
                //Descriptions are ordered by appearance
                OrderedDictionary groups = new OrderedDictionary();
                foreach (OntologyItem item in choiceList)
                {
                    //skip "dummy" value - add it to the beginning of data source list instead
                    if (string.Equals(item.ID, RmFactory.DummyCodedValue))
                    {
                        dataSource.Add(item);
                        continue;
                    }

                    List<OntologyItem> group = groups[item.Description] as List<OntologyItem>;
                    if (group == null)
                    {
                        group = new List<OntologyItem>();
                        groups[item.Description] = group;
                    }
                    group.Add(item);
                }
                //Now sort each group of ontology items by ordinal
                foreach (List<OntologyItem> group in groups.Values)
                {
                    group.Sort(new OrdinalIdBasedComparer());
                }

                //the data source to be fed to the combobox is set up differently
                //depending on the ShowValueContext value.
                if (showValueContext == ShowValueContextMode.Append)
                {
                    //In case of "Append", change the display text to show both
                    //description AND text from the ontology.
                    foreach (List<OntologyItem> group in groups.Values)
                    {
                        foreach (OntologyItem item in group)
                        {
                            dataSource.Add(new OntologyItem(item.ID)
                            {
                                Description = item.Description,
                                Ordinal = item.Ordinal,
                                Text =
                                    string.IsNullOrEmpty(item.Text)
                                        ? ""
                                        : "[" + item.Description + "] " + item.Text
                            });
                        }
                    }
                }
                else
                {
                    //In case of "Organise", add an extra organiser item in the beginning
                    //of each group
                    foreach (string description in groups.Keys)
                    {
                        dataSource.Add(new OntologyItem("")
                                           {Description = description, Text = "------ " + description + "------"});
                        dataSource.AddRange(groups[description] as IList<OntologyItem>);
                    }
                }
            }

            combo.DataSource = dataSource;
            combo.ValueMember = "ID";
            combo.DisplayMember = "Text";
            combo.SetBestFitWidthForCombo(ComboSizes);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs("Text"));
        }

        protected override int FieldCount
        {
            get { return 1; }
        }

        public override void Reset()
        {
            combo.SelectedIndex = 0;
        }
    }
}