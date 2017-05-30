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
//using ReservationManager.Properties;
using R = ReservationManager.Properties;

namespace ReservationManager
{
    /// <summary>
    /// Interaction logic for ShowsView.xaml
    /// </summary>
    public partial class ReservationsView : UserControlBase
    {

        public ICommand ClearFilterCommand { get; set; }

        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public bool IsValid { get; set; }
        
        
        public ReservationsView()
        {

            string testRes = R.Resources.Date;
            InitializeComponent();

            DataContext = this;

            baseQuery = App.Model.Reservations;

            Reservations = new MyObservableCollection<Reservation>(App.Model.Reservations);

            ReadOnly = App.Rights(Table.RESERVATION) != Right.ALL;

            ClearFilterCommand = new RelayCommand(() => { ClearFilter(); });

            NewCommand = new RelayCommand(() => 
            { App.Messenger.NotifyColleagues(App.MSG_NEW_RESERVATION); });
            EditCommand = new RelayCommand<Reservation>((r) =>
            { App.Messenger.NotifyColleagues(App.MSG_EDIT_RESERVATION, r); });


            RefreshCommand = new RelayCommand(() =>
            {
                Refresh();
                //Reservations.Refresh(baseQuery);
            },
            () => { return IsValid; });

            App.Messenger.Register(App.MSG_REFRESH, () =>
            {
                this.Refresh();
            });
        }

        private void Refresh()
        {
            ClearFilter();
            Reservations.Refresh(App.Model.Reservations);
            if (showId != null)
                Show = (from s in App.Model.Shows where s.Id == showId select s).FirstOrDefault();
            else if(clientId != null)
                Client = (from c in App.Model.Clients where c.Id == clientId select c).FirstOrDefault();
            SelectedReservation = null;
            SetMode();

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

        private void SetMode()
        {
            if (Show != null)
            {
                baseQuery = from r in App.Model.Reservations where r.Show.Id == show.Id select r;
                ShowFilterTxt.Visibility = Visibility.Collapsed;
                ShowFilterLabel.Visibility = Visibility.Collapsed;
                ApplyFilterAction();
                NewCommand = new RelayCommand(() =>
                { App.Messenger.NotifyColleagues(App.MSG_NEW_SHOW_RESERVATION, Show); });
            }
            else if (Client != null)
            {
                baseQuery = from r in App.Model.Reservations where r.Client.Id == client.Id select r;
                ClientFilterTxt.Visibility = Visibility.Collapsed;
                ClientFilterLabel.Visibility = Visibility.Collapsed;
                ApplyFilterAction();

                NewCommand = new RelayCommand(() =>
                { App.Messenger.NotifyColleagues(App.MSG_NEW_CLIENT_RESERVATION, Client); });
            }
            else
            {
                baseQuery = App.Model.Reservations;
                ShowFilterTxt.Visibility = Visibility.Visible;
                ShowFilterLabel.Visibility = Visibility.Visible;
                ApplyFilterAction();
            }

        }

        private void ClearFilter()
        {
            ShowFilter = ""; ClientFilter = ""; DateFilter = null;
        }

        private int? clientId = null;//To recover the client in case of an App.CancelChanges or something similar
        private Client client = null;
        public Client Client
        {
            set
            {
                client = value;
                if (client != null)
                    clientId = client.Id;
                SetMode();
            }

            get
            {
                return client;
            }
        }

        private int? showId = null;//To recover the client in case of an App.CancelChanges or something similar
        private Show show = null;
        public Show Show
        {
            set
            {
                show = value;
                if (show != null)
                    showId = show.Id;
                SetMode();
            }

            get
            {
                return show;
            }
        }

        private IQueryable<Reservation> baseQuery;

        private MyObservableCollection<Reservation> reservations;
        public MyObservableCollection<Reservation> Reservations
        {
            get { return reservations; }
            set
            {
                reservations = value;
                RaisePropertyChanged(nameof(Reservations));
            }
        }

        Show selectedReservation;
        public Show SelectedReservation
        {
            get { return selectedReservation; }
            set
            {
                selectedReservation = value;
            }
        }

        private string showFilter;
        public string ShowFilter
        {
            get { return showFilter; }
            set
            {
                showFilter = value;
                ApplyFilterAction();
                RaisePropertyChanged(nameof(ShowFilter));
            }
        }

        private string clientFilter;
        public string ClientFilter
        {
            get { return clientFilter; }
            set
            {
                clientFilter = value;
                ApplyFilterAction();
                RaisePropertyChanged(nameof(ClientFilter));
            }
        }

        private DateTime? dateFilter;
        public DateTime? DateFilter
        {
            get { return dateFilter; }
            set
            {
                dateFilter = value;
                ApplyFilterAction();
                RaisePropertyChanged(nameof(DateFilter));
            }
        }


        private void ApplyFilterAction()
        {
            if (!string.IsNullOrEmpty(ShowFilter) || !string.IsNullOrEmpty(ClientFilter) || DateFilter != null)
            {
                IQueryable<Reservation> filtered = baseQuery;

                if (DateFilter != null)
                {
                    filtered = from r in filtered
                               where DateTime.Compare(r.Show.Date, (DateTime)DateFilter) == 0
                               select r;
                }

                if (!string.IsNullOrEmpty(ShowFilter))
                {
                    filtered = from r in filtered
                               where r.Show.Name.Contains(ShowFilter)
                               select r;
                }
                
                if(!string.IsNullOrEmpty(ClientFilter))
                {
                    filtered = from r in filtered
                               where r.Client.FirstName.Contains(ClientFilter) 
                               || r.Client.LastName.Contains(ClientFilter)
                               select r;
                }
;
                Reservations = new MyObservableCollection<Reservation>(filtered, App.Model.Reservations);
            }
            else
                Reservations = new MyObservableCollection<Reservation>(baseQuery, App.Model.Reservations);
        }
    }
}
