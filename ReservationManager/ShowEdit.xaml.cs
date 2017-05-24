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
    /// Interaction logic for ShowEdit.xaml
    /// </summary>
    public partial class ShowEdit : UserControlBase
    {
        public Show Show { get; set; }
        //private ReservationsView reservationsView;

        public ICommand DeleteCommand { get; set; }
        public ICommand DeletePriceCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public ShowEdit(Show show, bool isNew = false)
        {
            Show = show;
            InitializeComponent();
                    
            DataContext = this;

            ReadOnly = App.Rights(Table.SHOW) != Right.ALL;
            IsNew = isNew;

            SaveCommand = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            CancelCommand = new RelayCommand(CancelAction, CanSaveOrCancelAction);
            DeleteCommand = new RelayCommand(DeleteAction, CanDeleteAction);
            DeletePriceCommand = new RelayCommand(DeletePriceAction);
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
                else if(!ReadOnly)
                {
                    btnDelete.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                }

            }
        }

        private Category category = null;
        public Category Category
        {
            get { return category; }
            set
            {
                category = value;
                if (category == null)
                {
                    PriceList = null;
                    txtPrice.Visibility = Visibility.Collapsed;
                    txtCurrency.Visibility = Visibility.Collapsed;
                    btnDeletePrice.Visibility = Visibility.Collapsed;
                    txtHasReservatoins.Visibility = Visibility.Collapsed;
                    RaisePropertyChanged(nameof(Category));
                    RaisePropertyChanged(nameof(Price));
                    Show.RefreshStrings();
                    RaisePropertyChanged(nameof(Show));
                }
                else
                {
                    PriceList p = (from pl in Show.PriceLists where pl.IdCat == Category.Id select pl).ToList().FirstOrDefault();

                    if (p == null)
                    {
                        p = new PriceList();
                        p.IdCat = Category.Id;
                        p.IdShow = Show.Id;
                        p.Price = 10;
                        App.Model.PriceLists.Add(p);
                    }

                    PriceList = p;
                    RaisePropertyChanged(nameof(Price));
                    Show.RefreshStrings();
                    RaisePropertyChanged(nameof(Show));

                    DeletePriceTxt = "Delete";

                    int ResCount = Show.ReservationsCount(Category.Id);
                    if (ResCount > 0)
                        HasReservationsTxt = ResCount + " reservations !!!";
                    else
                        HasReservationsTxt = "No reservations";

                    confirmDelete = false;
                    DeletePriceTxt = "Delete";
                    txtPrice.Visibility = Visibility.Visible;
                    txtCurrency.Visibility = Visibility.Visible;
                    btnDeletePrice.Visibility = Visibility.Visible;
                    txtHasReservatoins.Visibility = Visibility.Visible;
                }
            }
        }

        private string deletePriceTxt = "Delete";
        public string DeletePriceTxt
        {
            get { return deletePriceTxt; }
            set
            {
                deletePriceTxt = value;
                RaisePropertyChanged(nameof(DeletePriceTxt));
            }
        }

        private string hasReservationsTxt = "";
        public string HasReservationsTxt
        {
            get { return hasReservationsTxt; }
            set
            {
                hasReservationsTxt = value;
                RaisePropertyChanged(nameof(HasReservationsTxt));
            }
        }

        public string Price
        {
            get {
                Console.WriteLine(PriceList != null ? PriceList.Price.ToString("G29") : null);
                return PriceList != null ? PriceList.Price.ToString("G29") : null; }
            set
            {
                int intPrice = 0;
                if(!string.IsNullOrEmpty(value))
                {
                    intPrice = Util.StrToInt(value);
                    if (intPrice < 0) intPrice = 0;
                }

                PriceList.Price = intPrice;
                Show.RefreshStrings();
                RaisePropertyChanged(nameof(Show));

                RaisePropertyChanged(nameof(Price));
            }
        }

        private PriceList priceList = null;
        public PriceList PriceList
        {
            get { return priceList; }
            set
            {
                priceList = value;
            }
        }


        private MyObservableCollection<Category> categories = null;
        public MyObservableCollection<Category> Categories
        {
            get
            {
                if(categories == null)
                    categories = new MyObservableCollection<Category>(App.Model.Categories);
                
                return categories;
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

        public bool IsExisting { get { return !isNew; } }

        public string ShowName
        {
            get { return Show.Name; }
            set
            {
                Show.Name = value;
                RaisePropertyChanged(nameof(ShowName));
                //App.Messenger.NotifyColleagues(App.MSG_LAST_NAME_CHANGED, string.IsNullOrEmpty(value) ? "<new member>" : value);
            }
        }

        public string Description
        {
            get { return Show.Description; }
            set
            {
                Show.Description = value;
                RaisePropertyChanged(nameof(Description));
                //App.Messenger.NotifyColleagues(App.MSG_LAST_NAME_CHANGED, string.IsNullOrEmpty(value) ? "<new member>" : value);
            }
        }

        public byte[] Poster
        {
            get { return Show.Poster; }
            set
            {
                Show.Poster = value;
                RaisePropertyChanged(nameof(Show.Poster));
            }
        }

        public DateTime Date
        {
            get { return Show.Date; }
            set
            {
                Show.Date = value;
                RaisePropertyChanged(nameof(Date));
                //App.Messenger.NotifyColleagues(App.MSG_LAST_NAME_CHANGED, string.IsNullOrEmpty(value) ? "<new member>" : value);
            }
        }


        private void SaveAction()
        {
            if (IsNew)
            {
                App.Model.Shows.Add(Show);
                IsNew = false;
            }

            App.Model.SaveChanges();
            App.Messenger.NotifyColleagues(App.MSG_SHOW_CHANGED, Show);
        }

        private bool CanSaveOrCancelAction()
        {
            if (IsNew)
                return !string.IsNullOrEmpty(ShowName) && !string.IsNullOrEmpty(Description) && Date != null && !HasErrors;

            var changeShow = (from c in App.Model.ChangeTracker.Entries<Show>()
                          where c.Entity == Show
                          select c).FirstOrDefault();

            
            var changePrice = (from p in App.Model.ChangeTracker.Entries<PriceList>()
                          where p.Entity.IdShow == Show.Id
                          select p).FirstOrDefault();
            

            return (changePrice != null && changePrice.State != EntityState.Unchanged) 
                || (changeShow != null && changeShow.State != EntityState.Unchanged);
        }


        private void CancelAction()
        {
            var changeShow = (from c in App.Model.ChangeTracker.Entries<Show>()
                              where c.Entity == Show
                              select c).FirstOrDefault();


            var changePrice = (from p in App.Model.ChangeTracker.Entries<PriceList>()
                               where p.Entity.IdShow == Show.Id
                               select p).FirstOrDefault();


            if(changePrice != null || changeShow != null)
            {
                int showId = Show.Id;
                App.CancelChanges();
                Show = (from s in App.Model.Shows where s.Id == showId select s).FirstOrDefault();
                PriceList = null;
                Category = null;
                categories = null;
                RaisePropertyChanged(nameof(Categories));
                RaisePropertyChanged(nameof(Category));
                RaisePropertyChanged(nameof(Price));
                Show.RefreshStrings();
                RaisePropertyChanged(nameof(Show));
                RaisePropertyChanged(nameof(ShowName));
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(Poster));
                RaisePropertyChanged(nameof(Date));
            }

            // NOT NEEDED TO CLOSE TAB ON CANCEL, IT JUST RESETS THE DATA (CODE ABOVE)
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, !isNew ? Pseudo : "new member");
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Pseudo);
        }


        private void DeleteAction()
        {
            App.Model.Shows.Remove(Show);
            App.Model.SaveChanges();

            App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Show.Name);

            //App.Messenger.NotifyColleagues(App.MSG_SHOW_CHANGED, Show);
        }

        private bool confirmDelete = false;

        private void DeletePriceAction()
        {
            if (Show.ReservationsCount(Category.Id) != 0)
            {
                if(!confirmDelete)
                {
                    confirmDelete = true;
                    DeletePriceTxt = "CONFIRM";
                }
                else
                {
                    foreach (var res in from r in App.Model.Reservations where r.IdCat == Category.Id && r.IdShow == Show.Id select r)
                        App.Model.Reservations.Remove(res);

                    App.Model.PriceLists.Remove(PriceList);

                    Category = null;
                }
            }
            else
            {
                App.Model.PriceLists.Remove(PriceList);

                Category = null;
            }


            //App.Messenger.NotifyColleagues(App.MSG_SHOW_CHANGED, Show);
        }

        private bool CanDeleteAction()
        {
            return IsExisting;
        }
        /*
        private class CatRow
        {
            private Category cat;
            private Show Show;
            private string name;


            public CatRow(Show show, Category cat)
            {
                this.cat = cat;
                this.Show = show;

            }

            
            PriceList PriceList
            {
                get { return (from p in Show.PriceLists where p.IdCat == cat.Id select p).FirstOrDefault(null); }
                set
                {
                    Show.PriceLists.Add(value);
                }
            }

            private bool isActive;
            bool IsActive
            {
                get { return PriceList != null; }

                set
                {
                    isActive = value;
                    if (isActive && PriceList == null)
                    {
                        PriceList = new PriceList();
                        PriceList.Show = show;
                        PriceList.Category = cat;
                        PriceList.Price = 10;
                    }
                }
            }

            
            public decimal? Price
            {
                get { return PriceList != null ? (decimal?)PriceList.Price : null; }

                set
                {
                    if (PriceList != null)
                        PriceList.Price = (decimal)value;
                    else
                        throw new SystemException("Can´t set the price of a category without a pricelist");
                }
            }

            //Category
            //Price
            //Name
            //IsActive
        }*/

    }
}
