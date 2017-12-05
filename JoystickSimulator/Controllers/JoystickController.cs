using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using JoystickSimulator.Models;
using JoystickSimulator.Packets;
using SharpDX.DirectInput;

namespace JoystickSimulator.Controllers
{
    internal class JoystickController
    {
        #region variables
        /// <summary>
        /// Représente le joystick utilisé
        /// </summary>
        private Joystick currentJoystick;
        public Joystick CurrentJoystick
        {
            get => currentJoystick;
            set
            {
                if (value == null) return;

                currentJoystick = value;

                if (currentJoystick.Properties.BufferSize != 128)
                    currentJoystick.Properties.BufferSize = 128;

                currentJoystick.Acquire();

                DispatcherTimer timer = new DispatcherTimer(); //Faisable en une ligne ?
                timer.Interval = new TimeSpan(0, 0, 0, 0, 33);
                timer.Tick += AcquireInput;
                timer.Start();
            }
        }

        /// <summary>
        /// Classe permettant (entre autre) de trouver les joysticks
        /// </summary>
        private DirectInput di;

        /// <summary>
        /// Représente les joysticks connectés
        /// </summary>
        public ObservableCollection<Joystick> ConnectedControllers
        {
            get
            {
                RefreshJoyStickList();
                return connectedControllers;
            }
            private set => connectedControllers = value;
        }

        public Dictionary<JoystickOffset, double> InputValues { get; private set; } //TODO remplacer par type
        public AxisState AxisState { get; private set; }

        public EventHandler InputDataStored { get; set; }

        private readonly List<JoystickOffset> xyzOffsetList = new List<JoystickOffset> { JoystickOffset.X, JoystickOffset.Y, JoystickOffset.Z, JoystickOffset.Sliders0 };

        private ObservableCollection<Joystick> connectedControllers;
        #endregion

        public JoystickController()
        {          
            ConnectedControllers = new ObservableCollection<Joystick>();
            di = new DirectInput();
            InputValues = new Dictionary<JoystickOffset, double>();
            AxisState = new AxisState();
        }

        /// <summary>
        /// Va mettre à jour la liste des joystick connectés
        /// </summary>
        public void RefreshJoyStickList()
        {
            connectedControllers.Clear();
            foreach (DeviceInstance deviceInstance in di.GetDevices())
            {
                DeviceType type = deviceInstance.Type;
                if (type == DeviceType.Joystick) //Linq possible, à voir
                    connectedControllers.Add(new Joystick(di, deviceInstance.InstanceGuid));
            }
        }

        private void AcquireInput(object sender, EventArgs e)
        {
            //JoystickUpdate[] ControlerData = CurrentJoystick.GetBufferedData();

            var controlerData = CurrentJoystick.GetBufferedData();

            foreach (var action in controlerData)
            {
                if (!xyzOffsetList.Contains(action.Offset))
                { //Boutons
                    if (action.Value == 128)
                        InputValues[action.Offset] = DateTime.Now.TimeOfDay.TotalMilliseconds;
                    else if (action.Value == 0)
                        InputValues.Remove(action.Offset);
                }
                else //Axis
                {
                    AxisState.Set(action);
                }
                Console.WriteLine(action);
            }
            //Les données ont été analysées, on fire l'event du MainWindow
            InputDataStored(sender, new InputPacketEventArgs());
        }
    }
}