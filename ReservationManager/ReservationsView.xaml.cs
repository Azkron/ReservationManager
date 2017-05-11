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


        public MyObservableCollection<Reservation> Reservations { get; private set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public bool IsValid { get; set; }

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

        public ReservationsView()
        {
            InitializeComponent();

            DataContext = this;

            Reservations = new MyObservableCollection<Reservation>(App.Model.Reservation);



            SaveCommand = new RelayCommand(() => { App.Model.SaveChanges(); }, () => { return IsValid; });

            RefreshCommand = new RelayCommand(() =>
            {
                App.CancelChanges();
                Reservations.Refresh(App.Model.Reservation);
            },
            () => { return IsValid; });
        }
    }
}
