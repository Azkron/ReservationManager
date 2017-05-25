using Microsoft.Win32;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
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
    /// Interaction logic for Reservation.xaml
    /// </summary>
    public partial class ReservationEdit : UserControlBase
    {
        public int? reservation = null;// save the id of the show in case the model gets reloaded from other tab
        private Reservation Reservation = null;
        //private ReservationsView reservationsView;

        public ICommand DeleteCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        private ClientsView clientsView = null;

        public ReservationEdit(Reservation reservation, bool isNew = false)
        {
            Reservation = reservation;
            InitializeComponent();

            Client = Reservation.Client;
            Show = Reservation.Show;
            Category = Reservation.Category;

            DataContext = this;
            clientsView = ClientsControl.Content as ClientsView;
            clientsView.ReservationEdit = this;

            ReadOnly = App.Rights(Table.RESERVATION) != Right.ALL;
            IsNew = isNew;


        }


        private bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                RaisePropertyChanged(nameof(IsNew));

                if (isNew)
                {
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                }
                else if (!ReadOnly)
                {
                    btnDelete.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                }

            }
        }


        private bool readOnly;
        public bool ReadOnly
        {
            get { return readOnly; }

            set
            {
                readOnly = value;
                if (readOnly)
                {
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                }

                RaisePropertyChanged(nameof(ReadOnly));
            }
        }

        private Client client;
        public Client Client
        {
            get { return client; }
            set
            {
                client = value;
                if (client != null)
                    ClientName = client.FullName;
                else
                    ClientName = "Select a client from the list below";
            }
        }

        private string clientName = "";
        public string ClientName
        {
            get { return clientName; }
            set
            {
                clientName = value;
                RaisePropertyChanged(nameof(ClientName));
            }
        }

        private MyObservableCollection<Show> shows = null;
        public MyObservableCollection<Show> Shows
        {
            get
            {
                if (shows == null)
                {
                    List<Show> li = new List<Show>();
                    foreach (Show s in App.Model.Shows)
                        if (s.FreePlacesTotal > 0)
                            li.Add(s);
                    shows = new MyObservableCollection<Show>(li);
                }

                return shows;
            }
        }
        
        public Show Show
        {
            get { return Reservation.Show; }
            set
            {
                Reservation.Show = value;
                if(value == null)
                {
                    AfterShow(Visibility.Collapsed);
                }
                else
                {
                    ShowFreePlaces = value.FreePlacesString;
                    ShowPrices = value.Prices;
                    Categories = new MyObservableCollection<Category>(value.GetCategories());
                    AfterShow(Visibility.Visible);
                }
                RaisePropertyChanged(nameof(Show));
            }
        }

        private void AfterShow(Visibility v)
        {
            lblPlacesTitle.Visibility = v;
            lblPlaces.Visibility = v;
            lblPricesTitle.Visibility = v;
            lblPrices.Visibility = v;
            lblCategoryTitle.Visibility = v;
            cmbCategory.Visibility = v;

            if(v == Visibility.Collapsed)
            {
                Categories = null;
                ShowFreePlaces = null;
                ShowPrices = null;
                Categories = null;
                Category = null;
                NumberInput = null;
            }
        }

        private MyObservableCollection<Category> categories;
        public MyObservableCollection<Category> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                RaisePropertyChanged(nameof(Categories));
            }
        }
        
        public Category Category
        {
            get { return Reservation.Category; }
            set
            {
                Reservation.Category = value;
                if (value == null)
                    AfterCategory(Visibility.Collapsed);
                else
                    AfterCategory(Visibility.Visible);
                RaisePropertyChanged(nameof(Category));
            }
        }


        private void AfterCategory(Visibility v)
        {
            lblNumberTitle.Visibility = v;
            txtNumber.Visibility = v; 

            if (v == Visibility.Collapsed)
                NumberInput = null;
            else
                NumberInput = NumberInput;// to recheck avaliable places
        }


        private string showFreePlaces = null;
        public string ShowFreePlaces
        {
            get { return showFreePlaces; }
            set
            {
                showFreePlaces = value;
                RaisePropertyChanged(nameof(ShowFreePlaces));
            }
        }

        private string showPrices = null;
        public string ShowPrices
        {
            get { return showPrices; }
            set
            {
                showPrices = value;
                RaisePropertyChanged(nameof(ShowPrices));
            }
        }


        public string NumberInput
        {
            get
            {
                return Reservation.Number.ToString("G29");
            }
            set
            {
                int intNumber = 0;
                if (!string.IsNullOrEmpty(value))
                {
                    intNumber = Util.StrToInt(value);
                    if (intNumber < 0) intNumber = 0;
                    else
                    {
                        int free = (int)Show.CalcFreePlacesByCat(Category);
                        if (intNumber > free)
                            intNumber = free;
                    } 
                }

                Reservation.Number = intNumber;
                RaisePropertyChanged(nameof(NumberInput));
            }
        }
    }
}
