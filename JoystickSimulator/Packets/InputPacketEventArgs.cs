using System.Collections.Generic;
using SharpDX.DirectInput;

namespace JoystickSimulator.Packets
{
    internal class InputPacketEventArgs : System.EventArgs
    {
        /// <summary>
        /// Bouton pressé -> temps initial en ms
        /// </summary>
        //public Dictionary<int,double> ButtonPressed { get; private set; }
        //public Dictionary<string,int> JoystickHandleState { get; private set; }

        public InputPacketEventArgs(/*Dictionary<int, double> ButtonPressed, Dictionary<string, int> JoystickHandleState*/) {}
    }
}