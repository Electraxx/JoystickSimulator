using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JoystickSimulator.Errors;
using Newtonsoft.Json;
using SharpDX.DirectInput;

namespace JoystickSimulator.Models
{
    /// <summary>
    /// Permet d'enregistrer les actions faites sur le simulateur
    /// </summary>
    internal class ActionRecorder
    {
        /// <summary>
        /// Liste contenant les actions/positions du joystick
        /// </summary>
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
            {
                ActionList.Add(new InputPair(action, axisState));
                //Console.WriteLine("Recorded: \t X : " + axisState.X+"\n\tY : "+ axisState.Y+"\n\t Z : "+ axisState.Z);
            }

        }

        /// <summary>
        /// Inverse l'état du recorder, renvoie l'état actuel et réinitialise la liste
        /// </summary>
        /// <returns></returns>
        public bool SwitchRecorderState()
        {
            if(!IsRecording)
                ActionList.Clear();
            return IsRecording = !IsRecording;
        }

        public string GetJson()
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
        public InputPair(InputAction item1, AxisState item2) : base(item1, item2) {}
    }
}
