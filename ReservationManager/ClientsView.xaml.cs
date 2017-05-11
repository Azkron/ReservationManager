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
    /// Interaction logic for ClientsView.xaml
    /// </summary>
    public partial class ClientsView : UserControlBase
    {


        public MyObservableCollection<Client> Clients { get; private set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public bool IsValid { get; set; }

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

        public ClientsView()
        {
            InitializeComponent();

            DataContext = this;

            Clients = new MyObservableCollection<Client>(App.Model.Client);



            SaveCommand = new RelayCommand(() => { App.Model.SaveChanges(); }, () => { return IsValid; });

            RefreshCommand = new RelayCommand(() =>
            {
                App.CancelChanges();
                Clients.Refresh(App.Model.Client);
            },
            () => { return IsValid; });
        }
    }
}
