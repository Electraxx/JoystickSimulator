using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoystickSimulator.Models;

namespace JoystickSimulator.Packets
{
    class MoveViewerEventArgs:EventArgs
    {
        public InputAction InputAction { get; set; }
        public AxisState AxisState { get; set; }

        public MoveViewerEventArgs(InputAction ia, AxisState axis) {
            AxisState = axis;
            InputAction = ia;
        }
    }
}
