using SharpDX.DirectInput;

namespace JoystickSimulator.Packets
{
    /// <summary>
    /// Event utilisé quand un joystick est choisi
    /// </summary>
    internal class JoystickEventArgs : System.EventArgs
    {
        public Joystick Joystick { get; set; }

        public JoystickEventArgs(Joystick joystick) {
            Joystick = joystick;
        }
    }
}