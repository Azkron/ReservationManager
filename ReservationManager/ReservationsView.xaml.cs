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
    public partial class ReservationsView : UserControlBase
    {

        public ICommand ClearFilterCommand { get; set; }

        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public bool IsValid { get; set; }

        private Client client = null;
        
        public ReservationsView()
        {
            InitializeComponent();

            DataContext = this;

            App.Messenger.Register<Client>(App.MSG_CLIENT_RESERVATION, client => { SingleClient(client);  });

            Reservations = new MyObservableCollection<Reservation>(App.Model.Reservations);

            ClearFilterCommand = new RelayCommand(() => { ClearFilter(); });


            RefreshCommand = new RelayCommand(() =>
            {
                ClearFilter();
                Reservations.Refresh(App.Model.Reservations);
            },
            () => { return IsValid; });
        }

        private void ClearFilter()
        {
            ShowFilter = ""; ClientFilter = ""; DateFilter = null;
        }

        private void SingleClient(Client client)
        {
            this.client = client;
            ClientFilterTxt.Visibility = Visibility.Collapsed;
            ClientFilterLabel.Visibility = Visibility.Collapsed;
            ApplyFilterAction();
        }

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
                Console.WriteLine(selectedReservation);
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
                Console.WriteLine("On apply filter");
                IQueryable<Reservation> filtered;

                if (client == null)
                    filtered = App.Model.Reservations;
                else
                    filtered = from r in App.Model.Reservations
                               where r.Client.Id == client.Id
                               select r;

                if (DateFilter != null)
                {
                    Console.WriteLine("On date filter");
                    filtered = from r in filtered
                               where DateTime.Compare(r.Show.Date, (DateTime)DateFilter) == 0
                               select r;
                }

                if (!string.IsNullOrEmpty(ShowFilter))
                {
                    Console.WriteLine("On show filter");
                    filtered = from r in filtered
                               where r.Show.Name.Contains(ShowFilter)
                               select r;
                }
                
                if(!string.IsNullOrEmpty(ClientFilter))
                {
                    Console.WriteLine("On client filter");
                    filtered = from r in filtered
                               where r.Client.FirstName.Contains(ClientFilter) 
                               || r.Client.LastName.Contains(ClientFilter)
                               select r;
                }

                Console.WriteLine("On reservations");
                Reservations = new MyObservableCollection<Reservation>(filtered, App.Model.Reservations);
            }
            else
                Reservations = new MyObservableCollection<Reservation>(App.Model.Reservations);
        }
    }
}
