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
        

        public ShowEdit(Show show, bool isNew = false)
        {
            Show = show;
            InitializeComponent();

            //App.Messenger.NotifyColleagues(App.MSG_CLIENT_RESERVATION, Client);
            /*reservationsView = new ReservationsView();
            Console.WriteLine(reservationsView);
            if (reservationsView == null)
                Console.WriteLine("reservationsView == null");
            else
                ReservationsPanel.Children.Add(reservationsView);*/

            DataContext = this;
            IsNew = isNew;

            Save = new RelayCommand(SaveAction, CanSaveOrCancelAction);
            Cancel = new RelayCommand(CancelAction);
            Delete = new RelayCommand(DeleteAction, CanDeleteAction);
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

        public string Name
        {
            get { return Show.Name; }
            set
            {
                Show.Name = value;
                RaisePropertyChanged(nameof(Name));
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



        public ICommand Save { get; set; }

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
                return !string.IsNullOrEmpty(Name) && !HasErrors;

            var change = (from c in App.Model.ChangeTracker.Entries<Show>()
                          where c.Entity == Show
                          select c).FirstOrDefault();

            return change != null && change.State != EntityState.Unchanged;
        }


        public ICommand Cancel { get; set; }

        private void CancelAction()
        {
            Console.WriteLine("Cancel");

            var change = (from c in App.Model.ChangeTracker.Entries<Show>()
                          where c.Entity == Show
                          select c).FirstOrDefault();

            if (change != null)
            {
                change.Reload();
                RaisePropertyChanged(nameof(Name));
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(Poster));
                RaisePropertyChanged(nameof(Date));
            }

            // NOT NEEDED TO CLOSE TAB ON CANCEL, IT JUST RESETS THE DATA (CODE ABOVE)
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, !isNew ? Pseudo : "new member");
            //App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Pseudo);
        }

        public ICommand Delete { get; set; }

        private void DeleteAction()
        {
            App.Model.Shows.Remove(Show);
            App.Model.SaveChanges();

            App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Show.Name);

            App.Messenger.NotifyColleagues(App.MSG_SHOW_CHANGED, Show);
        }

        private bool CanDeleteAction()
        {
            return IsExisting;
        }

    }
}
