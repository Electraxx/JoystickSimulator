using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoystickSimulator.Errors
{
    internal class AlreadyRecordingException:Exception
    {
        public AlreadyRecordingException():base("You can't record if the recorder isn't off") {}
    }
}
