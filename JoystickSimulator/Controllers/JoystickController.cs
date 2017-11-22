using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
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
        public Joystick CurrentJoystick {
            get => currentJoystick;
            set {
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

        public Dictionary<int,double> InputValues { get; set; }

        public EventHandler InputPacketSent { get; set; }
        
        private readonly List<int> xyzOffsetList = new List<int> { (int)JoystickOffset.X, (int)JoystickOffset.Y, (int)JoystickOffset.Z } ;

        private ObservableCollection<Joystick> connectedControllers;
        #endregion

        public JoystickController()
        {
            ConnectedControllers = new ObservableCollection<Joystick>();
            connectedControllers = new ObservableCollection<Joystick>();
            InputValues = new Dictionary<int, double>();
            di = new DirectInput();
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
            InputPacketSent(sender, new InputPacketEventArgs(controlerData));
        }
    }
}