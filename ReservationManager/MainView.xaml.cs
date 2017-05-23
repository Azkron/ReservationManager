

using PRBD_Framework;
using System;
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
                TabItem tab = null;
                string Header = "<" + c.FullName + ">";
                foreach (TabItem t in tabControl.Items)
                    if (t.Header.Equals(Header))
                        tab = t;

                if (tab == null)
                {
                    tab = new TabItem()
                    {
                        Header = "<" + c.FullName + ">",
                        Content = new ClientEdit(c, false)
                    };

                    AddTabControls(tab);

                    tabControl.Items.Add(tab);
                }

                Dispatcher.InvokeAsync(() => tab.Focus());
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
                TabItem tab = null;
                string Header = "<" + s.Name + ">";
                foreach (TabItem t in tabControl.Items)
                    if (t.Header.Equals(Header))
                        tab = t;

                if (tab == null)
                {
                    tab = new TabItem()
                    {
                        Header = "<" + s.Name + ">",
                        Content = new ShowEdit(s, false)
                    };

                    AddTabControls(tab);

                    tabControl.Items.Add(tab);
                }

                Dispatcher.InvokeAsync(() => tab.Focus());
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

        private void AddTabControls(TabItem tab)
        {
            tab.MouseDown += (o, e) =>
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Middle && e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
                    tabControl.Items.Remove(o);
            };

            tab.KeyDown += (o, e) =>
            {
                if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl))
                    tabControl.Items.Remove(o);
            };
        }

    }

}
