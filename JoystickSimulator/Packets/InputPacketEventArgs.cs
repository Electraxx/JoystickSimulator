using SharpDX.DirectInput;

namespace JoystickSimulator.Packets
{
    internal class InputPacketEventArgs : System.EventArgs
    {
        public JoystickUpdate[] ControlerData { get; private set; }

        public InputPacketEventArgs(JoystickUpdate[] controlerData)
        {
            ControlerData = controlerData;
        }
    }
}