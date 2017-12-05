﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using JoystickSimulator.Helpers;
using JoystickSimulator.Models;
using SharpDX.DirectInput;

namespace JoystickSimulator.Controllers
{
    class SimulatorController
    {

        private MotionCalculation motionCalculation;
        private DAC dac;
        public Simulator Simulator { get; set; }
        private readonly Instruction originalInstruction;
        private bool isOn;

        public double Sensibility { get; set; }
        private InputAction lastAction;
        private AxisState lastAxisInput;
        public List<double> LastSize { get; private set; }

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
                        (axisState.H / 65535.0) * motionCalculation.RotationPoint.Z
                    );
                    Move(lastAxisInput);
                    break;
            }
            lastAction = action;
        }

        private void Move(AxisState axisState)
        {

            double pitchInput = ((axisState.Y - (65535.0 / 2.0)) / 65535.0) * Sensibility;
            double rollInput = (((axisState.X - (65535.0 / 2.0)) / 65535.0) * Sensibility);
            double yawInput = ((axisState.Z - (65535.0 / 2.0)) / 65535.0) * -Sensibility;

            Instruction currentInstr = new Instruction(rollInput, pitchInput, yawInput, Simulator.RotationPoint, new Vector3D(0, 0, 10));

            List<double> sizeV = motionCalculation.GetMuscleSize(MotionCalculation.Transform(
                motionCalculation.Seat, motionCalculation.Oversampling(10, currentInstr, originalInstruction)));

            LastSize = sizeV;
            OutputVoltage(sizeV);
        }

        private void OutputVoltage(List<double> volts) //TODO SLIDEEEER
        {
            if (volts.Count == 6)
                DAC.OutputVoltage(volts.Select(i => i * (isOn ? 1.0 : 0.0)).ToList());
        }
    }
}