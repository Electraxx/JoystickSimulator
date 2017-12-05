using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace JoystickSimulator.Models
{
    public class AxisState: ICloneable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int H { get; set; }

        public AxisState()
        {
            X = 65535 / 2;
            Y = 65535 / 2;
            Z = 65535 / 2;
            H = 65535 / 2;
        }

        ////private AxisState(int x, int y, int z, int h)
        //{
        //    X = x;
        //    Y = y;
        //    Z = z;
        //    H = h;
        //}

        //public AxisState Copy() {
        //    return new AxisState(X,Y,Z,H);
        //}

        public void Set(JoystickUpdate button)
        {
            switch (button.Offset)
            {
                case JoystickOffset.X:
                    X = button.Value;
                    break;
                case JoystickOffset.Y:
                    Y = button.Value;
                    break;
                case JoystickOffset.Z:
                    Z = button.Value;
                    break;
                case JoystickOffset.Sliders0:
                    H = button.Value;
                    break;
            }
        }

        public object Clone() {
            return MemberwiseClone();
        }
    }
}
