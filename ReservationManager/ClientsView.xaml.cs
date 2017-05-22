using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ClientsView.xaml
    /// </summary>
    public partial class ClientsView : UserControlBase
    {

        public ICommand ClearFilter { get; set; }

        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public bool IsValid { get; set; }
        
        public ClientsView()
        {
            InitializeComponent();

            DataContext = this;

            Clients = new MyObservableCollection<Client>(App.Model.Clients);

            ClearFilter = new RelayCommand(() => { NameFilter = ""; PostalCodeFilter = ""; });
            NewCommand = new RelayCommand(() => { App.Messenger.NotifyColleagues(App.MSG_NEW_CLIENT); });
            EditCommand = new RelayCommand<Client>((c) => { App.Messenger.NotifyColleagues(App.MSG_EDIT_CLIENT, c); });


            RefreshCommand = new RelayCommand(() =>
            {
                App.CancelChanges();
                Clients.Refresh(App.Model.Clients);
            },
            () => { return IsValid; });
        }

        public MyObservableCollection<Client> clients;
        public MyObservableCollection<Client> Clients
        {
            get { return clients; }
            set
            {
                clients = value;
                RaisePropertyChanged(nameof(Clients));
            }
        }

        Client selectedClient;
        public Client SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;
                Console.WriteLine(selectedClient);
            }
        }


        private string nameFilter;
        public string NameFilter
        {
            get { return nameFilter; }
            set
            {
                nameFilter = value;
                ApplyFilterAction();
                RaisePropertyChanged(nameof(NameFilter));
            }
        }

        private string postalCodeFilter;
        public string PostalCodeFilter
        {
            get { return postalCodeFilter; }
            set
            {
                postalCodeFilter = Regex.Replace(value, "[^0-9.]", "");
                if(postalCodeFilter.Length > 4)
                    postalCodeFilter = postalCodeFilter.Substring(0, 4);
                if (!string.IsNullOrEmpty(PostalCodeFilter))
                    Console.WriteLine(Int32.Parse(PostalCodeFilter));

                ApplyFilterAction();
                RaisePropertyChanged(nameof(PostalCodeFilter));
            }
        }

        private void ApplyFilterAction()
        {
            if (!string.IsNullOrEmpty(NameFilter) || !string.IsNullOrEmpty(PostalCodeFilter))
            {
                IQueryable<Client> filtered = App.Model.Clients;

                if (!string.IsNullOrEmpty(NameFilter))
                {
                    filtered = from c in filtered
                               where c.FirstName.Contains(NameFilter) || c.LastName.Contains(NameFilter)
                               select c;
                }

                if (!string.IsNullOrEmpty(PostalCodeFilter))
                {
                    int? pCFilter = Convert.ToInt32(PostalCodeFilter);
                    filtered = from c in filtered
                               where c.PostalCode == pCFilter
                               select c;
                }

                Clients = new MyObservableCollection<Client>(filtered, App.Model.Clients);
            }
            else
                Clients = new MyObservableCollection<Client>(App.Model.Clients);
        }
        
    }
}
