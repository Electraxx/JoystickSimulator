using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoystickSimulator.Packets
{
    class SliderChangedEventArgs : EventArgs
    {
        public double Value { get; set; }

        public SliderChangedEventArgs(double value) {
            Value = value;
        }
    }
}
