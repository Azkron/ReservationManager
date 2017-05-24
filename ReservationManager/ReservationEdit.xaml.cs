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

        private ClientsView Clients = null;

        public ReservationEdit(Reservation reservation, bool isNew = false)
        {
            Reservation = reservation;
            InitializeComponent();

            DataContext = this;

            ReadOnly = App.Rights(Table.SHOW) != Right.ALL;
            IsNew = isNew;

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
                else if (!ReadOnly)
                {
                    btnDelete.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
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
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                }

                RaisePropertyChanged(nameof(ReadOnly));
            }
        }
    }
}
