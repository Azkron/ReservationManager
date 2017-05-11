

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

            /*SandBox = new RelayCommand<string>((name) =>
            {
                WindowBase frm = null;
                switch (name)
                {
                    case "TestDataGrid":
                        frm = new TestDataGrid();
                        break;
                    case "TestMasterDetail":
                        frm = new TestMasterDetail();
                        break;
                }

                if (frm != null)
                    frm.ShowDialog();
                else
                    Console.WriteLine("no grid");
            });*/

            InitializeComponent();
            /*
            App.Messenger.Register(App.MSG_NEW_MEMBER,
                () =>
                {
                    // Create a new instance for a new member
                    var member = App.Model.Members.Create();
                    // Dyamically create a new tab with the title "<new member>"
                    var tab = new TabItem()
                    {
                        Header = "<new member>",
                        Content = new MemberDetailView(member, true)
                    };

                    AddTabControls(tab);

                    // Add the tab to the list of tabs existant int TabControl
                    tabControl.Items.Add(tab);
                    // Execute the tab´s Focus() method to give it focus (to activate it)
                    Dispatcher.InvokeAsync(() => tab.Focus());
                });

            App.Messenger.Register<string>(App.MSG_PSEUDO_CHANGED, (s) =>
            {
                (tabControl.SelectedItem as TabItem).Header = s;
            });*/

            /*App.Messenger.Register<Member>(App.MSG_SHOW_MEMBER, (m) =>
            {
                TabItem tab = null;
                string Header = "<" + m.Pseudo + ">";
                foreach (TabItem t in tabControl.Items)
                    if (t.Header.Equals(Header))
                        tab = t;

                if (tab == null)
                {
                    tab = new TabItem()
                    {
                        Header = "<" + m.Pseudo + ">",
                        Content = new MemberDetailView(m, false)
                    };

                    AddTabControls(tab);

                    tabControl.Items.Add(tab);
                }

                Dispatcher.InvokeAsync(() => tab.Focus());
            });
            */
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
