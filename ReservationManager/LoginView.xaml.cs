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
using System.Windows.Shapes;

namespace ReservationManager
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : WindowBase
    {
        private string pseudo;

        private string password;

        public string Password { get { return password; } set { password = value; Validate(); } }

        public string Pseudo { get { return pseudo; } set { pseudo = value; Validate(); } }

        public ICommand Login { get; set; }
        public ICommand Cancel { get; set; }

        public LoginView()
        {
            InitializeComponent();

            Login = new RelayCommand(LoginAction, () => { return pseudo != null && password != null && !HasErrors; });
            Cancel = new RelayCommand(() => Close());
            DataContext = this;
        }



        private void LoginAction()
        {
            var user = Validate(); // we look for a member
            if (!HasErrors)
            {
                App.CurrentUser = user;//the connecting member becomes the current member
                ShowMainView(); // Open the main window
                Close(); // close this window

            }
        }

        private static void ShowMainView()
        {
            var mainView = new MainView();
            mainView.Show();
            Application.Current.MainWindow = mainView;
        }

        private User Validate()
        {
            ClearErrors();
            //var user = App.Model.Users.Find(Pseudo);
            //var user = from u in App.Model.Users where u.Login == Pseudo select u;
            //Enumerator user = App.Model.Users.Where(u => u.Login == Pseudo).GetEnumerator();
            User user = App.Model.Users.FirstOrDefault(u => u.Login == Pseudo);

            if (string.IsNullOrEmpty(Pseudo))
            {
                AddError("Pseudo", Properties.Resources.Error_Required);
            }
            else if (Pseudo != null)
            {
                if (Pseudo.Length < 3)
                    AddError("Pseudo", Properties.Resources.Error_LengthGreaterEqual3);
                else if (user == null)
                    AddError("Pseudo", Properties.Resources.Error_DoesNotExist);
                else if (Password != user.Pwd)
                    AddError("Password", Properties.Resources.Error_MustMatchPassword);
            }

            RaiseErrors();

            return user;
        }
    }
}
