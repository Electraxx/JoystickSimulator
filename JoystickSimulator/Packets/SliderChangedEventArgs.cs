using System;

namespace JoystickSimulator.Packets
{
    internal class SliderChangedEventArgs : EventArgs
    {
        public double Value { get; set; }

        public SliderChangedEventArgs(double value)
        {
            Value = value;
        }
    }
}
