﻿

using PRBD_Framework;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReservationManager
{
    public partial class MainView : WindowBase
    {
        public ICommand SandBox { get; set; }

        public MainView()
        {
            DataContext = this;

            InitializeComponent();

            // CLIENT EDIT COMMANDS
            App.Messenger.Register(App.MSG_NEW_CLIENT, () =>
            {
                var client = App.Model.Clients.Create();
                var tab = new TabItem()
                {
                    Header = "<new client>",
                    Content = new ClientEdit(client, true)
                };

                AddTabControls(tab);
                
                tabControl.Items.Add(tab);
                Dispatcher.InvokeAsync(() => tab.Focus());
            });

            App.Messenger.Register<Client>(App.MSG_CLIENT_CHANGED, (c) =>
            {
                (tabControl.SelectedItem as TabItem).Header = "<" + c.FullName + ">";
            });

            App.Messenger.Register<Client>(App.MSG_EDIT_CLIENT, (c) =>
            {
                if(c != null)
                {
                    TabItem tab = null;
                    string Header = "<" + c.FullName + ">";
                    foreach (TabItem t in tabControl.Items)
                        if (t.Header.Equals(Header))
                            tab = t;

                    if (tab == null)
                    {
                        var clientEdit = new ClientEdit(c, false);
                        tab = new TabItem()
                        {
                            Header = "<" + c.FullName + ">",
                            Content = clientEdit
                        };


                        AddTabControls(tab, () => { if (clientEdit.CanSaveOrCancelAction()) clientEdit.CancelAction(); });

                        tabControl.Items.Add(tab);
                    }

                    Dispatcher.InvokeAsync(() => tab.Focus());

                }
            });
            

            // SHOW EDIT COMMANDS
            App.Messenger.Register(App.MSG_NEW_SHOW, () =>
            {
                var show = App.Model.Shows.Create();
                var tab = new TabItem()
                {
                    Header = "<new show>",
                    Content = new ShowEdit(show, true)
                };

                AddTabControls(tab);

                tabControl.Items.Add(tab);
                Dispatcher.InvokeAsync(() => tab.Focus());
            });

            App.Messenger.Register<Show>(App.MSG_SHOW_CHANGED, (s) =>
            {
                (tabControl.SelectedItem as TabItem).Header = "<" + s.Name + ">";
            });

            App.Messenger.Register<Show>(App.MSG_EDIT_SHOW, (s) =>
            {
                if (s != null)
                {
                    TabItem tab = null;
                    string Header = "<" + s.Name + ">";
                    foreach (TabItem t in tabControl.Items)
                        if (t.Header.Equals(Header))
                            tab = t;

                    if (tab == null)
                    {
                        var editShow = new ShowEdit(s, false);
                        tab = new TabItem()
                        {
                            Header = "<" + s.Name + ">",
                            Content = editShow
                        };

                        AddTabControls(tab, () => { if (editShow.CanSaveOrCancelAction()) editShow.CancelAction(); });

                        tabControl.Items.Add(tab);
                    }

                    Dispatcher.InvokeAsync(() => tab.Focus());
                }
            });


            // RESERVATION EDIT COMMANDS
            App.Messenger.Register(App.MSG_NEW_RESERVATION, () =>
            {
                var reservation = App.Model.Reservations.Create();
                var tab = new TabItem()
                {
                    Header = "<new reservation>",
                    Content = new ReservationEdit(reservation, true)
                };

                AddTabControls(tab);

                tabControl.Items.Add(tab);
                Dispatcher.InvokeAsync(() => tab.Focus());
            });

            App.Messenger.Register<Client>(App.MSG_NEW_CLIENT_RESERVATION, (c) =>
            {
                if (c != null)
                {
                    var reservation = App.Model.Reservations.Create();
                    reservation.Client = c;
                    var tab = new TabItem()
                    {
                        Header = "<new reservation>",
                        Content = new ReservationEdit(reservation, true)
                    };

                    AddTabControls(tab);

                    tabControl.Items.Add(tab);
                    Dispatcher.InvokeAsync(() => tab.Focus());
                }
            });

            App.Messenger.Register<Show>(App.MSG_NEW_SHOW_RESERVATION, (s) =>
            {
                if (s != null)
                {
                    if(s.FreePlacesTotal <= 0)
                    {
                        MessageBoxResult result = MessageBox.Show(Properties.Resources.NoPlaces, Properties.Resources.NotPossible, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var reservation = App.Model.Reservations.Create();
                        reservation.Show = s;
                        var tab = new TabItem()
                        {
                            Header = "<new reservation>",
                            Content = new ReservationEdit(reservation, true)
                        };

                        AddTabControls(tab);

                        tabControl.Items.Add(tab);
                        Dispatcher.InvokeAsync(() => tab.Focus());
                    }
                }
            });

            App.Messenger.Register<Reservation>(App.MSG_RESERVATION_CHANGED, (r) =>
            {
                (tabControl.SelectedItem as TabItem).Header = r.TabHeader;
            });

            App.Messenger.Register<Reservation>(App.MSG_EDIT_RESERVATION, (r) =>
            {
                if (r != null)
                {
                    TabItem tab = null;
                    string Header = r.TabHeader;
                    foreach (TabItem t in tabControl.Items)
                        if (t.Header.Equals(Header))
                            tab = t;

                    if (tab == null)
                    {
                        var resEdit = new ReservationEdit(r, false);
                        tab = new TabItem()
                        {
                            Header = r.TabHeader,
                            Content = resEdit
                        };

                        AddTabControls(tab, () => { if (resEdit.CanSaveOrCancelAction()) resEdit.CancelAction(); });

                        tabControl.Items.Add(tab);
                    }

                    Dispatcher.InvokeAsync(() => tab.Focus());
                }
            });



            // CLOSE TABS
            App.Messenger.Register<string>(App.MSG_CLOSE_TAB, (pseudo) =>
            {
                string tHeader = "<" + pseudo + ">";
                foreach (TabItem t in tabControl.Items)
                    if (t.Header.Equals(tHeader))
                    {
                        tabControl.Items.Remove(t);
                        break;
                    }
            });
        }

        private void AddTabControls(TabItem tab, Action act = null)
        {
            tab.MouseDown += (o, e) =>
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Middle && e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
                {
                    act?.Invoke();

                    tabControl.Items.Remove(o);
                }
            };

            tab.KeyDown += (o, e) =>
            {
                if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl))
                    tabControl.Items.Remove(o);
            };
        }

    }

}
