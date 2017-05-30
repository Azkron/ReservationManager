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
    /// Interaction logic for ClientEditView.xaml
    /// </summary>
    public partial class ClientEdit : UserControlBase
    {


        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand TestCommand { get; set; }

        public Client Client { get; set; }

        private ReservationsView reservations = null;
        
        public ClientEdit(Client client, bool isNew = false)
        {
            Client = client;

            InitializeComponent();

            DataContext = this;

            ReadOnly = App.Rights(Table.CLIENT) != Right.ALL;
            IsNew = isNew;

            reservations = Reservations.Content as ReservationsView;
            reservations.Client = Client;
            
            SaveCommand = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            CancelCommand = new RelayCommand(CancelAction, CanSaveOrCancelAction);
            DeleteCommand = new RelayCommand(DeleteAction, CanDeleteAction);
            


        }

        private bool readOnly;
        public bool ReadOnly
        {
            get { return readOnly; }

            set
            {
                readOnly = value;
                if(readOnly)
                {
                    txtBdd.Visibility = Visibility.Collapsed;
                    txtFirstName.Visibility = Visibility.Collapsed;
                    txtLastName.Visibility = Visibility.Collapsed;
                    txtPostalCode.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;

                    lblBdd.Visibility = Visibility.Visible;
                    lblFirstName.Visibility = Visibility.Visible;
                    lblLastName.Visibility = Visibility.Visible;
                    lblPostalCode.Visibility = Visibility.Visible;
                }

                RaisePropertyChanged(nameof(ReadOnly));
            }
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
                    btnCancel.Visibility = Visibility.Collapsed;
                else if (!ReadOnly)
                    btnCancel.Visibility = Visibility.Visible;
            }
        }

        public bool IsExisting { get { return !isNew; } }

        public string LastName
        {
            get { return Client.LastName; }
            set
            {
                Client.LastName = value;
                RaisePropertyChanged(nameof(LastName));
                //App.Messenger.NotifyColleagues(App.MSG_LAST_NAME_CHANGED, string.IsNullOrEmpty(value) ? "<new member>" : value);
            }
        }

        public string FirstName
        {
            get { return Client.FirstName; }
            set
            {
                Client.FirstName = value;
                RaisePropertyChanged(nameof(FirstName));
                //App.Messenger.NotifyColleagues(App.MSG_LAST_NAME_CHANGED, string.IsNullOrEmpty(value) ? "<new member>" : value);
            }
        }

        public int? PostalCode
        {
            get { return Client.PostalCode; }
            set
            {
                Client.PostalCode = value;
                RaisePropertyChanged(nameof(PostalCode));
                //App.Messenger.NotifyColleagues(App.MSG_LAST_NAME_CHANGED, string.IsNullOrEmpty(value) ? "<new member>" : value);
            }
        }

        public DateTime? Bdd
        {
            get { return Client.Bdd; }
            set
            {
                Client.Bdd = value;
                RaisePropertyChanged(nameof(Bdd));
                //App.Messenger.NotifyColleagues(App.MSG_LAST_NAME_CHANGED, string.IsNullOrEmpty(value) ? "<new member>" : value);
            }
        }

        private void SaveAction()
        {
            if (IsNew)
            {
                App.Model.Clients.Add(Client);
                IsNew = false;
            }

            App.Model.SaveChanges();
            App.Messenger.NotifyColleagues(App.MSG_REFRESH);
            App.Messenger.NotifyColleagues(App.MSG_CLIENT_CHANGED, Client);
        }

        public bool CanSaveOrCancelAction()
        {
            if (IsNew)
                return !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Name) && !HasErrors;

            var change = (from c in App.Model.ChangeTracker.Entries<Client>()
                          where c.Entity == Client
                          select c).FirstOrDefault();

            return change != null && change.State != EntityState.Unchanged;
        }


        public void CancelAction()
        {
            var change = (from c in App.Model.ChangeTracker.Entries<Client>()
                          where c.Entity == Client
                          select c).FirstOrDefault();

            if (change != null)
            {
                change.Reload();
                RaisePropertyChanged(nameof(FirstName));
                RaisePropertyChanged(nameof(LastName));
                RaisePropertyChanged(nameof(PostalCode));
                RaisePropertyChanged(nameof(Bdd));
                App.Messenger.NotifyColleagues(App.MSG_REFRESH);
            }

            // NOT NEEDED TO CLOSE TAB ON CANCEL, IT JUST RESETS THE DATA (CODE ABOVE)
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, !isNew ? Pseudo : "new member");
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Pseudo);
        }



        private void DeleteAction()
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            if(Client.Reservations.Count > 0)
                result = MessageBox.Show(Properties.Resources.DeleteReservations, Properties.Resources.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Client.Reservations.Clear();
                App.Model.Clients.Remove(Client);
                App.Model.SaveChanges();

                App.Messenger.NotifyColleagues(App.MSG_REFRESH);
                App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Client.FullName);
            }
           // App.Messenger.NotifyColleagues(App.MSG_CLIENT_CHANGED, Client);
        }

        private bool CanDeleteAction()
        {
            return IsExisting;
        }

    }
}
