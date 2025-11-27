using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Specialized;

namespace CretaBase
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        #region Resolución del problema de los Threads
        // Override the event so this class can access it
        public override event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Be nice - use BlockReentrancy like MSDN said
            using (BlockReentrancy())
            {
                System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
                if (eventHandler == null)
                    return;

                Delegate[] delegates = eventHandler.GetInvocationList();
                // Walk thru invocation list
                foreach (System.Collections.Specialized.NotifyCollectionChangedEventHandler handler in delegates)
                {
                    DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                    // If the subscriber is a DispatcherObject and different thread
                    if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                    {
                        // Invoke handler in the target dispatcher's thread
                        dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                    }
                    else // Execute handler as is
                        handler(this, e);
                }
            }
        }
        #endregion
      
        #region Sorting

        
        #endregion // Sorting
    }

    public class SortedObservableCollectionEx<T> : ObservableCollection<T>
    where T : IComparable
    {
        #region Resolución del problema de los Threads
        // Override the event so this class can access it
        public override event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Be nice - use BlockReentrancy like MSDN said
            using (BlockReentrancy())
            {
                System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
                if (eventHandler == null)
                    return;

                Delegate[] delegates = eventHandler.GetInvocationList();
                // Walk thru invocation list
                foreach (System.Collections.Specialized.NotifyCollectionChangedEventHandler handler in delegates)
                {
                    DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                    // If the subscriber is a DispatcherObject and different thread
                    if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                    {
                        // Invoke handler in the target dispatcher's thread
                        dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                    }
                    else // Execute handler as is
                        handler(this, e);
                }
            }
        }
        #endregion

        protected override void InsertItem(int index, T item)
        {
            // If list is empty, no need to find insertion point.
            if (this.Count > 0)
            {
                // If item to insert is > last item just add it to the end
                if (Math.Sign(item.CompareTo(this[this.Count - 1])) == 1)
                {
                    base.InsertItem(this.Count, item);
                    return;
                }
                for (int i = 0; i < this.Count; i++)
                {
                    switch (Math.Sign(this[i].CompareTo(item)))
                    {
                        case 0:
                            //throw new InvalidOperationException("Cannot insert duplicate items");
                            base.InsertItem(i, item);
                            return;
                        case 1:
                            base.InsertItem(i, item);
                            return;
                        case -1: break;
                    }
                }
            }
            base.InsertItem(this.Count, item);
        }
    }
}
