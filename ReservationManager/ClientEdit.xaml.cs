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
        public Client Client { get; set; }

        private ReservationsView reservations = null;

        public ClientEdit() : this(App.Model.Clients.FirstOrDefault<Client>(), false)
        {
            
        }

        public ClientEdit(Client client, bool isNew = false)
        {
            DataContext = this;
            Client = client;
            InitializeComponent();

            ReadOnly = App.Rights(Table.CLIENT) != Right.ALL;
            IsNew = isNew;

            reservations = Reservations.Content as ReservationsView;
            reservations.Client = Client;

            Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            Cancel = new RelayCommand(CancelAction);
            Delete = new RelayCommand(DeleteAction, CanDeleteAction);
        }

        private bool readOnly;
        public bool ReadOnly
        {
            get { return !readOnly; }

            set
            {
                readOnly = value;
                if(readOnly)
                {
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                }
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
                Console.WriteLine("value = " + value);
                Console.WriteLine("Client.Bdd = " + Client.Bdd);
                RaisePropertyChanged(nameof(Bdd));
                //App.Messenger.NotifyColleagues(App.MSG_LAST_NAME_CHANGED, string.IsNullOrEmpty(value) ? "<new member>" : value);
            }
        }



        public ICommand Save { get; set; }

        private void SaveAction()
        {
            if (IsNew)
            {
                // A small shortcut  /(.)(.)\
                //                     |  |
                App.Model.Clients.Add(Client);
                IsNew = false;
            }

            App.Model.SaveChanges();
            App.Messenger.NotifyColleagues(App.MSG_CLIENT_CHANGED, Client);
        }

        private bool CanSaveOrCancelAction()
        {
            if (IsNew)
                return !string.IsNullOrEmpty(LastName) && !HasErrors;

            var change = (from c in App.Model.ChangeTracker.Entries<Client>()
                          where c.Entity == Client
                          select c).FirstOrDefault();

            return change != null && change.State != EntityState.Unchanged;
        }


        public ICommand Cancel { get; set; }

        private void CancelAction()
        {
            Console.WriteLine("Cancel");

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
            }

            // NOT NEEDED TO CLOSE TAB ON CANCEL, IT JUST RESETS THE DATA (CODE ABOVE)
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, !isNew ? Pseudo : "new member");
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Pseudo);
        }

        public ICommand Delete { get; set; }

        private void DeleteAction()
        {
            App.Model.Clients.Remove(Client);
            App.Model.SaveChanges();

            App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Client.FullName);

            App.Messenger.NotifyColleagues(App.MSG_CLIENT_CHANGED, Client);
        }

        private bool CanDeleteAction()
        {
            return IsExisting;
        }

    }
}
