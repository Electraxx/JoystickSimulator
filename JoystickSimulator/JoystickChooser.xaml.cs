using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JoystickSimulator.Packets;
using SharpDX.DirectInput;

namespace JoystickSimulator
{
    /// <summary>
    /// Interaction logic for JoystickChooser.xaml
    /// </summary>
    public partial class JoystickChooser : UserControl
    {
        public EventHandler JoystickSelectedEvent { get; set; }

        public EventHandler RefreshButttonPressed { get; set; }

        public JoystickChooser()
        {
            InitializeComponent();
        }

        private void ControlerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Joystick selectedController;

            try //Pas forcément nécéssaire, mais par sécurité
            {
                selectedController = (Joystick)ControlerListView.SelectedItem;
            }
            catch (InvalidCastException)
            {
                selectedController = null;
            }

            if (selectedController != null) //Fire de l'évenement dans MainWindow si on a selectionné un contrôleur
            {
                JoystickSelectedEvent(sender, new JoystickEventArgs(selectedController));
            }
        }

        public void bt_refreshControllers_Click(object sender, RoutedEventArgs e) //Fire de l'évenement dans MainWindow
        {
            RefreshButttonPressed(sender, new EventArgs());
        }

    }
}