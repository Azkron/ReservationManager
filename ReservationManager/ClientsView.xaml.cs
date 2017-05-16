using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ClientsView.xaml
    /// </summary>
    public partial class ClientsView : UserControlBase
    {

        public ICommand ClearFilter { get; set; }
        
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public bool IsValid { get; set; }
        
        public ClientsView()
        {
            InitializeComponent();

            DataContext = this;

            Clients = new MyObservableCollection<Client>(App.Model.Clients);

            ClearFilter = new RelayCommand(() => { NameFilter = ""; });


            SaveCommand = new RelayCommand(() => { App.Model.SaveChanges(); }, () => { return IsValid; });

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
                // this changes the content of filter in view if it was changed in the code (using clear() for instance)
            }
        }
        
        private void ApplyFilterAction()
        {
            if (!string.IsNullOrEmpty(NameFilter))
            {
                var filtered = from m in App.Model.Clients
                               where m.FirstName.Contains(NameFilter) || m.LastName.Contains(NameFilter)
                               select m;

                Clients = new MyObservableCollection<Client>(filtered, App.Model.Clients);
            }
            else
                Clients = new MyObservableCollection<Client>(App.Model.Clients);
        }
        
    }
}
