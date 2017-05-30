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
        public int? showId = null;// save the id of the show in case the model gets reloaded from other tab
        private Show show = null;
        public Show Show { get { return show; } set { show = value;  if(show != null)showId = Show.Id; } }
        //private ReservationsView reservationsView;

        public ICommand DeleteCommand { get; set; }
        public ICommand DeletePriceCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadImage { get; set; }
        public ICommand ClearImage { get; set; }

        private ReservationsView reservations = null;

        public ShowEdit(Show show, bool isNew = false)
        {
            Show = show;
            InitializeComponent();
                    
            DataContext = this;

            ShowReadOnly = App.Rights(Table.SHOW) != Right.ALL;
            PriceReadOnly = App.Rights(Table.PRICE) != Right.ALL;

            IsNew = isNew;
            if (isNew)
                Show.Date = DateTime.Today;

            reservations = Reservations.Content as ReservationsView;
            reservations.Show = Show;
            HasReservationsTxt = null;
            Category = null;

            SaveCommand = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            CancelCommand = new RelayCommand(CancelAction, CanSaveOrCancelAction);
            DeleteCommand = new RelayCommand(DeleteAction, CanDeleteAction);
            DeletePriceCommand = new RelayCommand(DeletePriceAction);
            LoadImage = new RelayCommand(LoadImageAction);
            ClearImage = new RelayCommand(ClearImageAction);
        }

        private void Refresh()
        {
            Show = (from s in App.Model.Shows where s.Id == showId select s).FirstOrDefault();
            PriceList = null;
            Category = null;
            categories = null;
            HasReservationsTxt = null;
            RaisePropertyChanged(nameof(Categories));
            //RaisePropertyChanged(nameof(Category));
            Show.RefreshStrings();
            RaisePropertyChanged(nameof(Price));
            RaisePropertyChanged(nameof(Show));
            RaisePropertyChanged(nameof(ShowName));
            RaisePropertyChanged(nameof(Description));
            RaisePropertyChanged(nameof(Poster));
            RaisePropertyChanged(nameof(Date));
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
                else
                {
                    if (!ShowReadOnly)
                    {
                        btnCancel.Visibility = Visibility.Visible;
                        btnDelete.Visibility = Visibility.Visible;
                    }
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
                    lblPriceHasReservations.Visibility = Visibility.Collapsed;
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
                        Show.PriceLists.Add(p);
                        Category.PriceList.Add(p);
                    }

                    PriceList = p;
                    RaisePropertyChanged(nameof(Price));
                    Show.RefreshStrings();
                    RaisePropertyChanged(nameof(Show));
                    

                    int ResCount = Show.ReservationsCount(Category.Id);
                    if (ResCount > 0)
                        HasReservationsPriceTxt = ResCount + " reservations !!!";
                    else
                        HasReservationsPriceTxt = "No reservations";

                    confirmDelete = false;
                    confirmPriceDelete = false;
                    DeletePriceTxt = "Del Cat";
                    txtPrice.Visibility = Visibility.Visible;
                    txtCurrency.Visibility = Visibility.Visible;
                    btnDeletePrice.Visibility = Visibility.Visible;
                    lblPriceHasReservations.Visibility = Visibility.Visible;
                }
            }
        }

        private string deletePriceTxt = "Del Cat";
        public string DeletePriceTxt
        {
            get { return deletePriceTxt; }
            set
            {
                deletePriceTxt = value;
                RaisePropertyChanged(nameof(DeletePriceTxt));
            }
        }

        private string deleteTxt = "Delete";
        public string DeleteTxt
        {
            get { return deleteTxt; }
            set
            {
                deleteTxt = value;
                RaisePropertyChanged(nameof(deleteTxt));
            }
        }

        private string hasReservationsTxt = null;
        public string HasReservationsTxt
        {
            get {  return hasReservationsTxt; }
            set
            {
                int ResCount = Show.Reservations.Count;
                if (ResCount > 0)
                    hasReservationsTxt = ResCount + " " + Properties.Resources.HasReservations;
                else
                    hasReservationsTxt = Properties.Resources.NoReservations;

                RaisePropertyChanged(nameof(HasReservationsPriceTxt));
            }
        }

        private string hasReservationsPriceTxt = "";
        public string HasReservationsPriceTxt
        {
            get { return hasReservationsPriceTxt; }
            set
            {
                hasReservationsPriceTxt = value;
                RaisePropertyChanged(nameof(HasReservationsPriceTxt));
            }
        }

        public string Price
        {
            get {
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


        private bool showReadOnly;
        public bool ShowReadOnly
        {
            get { return showReadOnly; }

            set
            {
                showReadOnly = value;
                if (showReadOnly)
                {
                    txtName.Visibility = Visibility.Collapsed;
                    txtDescription.Visibility = Visibility.Collapsed;
                    txtDate.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnLoad.Visibility = Visibility.Collapsed;
                    btnClear.Visibility = Visibility.Collapsed;
                    lblHasReservations.Visibility = Visibility.Collapsed;

                    lblName.Visibility = Visibility.Visible;
                    lblDescription.Visibility = Visibility.Visible;
                    lblDate.Visibility = Visibility.Visible;
                }

                RaisePropertyChanged(nameof(ShowReadOnly));
            }
        }

        private bool priceReadOnly;
        public bool PriceReadOnly
        {
            get { return priceReadOnly; }

            set
            {
                priceReadOnly = value;
                if (priceReadOnly)
                {
                    lblSetPrice.Visibility = Visibility.Collapsed;
                    gridSetPrice.Visibility = Visibility.Collapsed;
                }
                else
                {
                    lblSetPrice.Visibility = Visibility.Visible;
                    gridSetPrice.Visibility = Visibility.Visible;
                }

                RaisePropertyChanged(nameof(PriceReadOnly));
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
            
            App.Messenger.NotifyColleagues(App.MSG_REFRESH);

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
                changeShow.Reload();
                Refresh();
            }
            
        }


        private bool confirmDelete = false;
        private void DeleteAction()
        {
            if (Show.Reservations.Count != 0)
            {
                if (!confirmDelete)
                {
                    confirmDelete = true;
                    DeleteTxt = Properties.Resources.Confirm;
                }
                else
                {
                    show.Reservations.Clear();
                    show.PriceLists.Clear();

                    App.Model.Shows.Remove(show);
                    App.Model.SaveChanges();
                    App.Messenger.NotifyColleagues(App.MSG_REFRESH);
                    App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Show.Name);
                    
                }
            }
            else
            {
                show.Reservations.Clear();
                show.PriceLists.Clear();

                App.Model.Shows.Remove(show);
                App.Model.SaveChanges();
                App.Messenger.NotifyColleagues(App.MSG_REFRESH);
                App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Show.Name);
            }
            
        }

        private bool confirmPriceDelete = false;

        private void DeletePriceAction()
        {
            if (Show.ReservationsCount(Category.Id) != 0)
            {
                if(!confirmPriceDelete)
                {
                    confirmPriceDelete = true;
                    DeletePriceTxt = Properties.Resources.Confirm;
                }
                else
                {
                    foreach (var res in from r in App.Model.Reservations where r.IdCat == Category.Id && r.IdShow == Show.Id select r)
                        App.Model.Reservations.Remove(res);

                    App.Model.PriceLists.Remove(PriceList);

                    HasReservationsTxt = null;
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

        private void LoadImageAction()
        {
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                var filename = fd.FileName;
                if (filename != null && File.Exists(filename))
                {
                    var img = System.Drawing.Image.FromFile(filename);
                    var ms = new MemoryStream();
                    var ext = System.IO.Path.GetExtension(filename).ToUpper();

                    switch (ext)
                    {
                        case ".PNG":
                            img.Save(ms, ImageFormat.Png);
                            break;
                        case ".GIF":
                            img.Save(ms, ImageFormat.Gif);
                            break;
                        case ".JPG":
                            img.Save(ms, ImageFormat.Jpeg);
                            break;
                    }

                    Poster = ms.ToArray();
                }
            }
        }
        

        private void ClearImageAction()
        {
            Poster = null;
        }

    }
}
