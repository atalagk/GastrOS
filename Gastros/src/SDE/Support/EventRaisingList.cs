using System;
using System.Collections;
using System.Collections.Generic;

namespace GastrOs.Sde.Support
{
    public class ListEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }
        public int Index { get; private set; }
        public ListEventArgs(T item, int index)
        {
            Item = item;
            Index = index;
        }
    }

    public class EventRaisingList<T> : IList<T>
    {
        private List<T> backingList;

        /// <summary>
        /// Raised when an item has been added or inserted to this list
        /// </summary>
        public event EventHandler<ListEventArgs<T>> ItemAdded;
        /// <summary>
        /// Raised when an item was removed from this list (but not when
        /// the list is cleared)
        /// </summary>
        public event EventHandler<ListEventArgs<T>> ItemRemoved;
        /// <summary>
        /// Raised when the list has been cleared.
        /// </summary>
        public event EventHandler<EventArgs> ItemsCleared;

        public EventRaisingList()
        {
            backingList = new List<T>();
        }

        public EventRaisingList(int capacity)
        {
            backingList = new List<T>(capacity);
        }

        public EventRaisingList(IEnumerable<T> col)
        {
            backingList = new List<T>(col);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return backingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            backingList.Add(item);
            OnItemAdded(new ListEventArgs<T>(item, Count - 1));
        }

        public void Insert(int index, T item)
        {
            backingList.Insert(index, item);
            OnItemAdded(new ListEventArgs<T>(item, index));
        }

        public bool Remove(T item)
        {
            int index = backingList.IndexOf(item);
            if (backingList.Remove(item))
            {
                OnItemRemoved(new ListEventArgs<T>(item, index));
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            T item = backingList[index];
            backingList.RemoveAt(index);
            OnItemRemoved(new ListEventArgs<T>(item, index));
        }

        public void Clear()
        {
            backingList.Clear();
            OnItemsCleared(new EventArgs());
        }

        public bool Contains(T item)
        {
            return backingList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            backingList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return backingList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(T item)
        {
            return backingList.IndexOf(item);
        }

        public T this[int index]
        {
            get { return backingList[index]; }
            set { backingList[index] = value; }
        }

        protected virtual void OnItemAdded(ListEventArgs<T> e)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, e);
            }
        }

        protected virtual void OnItemRemoved(ListEventArgs<T> e)
        {
            if (ItemRemoved != null)
            {
                ItemRemoved(this, e);
            }
        }

        protected virtual void OnItemsCleared(EventArgs e)
        {
            if (ItemsCleared != null)
            {
                ItemsCleared(this, e);
            }
        }
    }
}