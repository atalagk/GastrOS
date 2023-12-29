using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views;

namespace GastrOs.Sde.Test.GuiTests
{
    public class MockView : IView
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler NewInstanceRequest;
        public event EventHandler RemoveInstanceRequest;
        public event EventHandler TitleChanged;
        public event EventHandler VisibleChanged;
        public event EventHandler SizeChanged;
        public event EventHandler Disposed;

        private string title = "";
        private bool visible = true;
        private List<IDirective> directives;

        private Size size;

        public string Title
        {
            get { return title; }
            set
            {
                if (string.Equals(title, value))
                    return;
                title = value;
                OnTitleChanged(EventArgs.Empty);
            }
        }

        public virtual bool Visible
        {
            get { return visible; }
            set {
                if (visible == value)
                    return;
                visible = value;
                OnVisibleChanged(EventArgs.Empty);
            }
        }

        public Size Size
        {
            get { return size; }
            set
            {
                if (size == value)
                    return;
                size = value;
                OnSizeChanged(EventArgs.Empty);
            }
        }

        public virtual Size IdealSize
        {
            get { return Size; }
        }

        public IList<IDirective> Directives
        {
            get
            {
                if (directives == null)
                {
                    directives = new List<IDirective>();
                }
                return directives;
            }
        }

        public virtual bool RequiresExternalScrolling
        {
            get { return true; }
        }

        public bool CanAddNewInstance
        {
            get; set;
        }

        public bool CanRemoveInstance
        {
            get; set;
        }

        protected virtual void OnTitleChanged(EventArgs e)
        {
            if (TitleChanged != null)
            {
                TitleChanged(this, e);
            }
        }

        protected virtual void OnVisibleChanged(EventArgs e)
        {
            if (VisibleChanged != null)
            {
                VisibleChanged(this, e);
            }
        }

        protected virtual void OnSizeChanged(EventArgs e)
        {
            if (SizeChanged != null)
            {
                SizeChanged(this, e);
            }
        }

        public void TriggerNewInstance()
        {
            if (NewInstanceRequest != null)
            {
                NewInstanceRequest(this, EventArgs.Empty);
            }
        }

        public void TriggerRemoveInstance()
        {
            if (RemoveInstanceRequest != null)
            {
                RemoveInstanceRequest(this, EventArgs.Empty);
            }   
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public void Dispose()
        {
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }
    }

    public class MockTextView : MockView, ITextView
    {
        public event EventHandler TextChanged;

        private string text = "";

