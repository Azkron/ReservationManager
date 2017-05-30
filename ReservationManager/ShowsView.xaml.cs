using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReservationManager
{
    /// <summary>
    /// Interaction logic for ShowsView.xaml
    /// </summary>
    public partial class ShowsView : UserControlBase
    {

        public ICommand ClearFilter { get; set; }

        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public bool IsValid { get; set; }


        public ShowsView()
        {
            InitializeComponent();

            DataContext = this;

            Shows = new MyObservableCollection<Show>(App.Model.Shows);

            ClearFilter = new RelayCommand(() => { NameFilter = ""; DateFilter = null; });
            NewCommand = new RelayCommand(() => { App.Messenger.NotifyColleagues(App.MSG_NEW_SHOW);});
            EditCommand = new RelayCommand<Show>((s) => { App.Messenger.NotifyColleagues(App.MSG_EDIT_SHOW, s); });

            ReadOnly = App.Rights(Table.SHOW) != Right.ALL;

            RefreshCommand = new RelayCommand(() =>
            {
                //App.CancelChanges();
                Shows.Refresh(App.Model.Shows);
            },
            () => { return IsValid; });

            
            App.Messenger.Register(App.MSG_REFRESH, () =>
            {
                this.Shows.Refresh(App.Model.Shows);
            });
        }

        private bool readOnly;
        public bool ReadOnly
        {
            get { return readOnly; }

            set
            {
                readOnly = value;
                if (readOnly)
                    btnNew.Visibility = Visibility.Collapsed;

                RaisePropertyChanged(nameof(ReadOnly));
            }
        }

        public MyObservableCollection<Show> shows;
        public MyObservableCollection<Show> Shows
        {
            get { return shows; }
            set
            {
                shows = value;
                RaisePropertyChanged(nameof(Shows));
            }
        }

        Show selectedShow;
        public Show SelectedShow
        {
            get { return selectedShow; }
            set
            {
                selectedShow = value;
                Console.WriteLine(selectedShow);
            }
        }

        private string nameFilter;
        public string NameFilter
        {
            get { return nameFilter; }
            set
            {
                nameFilter = value;
                ApplyFilterAction();
                RaisePropertyChanged(nameof(NameFilter));
                // this changes the content of filter in view if it was changed in the code (using clear() for instance)
            }
        }

        private DateTime? dateFilter;
        public DateTime? DateFilter
        {
            get { return dateFilter; }
            set
            {
                Console.WriteLine("DateFilterSet");
                dateFilter = value;
                ApplyFilterAction();
                RaisePropertyChanged(nameof(DateFilter));
                // this changes the content of filter in view if it was changed in the code (using clear() for instance)
            }
        }


        private void ApplyFilterAction()
        {
            if (!string.IsNullOrEmpty(NameFilter) || DateFilter != null)
            {
                IQueryable<Show> filtered = App.Model.Shows;

                if(DateFilter != null)
                {
                    filtered = from m in filtered
                               where DateTime.Compare(m.Date, (DateTime)DateFilter) == 0
                                   select m;
                }

                if (!string.IsNullOrEmpty(NameFilter))
                {
                    filtered = from m in filtered
                               where m.Name.Contains(NameFilter)
                               select m;
                }

                Shows = new MyObservableCollection<Show>(filtered, App.Model.Shows);
            }
            else
                Shows = new MyObservableCollection<Show>(App.Model.Shows);
        }
    }
}
