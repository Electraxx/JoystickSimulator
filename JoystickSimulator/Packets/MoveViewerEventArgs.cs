using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoystickSimulator.Models;

namespace JoystickSimulator.Packets
{
    /// <summary>
    /// Event utilisé pour remonter la hiérarchie afin de bouger la partie visuelle
    /// </summary>
    class MoveViewerEventArgs :EventArgs
    {
        public InputAction InputAction { get; set; }
        public AxisState AxisState { get; set; }

        public MoveViewerEventArgs(InputAction ia, AxisState axis) {
            AxisState = axis;
            InputAction = ia;
        }
    }
}
