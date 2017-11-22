using SharpDX.DirectInput;

namespace JoystickSimulator.Packets
{
    internal class JoystickEventArgs : System.EventArgs
    {
        public Joystick Joystick { get; set; }

        public JoystickEventArgs(Joystick joystick) {
            Joystick = joystick;
        }
    }
}