        public string Text
        {
            get { return text; }
            set
            {
                if (string.Equals(text, value))
                    return;
                text = value;
                OnTextChanged(EventArgs.Empty);
            }
        }

        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Text"));
        }
    }

    public class MockCodedTextView : MockTextView, ICodedTextView
    {
        private IList<OntologyItem> choiceList;
        private ShowValueContextMode showValueContext;

        public IList<OntologyItem> ChoiceList
        {
            get { return choiceList; }
            set { choiceList = value; }
        }

        public ShowValueContextMode ShowValueContext
        {
            get { return showValueContext; }
            set { showValueContext = value; }
        }
    }

    public class MockNumericView : MockView, INumericView
    {
        public event EventHandler ValueChanged;

        private decimal? value;
        private decimal minValue = decimal.MinValue, maxValue = decimal.MaxValue;

        public decimal? Value
        {
            get { return value; }
            set
            {
                if (this.value == value)
                    return;
                if (value > maxValue || value < minValue)
                    return;
                this.value = value;
                OnNumericChanged(EventArgs.Empty);
            }
        }

        protected virtual void OnNumericChanged(EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Value"));
        }

        public decimal MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }

        public decimal MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
    }

    public class MockNumericUnitView : MockNumericView, INumericUnitView
    {
        public event EventHandler UnitChanged;

        private string unit;
        private IList<string> availableUnits;

        public string Unit
        {
            get { return unit; }
            set
            {
                if (string.Equals(unit, value))
                    return;
                unit = value;
                OnUnitChanged(EventArgs.Empty);
            }
        }

        public IList<string> AvailableUnits
        {
            get { return availableUnits; }
            set { availableUnits = value; }
        }

        protected virtual void OnUnitChanged(EventArgs e)
        {
            if (UnitChanged != null)
            {
                UnitChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Unit"));
        }
    }

    public class MockContainerView : MockView, IContainerView
    {
        public event EventHandler<ChildEventArgs> ChildAdded;
        public event EventHandler<ChildEventArgs> ChildRemoved;

        private EventRaisingList<IView> children;

        public IList<IView> Children
        {
            get
            {
                if (children == null)
                {
                    children = new EventRaisingList<IView>();
                    children.ItemAdded += ItemAdded;
                    children.ItemRemoved += ItemRemoved;
                }
                return children;
            }
        }

        public bool Framed
        {
            get; set;
        }

        private void ItemAdded(object sender, ListEventArgs<IView> e)
        {
            OnChildAdded(new ChildEventArgs(e.Item));
        }

        private void ItemRemoved(object sender, ListEventArgs<IView> e)
        {
            OnChildRemoved(new ChildEventArgs(e.Item));
        }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
                foreach (IView view in Children)
                {
                    view.Visible = value;
                }
            }
        }

        protected virtual void OnChildAdded(ChildEventArgs e)
        {
            if (ChildAdded != null)
            {
                ChildAdded(this, e);
            }
        }

        protected virtual void OnChildRemoved(ChildEventArgs e)
        {
            if (ChildRemoved != null)
            {
                ChildRemoved(this, e);
            }
        }
    }

    public class MockCoreConceptView : MockContainerView, ICoreConceptView
    {
        public event EventHandler PresenceChanged;

        private PresenceState presence;
        private List<IView> sideChildren = new List<IView>();

        public void SetAvailablePresenceStates(PresenceState states)
        {
        }

        public PresenceState Presence
        {
            get { return presence; }
            set
            {
                if (presence == value)
                    return;
                presence = value;
                OnPresenceChanged(EventArgs.Empty);
            }
        }

        public void AddChildToSide(IView child)
        {
            sideChildren.Add(child);
        }

        protected virtual void OnPresenceChanged(EventArgs e)
        {
            if (PresenceChanged != null)
            {
                PresenceChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Present"));
        }
    }

    public class MockMultiChoiceView : MockView, IMultiChoiceView
    {
        public event EventHandler<SelectionEventArgs> ItemSelectionChanged;

        private ToString displayFunction = obj => obj.ToString();

        private Dictionary<object, bool> selectedMap = new Dictionary<object, bool>();
        private bool canSelectMore;

        public void SetDisplayFunction(ToString func)
        {
            displayFunction = func;
        }

        public bool AddSelectableItem(object item)
        {
            if (selectedMap.ContainsKey(item))
                return false;
            selectedMap[item] = false;
            return true;
        }

        public bool RemoveSelectableItem(object item)
        {
            if (!selectedMap.ContainsKey(item))
                return false;
            selectedMap.Remove(item);
            return true;
        }

        public IEnumerable<object> SelectedItems
        {
            get
            {
                return selectedMap.Where(keyValue => keyValue.Value).Select(kv => kv.Key);
            }
        }

        public bool SetSelected(object item, bool selected)
        {
            if (selectedMap.ContainsKey(item) && selectedMap[item] == selected)
                return false;
            selectedMap[item] = selected;
            OnItemSelectionChanged(new SelectionEventArgs(item, selected));
            return true;
        }

        public bool CanSelectMore
        {
            get { return canSelectMore; }
            set { canSelectMore = value; }
        }

        public string DisplayValueFor(object item)
        {
            return displayFunction(item);
        }

        protected virtual void OnItemSelectionChanged(SelectionEventArgs e)
        {
            if (ItemSelectionChanged != null)
            {
                ItemSelectionChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
        }
    }

    public class MockSplasherView<T> : MockView, ISplasherView<T> where T : IView
    {
        public event EventHandler SplashOpened;
        public event EventHandler SplashClosed;

        private T splashedView;
        
        public T SplashedView
        {
            get { return splashedView; }
            set
            {
                splashedView = value;
            }
        }

        public void OpenSplash()
        {
            splashedView.Visible = true;
            OnSplashOpen(EventArgs.Empty);
        }

        public void CloseSplash()
        {
            splashedView.Visible = false;
            OnSplashClose(EventArgs.Empty);
        }

        protected virtual void OnSplashOpen(EventArgs e)
        {
            if (SplashOpened != null)
            {
                SplashOpened(this, e);
            }
        }

        protected virtual void OnSplashClose(EventArgs e)
        {
            if (SplashClosed != null)
            {
                SplashClosed(this, e);
            }
        }
    }
}
