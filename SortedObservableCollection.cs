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
    public class SortedObservableCollection<T> : ObservableCollection<T>
    where T : IComparable
    {
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
