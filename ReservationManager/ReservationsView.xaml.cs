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

        public ICommand ClearFilter { get; set; }

        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public bool IsValid { get; set; }
        
        public ReservationsView()
        {
            InitializeComponent();

            DataContext = this;

            Reservations = new MyObservableCollection<Reservation>(App.Model.Reservations);

            ClearFilter = new RelayCommand(() => { ShowFilter = ""; ClientFilter = ""; DateFilter = null; });


            SaveCommand = new RelayCommand(() => { App.Model.SaveChanges(); }, () => { return IsValid; });

            RefreshCommand = new RelayCommand(() =>
            {
                App.CancelChanges();
                Reservations.Refresh(App.Model.Reservations);
            },
            () => { return IsValid; });
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
                IQueryable<Reservation> filtered = App.Model.Reservations;

                if (DateFilter != null)
                {
                    Console.WriteLine("On date filter");
                    filtered = from m in filtered
                               where DateTime.Compare(m.Show.Date, (DateTime)DateFilter) == 0
                               select m;
                }

                if (!string.IsNullOrEmpty(ShowFilter))
                {
                    Console.WriteLine("On show filter");
                    filtered = from m in filtered
                               where m.Show.Name.Contains(ShowFilter)
                               select m;
                }
                
                if(!string.IsNullOrEmpty(ClientFilter))
                {
                    Console.WriteLine("On client filter");
                    filtered = from m in filtered
                               where m.Client.FirstName.Contains(ClientFilter) 
                               || m.Client.LastName.Contains(ClientFilter)
                               select m;
                }

                Console.WriteLine("On reservations");
                Reservations = new MyObservableCollection<Reservation>(filtered, App.Model.Reservations);
            }
            else
                Reservations = new MyObservableCollection<Reservation>(App.Model.Reservations);
        }
    }
}
