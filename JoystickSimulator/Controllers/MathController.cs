using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using JoystickSimulator.Helpers;
using JoystickSimulator.Models;

namespace JoystickSimulator.Controllers
{
    internal class MathController {

        private List<Point3D> seatPoints;
        private MotionCalculation mc;

        public MathController(ConfigManager cm) {
            this.seatPoints = cm.Seat;
            mc = new MotionCalculation(cm.Support,cm.Seat,cm.Offset,cm.RotationPoint,cm.MuscleMin,cm.MuscleMax,cm.VoltCurve);
        }
    }
}
