using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using JoystickSimulator.Controllers;
using JoystickSimulator.Models;
using JoystickSimulator.Packets;
using SharpDX.DirectInput;
using Key = System.Windows.Input.Key;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

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

            //Abonnement aux énénements
            JoystickChooserControl.JoystickSelectedEvent += new EventHandler(JoystickSelectedHandler);
            JoystickChooserControl.RefreshButttonPressed += new EventHandler(RefreshButtonPressedHandler);
            joyController.InputDataStored += new EventHandler(InputPacketSentHandler);
            ViewerControl.SliderValueChanged += new EventHandler(SliderValueChangedHandler);
            ViewerControl.FileButtonClicked += new EventHandler(FileButtonClickedHandler);

            //Binding de la view à la liste de joysticks
            JoystickChooserControl.ControlerListView.ItemsSource = joyController.ConnectedControllers;

            ViewerControl.SetSeatPoint(fileController.Cm.Seat);
            inputInterpreter = new InputInterpreter();

            visualizerTab.IsEnabled = false;

            //List<Tuple<int,string>> test = new List<Tuple<int, string>>();
            //test.Add(new Tuple<int, string>());
        }

        private void MoveViewerHandler(object sender, EventArgs e) {
            ViewerControl.Do(((MoveViewerEventArgs)e).InputAction, ((MoveViewerEventArgs)e).AxisState);
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

            simController.MoveViewerHandler += new EventHandler(MoveViewerHandler);
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

        private void InputPacketSentHandler(object sender, EventArgs e) //TODO Demander expl. prof
        {
            InputAction action = inputInterpreter.GetAction(joyController.InputValues);

            simController.Do(action, joyController.AxisState);
            ViewerControl.UpdateTextblocks(simController.LastSize);
            ViewerControl.Do(action, joyController.AxisState);
            
            fileController.Record(action, joyController.AxisState);
        }

        private void SliderValueChangedHandler(object sender, EventArgs e)
        {
            simController.Sensibility = ((SliderChangedEventArgs)e).Value;
            Console.WriteLine(simController.Sensibility);
        }

        private void FileButtonClickedHandler(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.DefaultExt = ".json";
            op.Filter = "Json File (*.json)|*.json";
            op.Title = "Save as";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                simController.InputFromJson(fileController.GetContent(op.FileName));
            }
        }

        private void visualizerTab_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F9: //Y'a moyen de faire plus propre (manager ou autre...)
                    ViewerControl.RecorderStateLabel.Content = fileController.SwitchRecorderState() ? "On" : "Off";
                    ViewerControl.IsAbleToSaveLabel.Content = fileController.GetRecorderState() ? "You cannot save" : "You can save";
                    break;
                case Key.F6: //Filecontroller ?
                    if (fileController.IsAbleToSave())
                    {
                        SaveFileDialog sd = new SaveFileDialog {
                            Filter = "Json File (*.json)|*.json",
                            FileName = "Recorded",
                            Title = "Save As"
                        };
                        if (sd.ShowDialog() == true)
                            fileController.SaveJson(sd.FileName);
                    }

                    break;

            }
        }
    }
}
