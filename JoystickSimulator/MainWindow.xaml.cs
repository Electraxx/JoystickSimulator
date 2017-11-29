using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JoystickSimulator.Controllers;
using JoystickSimulator.Models;
using JoystickSimulator.Packets;
using SharpDX.DirectInput;

namespace JoystickSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// EventHandlers
        /// </summary>
        private EventHandler joystickChoosed;
        private EventHandler refrsehButtonPressed;
        private EventHandler InputPacketSent;
        /// <summary>
        /// Contrôleurs
        /// </summary>
        private JoystickController joyController;
        private FileController fileController;
        private InputInterpreter inputInterpreter;
        private SimulatorController simController;

        public MainWindow()
        {
            InitializeComponent();

            //Création des contrôleurs
            joyController = new JoystickController();
            fileController = new FileController();
            //mathController = new MathController(fileController.Cm);

            //Abonnement aux énénements
            JoystickChooserControl.JoystickSelectedEvent += new EventHandler(JoystickSelectedHandler);
            JoystickChooserControl.RefreshButttonPressed += new EventHandler(RefreshButtonPressedHandler);
            joyController.InputDataStored += new EventHandler(InputPacketSentHandler);

            //Binding de la view à la liste de joysticks
            JoystickChooserControl.ControlerListView.ItemsSource = joyController.ConnectedControllers;

            ViewerControl.SetSeatPoint(fileController.Cm.Seat);
            inputInterpreter = new InputInterpreter();

            visualizerTab.IsEnabled = false;
        }

        /// <summary>
        /// Est appelé quand un joystick est selectionné dans le contrôle JoystickChooser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JoystickSelectedHandler(object sender, EventArgs e)
        {
            joyController.CurrentJoystick = ((JoystickEventArgs)e).Joystick;

            if (joyController.CurrentJoystick == null)
                return; //vérification

            simController = new SimulatorController(fileController.Cm, (int)ViewerControl.sensibilitySlider.Value);
            ViewerControl.RotationPointLabel.DataContext = simController.Simulator.RotationPoint;

            visualizerTab.IsEnabled = true;
            tabControler.SelectedValue = visualizerTab;
        }

        /// <summary>
        /// Est appelé quand le bouton refresh est pressé dans le contrôle JoystickChooser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButtonPressedHandler(object sender, EventArgs e)
        {
            joyController.RefreshJoyStickList();
        }

        private void InputPacketSentHandler(object sender, EventArgs e)
        {
            InputAction action = inputInterpreter.GetAction(joyController.InputValues);

            simController.Do(action, joyController.AxisValues);
            ViewerControl.UpdateTextblocks(simController.LastSize);
            ViewerControl.Do(action, joyController.AxisValues);
        }
    }
}