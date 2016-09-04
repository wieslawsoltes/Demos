#region References

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Rx
{
    #region ReactiveExtensions

    public static class ReactiveExtensions
    {
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> FromCollectionChange(this INotifyCollectionChanged collection)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => (s, e) => h(s, e),
                        h => collection.CollectionChanged += h,
                        h => collection.CollectionChanged -= h);
        }

        public static IObservable<object> FromPropertyChange(this INotifyPropertyChanged propertyChange, string propertyName)
        {
            return Observable.FromEventPattern<PropertyChangedEventArgs>(propertyChange, "PropertyChanged")
                .Where(p => p.EventArgs.PropertyName == propertyName)
                .Select(p => p.Sender);
        }

        public static IObservable<T> ObserveAddedValues<T>(this ObservableCollection<T> collection)
        {
            return collection.FromCollectionChange()
                .Where(e => e.EventArgs.Action == NotifyCollectionChangedAction.Add)
                .SelectMany(e => e.EventArgs.NewItems.Cast<T>());
        }

        public static IObservable<T> ObserveRemovedValues<T>(this ObservableCollection<T> collection)
        {
            return collection.FromCollectionChange()
                .Where(e => e.EventArgs.Action == NotifyCollectionChangedAction.Remove)
                .SelectMany(e => e.EventArgs.OldItems.Cast<T>());
        }
    }

    #endregion
}
