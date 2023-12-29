using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views.WinForms.Properties;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Views.WinForms.Generic
{
    /// <summary>
    /// A special "checkbox" that allows four rotating states:
    /// non-data, present, unknown and absent.
    /// </summary>
    public partial class MultiStateCheck : UserControl
    {
        public event EventHandler StateChanged;

        private Dictionary<PresenceState, PresenceState> nextStates = new Dictionary<PresenceState, PresenceState>();
        private Dictionary<PresenceState, Image> icons = new Dictionary<PresenceState, Image>();

        private PresenceState state;
        private ToolTip toolTip;

        public MultiStateCheck()
        {
            //Initialise icon mappings
            icons[PresenceState.Null] = Resources.empty;
            icons[PresenceState.Present] = Resources.green_check;
            icons[PresenceState.Unknown] = Resources.blue_question;
            icons[PresenceState.Absent] = Resources.red_X;

            InitializeComponent();
            toolTip = new ToolTip();

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(153, 70);
            checkButton.ContextMenuStrip = contextMenuStrip;

            SetAvailableStates(PresenceState.Present | PresenceState.Unknown | PresenceState.Absent | PresenceState.Null);
            checklabel.TextChanged += ChainTextChanged;
            checkButton.Click += delegate { MoveToNextState(); };
        }

        /// <summary>
        /// Sets the available checked presence states for this control.
        /// If the current state isn't in the new available state-set, then
        /// defaults to "Null"
        /// Note only certain states are allowed:
        /// <list type="ordered">
        /// <item>Present, Null</item>
        /// <item>Present, Absent, Null</item>
        /// <item>Present, Unknown, Null</item>
        /// <item>Present, Absent, Unknown, Null</item>
        /// </list>
        /// </summary>
        /// <param name="states"></param>
        /// <exception cref="ArgumentException">If an invalid combination of states is passed</exception>
        public void SetAvailableStates(PresenceState states)
        {
            //Initialise the state transitions depending on the available states
            //Also setup context menu depending on available states
            switch (states)
            {
                case PresenceState.Present | PresenceState.Null:
                    nextStates.Clear();
                    nextStates[PresenceState.Null] = PresenceState.Present;
                    nextStates[PresenceState.Present] = PresenceState.Null;
                    break;
                case PresenceState.Present | PresenceState.Absent | PresenceState.Null:
                    nextStates.Clear();
                    nextStates[PresenceState.Null] = PresenceState.Present;
                    nextStates[PresenceState.Present] = PresenceState.Absent;
                    nextStates[PresenceState.Absent] = PresenceState.Present;
                    break;
                case PresenceState.Present | PresenceState.Unknown | PresenceState.Null:
                    nextStates.Clear();
                    nextStates[PresenceState.Null] = PresenceState.Present;
                    nextStates[PresenceState.Present] = PresenceState.Unknown;
                    nextStates[PresenceState.Unknown] = PresenceState.Present;
                    break;
                case PresenceState.Present | PresenceState.Unknown | PresenceState.Absent | PresenceState.Null:
                    nextStates.Clear();
                    nextStates[PresenceState.Null] = PresenceState.Present;
                    nextStates[PresenceState.Present] = PresenceState.Unknown;
                    nextStates[PresenceState.Unknown] = PresenceState.Absent;
                    nextStates[PresenceState.Absent] = PresenceState.Present;
                    break;
                default:
                    throw new ArgumentException("The states "+states+" are not valid for this checked control");
            }
            if (!nextStates.ContainsKey(state))
                State = PresenceState.Null; //we know this is always there
        }

        /// <summary>
        /// Returns or sets the current state
        /// </summary>
        public PresenceState State
        {
            get
            {
                return state;
            }
            set
            {
                if (!nextStates.ContainsKey(value))
                    throw new ArgumentException("'"+value+"' is not a valid state for this checked control.");
                if (state == value)
                    return;
                state = value;
                //update button icon
                checkButton.Image = icons[state];
                OnStateChanged(EventArgs.Empty);
            }
        }

        public override string Text
        {
            get
            {
                return checklabel.Text;
            }
            set
            {
                //This will implicitly chain the TextChanged event
                checklabel.Text = value;
            }
        }

        /// <summary>
        /// You can set/get the context menu for this multi-state checkbox.
        /// By default has an empty menu.
        /// </summary>
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return checkButton.ContextMenuStrip;
            }
            set
            {
                checkButton.ContextMenuStrip = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return toolTip.GetToolTip(checklabel);
            }
            set
            {
                toolTip.SetToolTip(checklabel, value);
                toolTip.SetToolTip(checkButton, value);
            }
        }

        /// <summary>
        /// Rotates to the next logical state and updates the icon
        /// </summary>
        /// <returns></returns>
        public void MoveToNextState()
        {
            State = nextStates[state];
        }
        
        protected virtual void OnStateChanged(EventArgs e)
        {
            if (StateChanged != null)
                StateChanged(this, e);
        }

        private void ChainTextChanged(object sender, EventArgs e)
        {
            checklabel.Size = checklabel.GetSizeToFitText(GastrOsConfig.LayoutConfig.CoreConceptCheckWidth);
            //adjust size according to text
            ClientSize = AdjustedSize();
        }

        private Size AdjustedSize()
        {
            Size checkButtonMargin = checkButton.Size + checkButton.Margin.Size;
            Size total = checklabel.Size + new Size(checklabel.Location);
            return new Size(Math.Max(total.Width, checkButtonMargin.Width), Math.Max(total.Height, checkButtonMargin.Height));
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            return AdjustedSize();
        }
    }
}