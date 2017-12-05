using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoystickSimulator.Errors;
using Newtonsoft.Json;
using SharpDX.DirectInput;

namespace JoystickSimulator.Models
{
    internal class ActionRecorder
    {
        public ActionSequence ActionList { get; private set; }
        public bool IsRecording { get; set; }

        public ActionRecorder()
        {
            ActionList = new ActionSequence();
            IsRecording = false;
        }

        public void Record(InputAction action, AxisState axisState)
        {
            if (IsRecording)
                ActionList.Add(new InputPair(action, axisState));
        }

        /// <summary>
        /// Inverse l'état du recorder et renvoie l'état actuel
        /// </summary>
        /// <returns></returns>
        public bool SwitchRecorderState()
        {
            return IsRecording = !IsRecording;
        }

        public String GetJson()
        {
            if (!IsRecording)
                return JsonConvert.SerializeObject(ActionList);

            if (ActionList.Count > 0)
                throw new NothingRecordedException();

            throw new AlreadyRecordingException();
        }
    }

    internal class ActionSequence : List<InputPair>
    {

    }

    internal class InputPair : Tuple<InputAction, AxisState>
    {
        public InputPair(InputAction item1, AxisState item2) : base(item1, item2) { }
    }
}
