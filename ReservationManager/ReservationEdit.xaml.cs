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

            SaveCommand = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            CancelCommand = new RelayCommand(CancelAction, CanSaveOrCancelAction);
            DeleteCommand = new RelayCommand(DeleteAction, CanDeleteAction);

        }


        public bool CantModify { get { return IsExisting || ReadOnly; } }

        public bool IsExisting { get { return !isNew; } }

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
                else
                {
                    ClientsPanel.Visibility = Visibility.Collapsed;
                    cmbShows.Visibility = Visibility.Collapsed;
                    cmbCategory.Visibility = Visibility.Collapsed;

                    lblShow.Visibility = Visibility.Visible;
                    lblCategory.Visibility = Visibility.Visible;

                    if (!ReadOnly)
                    {
                        btnDelete.Visibility = Visibility.Visible;
                        btnCancel.Visibility = Visibility.Visible;
                        btnSave.Visibility = Visibility.Visible;
                    }
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
                    ClientsPanel.Visibility = Visibility.Collapsed;
                    cmbShows.Visibility = Visibility.Collapsed;
                    cmbCategory.Visibility = Visibility.Collapsed;
                    txtNumber.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                    lblPlacesTitle.Visibility = Visibility.Collapsed;
                    lblPlaces.Visibility = Visibility.Collapsed;
                    lblPricesTitle.Visibility = Visibility.Collapsed;
                    lblPrices.Visibility = Visibility.Collapsed;

                    lblShow.Visibility = Visibility.Visible;
                    lblCategory.Visibility = Visibility.Visible;
                    lblNumber.Visibility = Visibility.Visible;
                }

                RaisePropertyChanged(nameof(ReadOnly));
            }
        }
        
        public Client Client
        {
            get { return Reservation.Client; }
            set
            {
                Reservation.Client = value;
                if (value != null)
                    ClientName = value.FullName;
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
                    {
                        if (s == Show)
                            s.DiscountResPlaces(Reservation.Category.Id, Reservation.Number);

                        if (s.FreePlacesTotal > 0)
                            li.Add(s);
                    }
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
                bool selectText = false;
                if (!string.IsNullOrEmpty(value))
                {
                    intNumber = Util.StrToInt(value);
                    if (intNumber < 1)
                    {
                        intNumber = 1;
                        selectText = true;
                    }
                    else
                    {
                        int free = (int)Show.CalcFreePlacesByCat(Category);
                        if (intNumber > free)
                        {
                            intNumber = free;
                            selectText = true;
                        }
                    } 
                }

                Reservation.Number = intNumber;
                RaisePropertyChanged(nameof(NumberInput));
                if (selectText)
                    Util.SelectText(txtNumber);
            }
        }

        private void SaveAction()
        {
            if (IsNew)
            {
                App.Model.Reservations.Add(Reservation);
                IsNew = false;
            }

            App.Model.SaveChanges();
            App.Messenger.NotifyColleagues(App.MSG_RESERVATION_CHANGED, Reservation);
        }

        private bool CanSaveOrCancelAction()
        {
            if (IsNew)
                return Reservation.Category != null 
                    && Reservation.Client != null
                    && Reservation.Number > 0
                    && !HasErrors;

            var change = (from r in App.Model.ChangeTracker.Entries<Reservation>()
                          where r.Entity == Reservation
                          select r).FirstOrDefault();

            return change != null && change.State != EntityState.Unchanged;
        }


        private void CancelAction()
        {
            var change = (from r in App.Model.ChangeTracker.Entries<Reservation>()
                          where r.Entity == Reservation
                          select r).FirstOrDefault();

            if (change != null)
            {
                change.Reload();
                RaisePropertyChanged(nameof(Show));
                RaisePropertyChanged(nameof(Category));
                RaisePropertyChanged(nameof(NumberInput));
            }

            // NOT NEEDED TO CLOSE TAB ON CANCEL, IT JUST RESETS THE DATA (CODE ABOVE)
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, !isNew ? Pseudo : "new member");
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Pseudo);
        }

        private string deleteTxt = "Delete";
        public string DeleteTxt
        {
            get { return deleteTxt; }
            set
            {
                deleteTxt = value;
                RaisePropertyChanged(nameof(DeleteTxt));
            }
        }

        private bool confirmDelete = false;
        private void DeleteAction()
        {
            if(!confirmDelete)
            {
                DeleteTxt = "CONFIRM";
                confirmDelete = true;
            }
            else
            {
                //DataContext = this;
                //App.Model.Reservations.Attach(Reservation);
                string tabHeader = Reservation.TabHeaderPseudo;
                App.Model.Reservations.Remove(Reservation);
                App.Model.SaveChanges();

                App.Messenger.NotifyColleagues(App.MSG_GENERAL_REFRESH);
                App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, tabHeader);
            }

            // App.Messenger.NotifyColleagues(App.MSG_CLIENT_CHANGED, Client);
        }

        private bool CanDeleteAction()
        {
            return IsExisting;
        }
    }
}
