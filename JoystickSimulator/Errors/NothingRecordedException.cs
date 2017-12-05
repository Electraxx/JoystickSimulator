using System;

namespace JoystickSimulator.Models
{
    internal class NothingRecordedException : Exception {
        public NothingRecordedException() : base("Nothing was recorded"){}
    }
}