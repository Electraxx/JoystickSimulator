﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using JoystickSimulator.Helpers;
using JoystickSimulator.Models;
using JoystickSimulator.Packets;
using Newtonsoft.Json;
using SharpDX.DirectInput;

namespace JoystickSimulator.Controllers
{
    /// <summary>
    /// Contrôle les interactions avec le simulateur
    /// </summary>
    class SimulatorController
    {
        //TODO, mettre 2,3 trucs dans le math controlleurs
        private MotionCalculation motionCalculation;
        //private DAC dac;
        public Simulator Simulator { get; set; }
        private readonly Instruction originalInstruction;
        private bool isOn;

        public double Sensibility { get; set; }
        private InputAction lastAction;
        private AxisState lastAxisInput;
        public List<double> LastSize { get; private set; }

        public EventHandler MoveViewerHandler;

        public SimulatorController(ConfigManager cm, int defaultSensibility)
        {
            motionCalculation = new MotionCalculation(cm.Support, cm.Seat, cm.Offset, cm.RotationPoint, cm.MuscleMin, cm.MuscleMax, cm.VoltCurve);
            originalInstruction = new Instruction(0, 0, 0, motionCalculation.RotationPoint, new Vector3D(0, 0, 10));
            Simulator = new Simulator(cm.Seat, cm.RotationPoint);
            isOn = false;
            Sensibility = defaultSensibility;
            lastAction = null;
            lastAxisInput = new AxisState();
            LastSize = motionCalculation.GetMuscleSize(Simulator.Seat);

            DAC.InitDac();
        }

        /// <summary>
        /// Methode d'interaction principale. Elle va détérminer ce qu'il faudra faire
        /// </summary>
        /// <param name="action">Action à faire</param>
        /// <param name="axisState">Position du joystick à intrepréter</param>
        public void Do(InputAction action, AxisState axisState)
        {
            switch (action.Name)
            {
                case "SwitchSimulatorState":
                    if (lastAction.Name != "SwitchSimulatorState") //manière plus propre de faire ?
                        isOn = !isOn;
                    break;
                case "MoveSimulator":
                    Move(axisState);
                    lastAxisInput = axisState;  //On stock les dernier inputs du mouvement afin de bien pouvoir bouger le simulateur quand on bouge son point de rotation
                    break;
                case "MoveRotationPoint":
                    Simulator.RotationPoint = new Point3D(
                        (axisState.X / 65535.0) * motionCalculation.RotationPoint.X,
                        (axisState.Y / 65535.0) * motionCalculation.RotationPoint.Y,
                        (axisState.H / 1100) * motionCalculation.RotationPoint.Z //TODO find max usable value
                    );
                    Move(lastAxisInput);
                    Console.WriteLine("Height : " + axisState.H / 1100);
                    break;

                //INFO 76x91 
            }
            lastAction = action;
        }

        /// <summary>
        ///Permet de bouger le simulateur 
        /// </summary>
        /// <param name="axisState">Position actuelle du joystick</param>
        private void Move(AxisState axisState)
        {

            double pitchInput = ((axisState.Y - (65535.0 / 2.0)) / 65535.0) * Sensibility;
            double rollInput = (((axisState.X - (65535.0 / 2.0)) / 65535.0) * Sensibility);
            double yawInput = ((axisState.Z - (65535.0 / 2.0)) / 65535.0) * -Sensibility;

            Instruction currentInstr = new Instruction(rollInput, pitchInput, yawInput, Simulator.RotationPoint, new Vector3D(0, 0, 10));

            List<double> sizeV = motionCalculation.GetMuscleSize(MotionCalculation.Transform(
                motionCalculation.Seat, motionCalculation.Oversampling(10, currentInstr, originalInstruction)));

            OutputVoltage(sizeV);
            LastSize = sizeV;
        }

        /// <summary>
        /// Va donner le voltage aux éléctrovannes
        /// </summary>
        /// <param name="size">Liste des size à distribuer</param>
        private void OutputVoltage(List<double> size)
        {

            if (size.Count == 6)
                DAC.OutputVoltage(motionCalculation.DistsToVolts(size.Select(i => i * (isOn ? 1.0 : 0.0)).ToList()));
        }

        /// <summary>
        /// Permet de simuler l'input avec un fichier Json
        /// </summary>
        /// <param name="json">Fichier Json contenant les inputs</param>
        public async void InputFromJson(string json)
        {
            ActionSequence acSequence = JsonConvert.DeserializeObject<ActionSequence>(json);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (InputPair pair in acSequence)
            {
                //Dispatcher.CurrentDispatcher.Invoke(() => { MoveViewerHandler(this, new MoveViewerEventArgs(pair.Item1, pair.Item2)); }, DispatcherPriority.ContextIdle);
                MoveViewerHandler(this, new MoveViewerEventArgs(pair.Item1, pair.Item2));
                Do(pair.Item1, pair.Item2);
                sw.Stop();
                //Console.WriteLine("Action : "+ pair.Item1.Name + " Temps (ms) : "+ (int)sw.ElapsedMilliseconds);
                int milisec = (int)sw.ElapsedMilliseconds;
                sw.Restart();
                await Task.Delay(33 - ((milisec > 33) ? 0 : milisec)); //30? 50? async ?
            }
        }
        #region oldversion
        //DispatcherTimer timer = new DispatcherTimer(); //Faisable en une ligne ?
        //timer.Interval = new TimeSpan(0, 0, 0, 0, 33);
        //timer.Tick += (sender, e) =>
        //{
        //    foreach (InputPair pair in acSequence) { //ici
        //        MoveViewerHandler(this, new MoveViewerEventArgs(pair.Item1, pair.Item2));
        //        Do(pair.Item1, pair.Item2);
        //        //yield return;
        //    }
        //};
        //timer.Start();//Yield + timer = prefect

        //Thread jsonInput = new Thread(() => { //Thread ici ou dans mainwindow ?
        //    ActionSequence acSequence = JsonConvert.DeserializeObject<ActionSequence>(json);
        //Stopwatch sw = new Stopwatch();

        //foreach (InputPair pair in acSequence)
        //{
        //    sw.Start();
        //    Dispatcher.CurrentDispatcher.Invoke(() => { MoveViewerHandler(this, new MoveViewerEventArgs(pair.Item1, pair.Item2)); }, DispatcherPriority.ContextIdle);
        //    Do(pair.Item1,pair.Item2);
        //    sw.Stop();
        //    Thread.Sleep(33-(int)sw.ElapsedMilliseconds);
        //    sw = new Stopwatch();
        //    //yield return 
        //}
        //});

        //jsonInput.Start();// Start a new thread
        #endregion
    }
}