using System;

namespace JoystickSimulator.Packets
{
    /// <summary>
    /// Event utilisé quand le Slider de sensibilité est déplacé
    /// </summary>
    internal class SliderChangedEventArgs : EventArgs
    {
        public double Value { get; set; }

        public SliderChangedEventArgs(double value)
        {
            Value = value;
        }
    }
}
