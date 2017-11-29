using System;
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
        private Dictionary<JoystickOffset, int> lastAxisInput;
        public List<double> LastSize { get; private set; }

        public SimulatorController(ConfigManager cm, int defaultSensibility)
        {
            motionCalculation = new MotionCalculation(cm.Support, cm.Seat, cm.Offset, cm.RotationPoint, cm.MuscleMin, cm.MuscleMax, cm.VoltCurve);
            originalInstruction = new Instruction(0, 0, 0, motionCalculation.RotationPoint, new Vector3D(0, 0, 10));
            Simulator = new Simulator(cm.Seat, cm.RotationPoint);
            isOn = false;
            Sensibility = defaultSensibility;
            lastAction = null;
            lastAxisInput = new Dictionary<JoystickOffset, int> {
                {JoystickOffset.X, (int) 65535.0/2},
                {JoystickOffset.Y, (int) 65535.0/2},
                {JoystickOffset.Z, (int) 65535.0/2},
                {JoystickOffset.Sliders0, (int) 65535.0/2}
            };
            LastSize = motionCalculation.GetMuscleSize(Simulator.Seat);

            DAC.InitDac();
        }

        public void Do(InputAction action, Dictionary<JoystickOffset, int> axisInput)
        {
            switch (action.Name)
            {
                case "SwitchSimulatorState":
                    if (lastAction.Name != "SwitchSimulatorState") //manière plus propre de faire ?
                        isOn = !isOn;
                    break;
                case "MoveSimulator":
                    Move(axisInput);
                    lastAxisInput = axisInput;  //On stock les dernier inputs du mouvement afin de bien pouvoir bouger le simulateur quand on bouge son point de rotation
                    break;
                case "MoveRotationPoint":
                    Simulator.RotationPoint = new Point3D(
                        (axisInput[JoystickOffset.X] / 65535.0) * motionCalculation.RotationPoint.X,
                        (axisInput[JoystickOffset.Y] / 65535.0) * motionCalculation.RotationPoint.Y,
                        (axisInput[JoystickOffset.Sliders0] / 65535.0) * motionCalculation.RotationPoint.Z
                    );
                    Move(lastAxisInput);
                    break;
            }
            lastAction = action;
        }

        private void Move(Dictionary<JoystickOffset, int> axisInput)
        {

            double pitchInput = ((axisInput[JoystickOffset.Y] - (65535.0 / 2.0)) / 65535.0) * Sensibility;
            double rollInput = (((axisInput[JoystickOffset.X] - (65535.0 / 2.0)) / 65535.0) * Sensibility);
            double yawInput = ((axisInput[JoystickOffset.Z] - (65535.0 / 2.0)) / 65535.0) * -Sensibility;

            Instruction currentInstr = new Instruction(rollInput, pitchInput, yawInput, Simulator.RotationPoint, new Vector3D(0, 0, 10));

            List<double> sizeV = motionCalculation.GetMuscleSize(MotionCalculation.Transform(
                motionCalculation.Seat, motionCalculation.Oversampling(10, currentInstr, originalInstruction)));

            LastSize = sizeV;
            OutputVoltage(sizeV);
        }

        private void OutputVoltage(List<double> volts)
        {
            if (volts.Count == 6)
                DAC.OutputVoltage(volts.Select(i => i * (isOn ? 1.0 : 0.0)).ToList());
        }
    }
}