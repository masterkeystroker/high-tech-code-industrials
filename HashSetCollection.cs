using System.Collections.ObjectModel;
using System;

namespace CretaBase
{
    /// <summary>
    /// Funciona com un hashset donde no hay repeticiones
    /// Implementa notificaciones para el MVVM heredando de ObservableCollection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashSetCollection<T> : ObservableCollection<T>
    {
        protected override void InsertItem(int index, T item)
        {
            if (!Contains(item))
                base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            int i = IndexOf(item);
            bool bIsertError = (i >= 0 && i != index);
            if (!bIsertError)
                base.SetItem(index, item);
        }
    }

    /// <summary>
    /// Funciona como un hashset ordenado donde no hay repeticiones
    /// Implementa notificaciones para el MVVM heredando de ObservableCollection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortedHashSetCollection<T> : SortedObservableCollection<T> where T : IComparable
    {
        protected override void InsertItem(int index, T item)
        {
            if (!Contains(item))
            {
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
                base.InsertItem(index, item);
            }
        }

        protected override void SetItem(int index, T item)
        {
            int i = IndexOf(item);
            bool bIsertError = (i >= 0 && i != index);
            if (!bIsertError)
                base.SetItem(index, item);
        }
    }
}
