using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;

namespace PRBD_Framework
{
    public class MyObservableCollection<T> : ObservableCollection<T> where T : class
    {
        private bool refresh = false;
        private DbSet<T> model = null;

        public bool ReadOnly
        {
            get { return model == null; }
        }

        public MyObservableCollection(DbSet<T> collection) : base(collection)
        {
            this.model = collection;
            this.CollectionChanged += MyObservableCollection_CollectionChanged;
        }

        public MyObservableCollection(IEnumerable<T> query, DbSet<T> collection = null) : base(query)
        {
            this.model = collection;
            if (model != null)
                this.CollectionChanged += MyObservableCollection_CollectionChanged;
        }

        private void MyObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (refresh)
                return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T m in e.NewItems)
                        model.Add(m);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (T m in e.OldItems)
                        model.Remove(m);
                    break;
            }
        }

        public void Refresh(DbSet<T> collection)
        {
            model = collection;
            refresh = true;
            Clear();
            foreach (var o in model)
                Add(o);
            refresh = false;
        }
    }
}
