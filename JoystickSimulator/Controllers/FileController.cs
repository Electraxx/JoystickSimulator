using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using JoystickSimulator.Helpers;
using JoystickSimulator.Models;

namespace JoystickSimulator.Controllers
{
    /// <summary>
    /// Va permettre d'accéder au système de fichier, en attendant c'est la foire
    /// </summary>
    class FileController
    {

        public ConfigManager Cm { get; set; }
        private ActionRecorder AR;

        public FileController()
        {
            Cm = new ConfigManager();
            AR = new ActionRecorder();
        }

        public List<Point3D> GetSeatPoints()
        {
            return Cm.Seat;
        }


        public void Record(InputAction action, AxisState state)
        {
            AR.Record(action, state);
        }

        public bool SwitchRecorderState() {
            return AR.SwitchRecorderState();
        }

        /// <summary>
        /// Save le json et retourne l'état du recorder
        /// </summary>
        /// <returns></returns>
        public bool SaveJson(string filename) {
            //Console.WriteLine(AR.GetJson());
            File.WriteAllText(filename, AR.GetJson());
            return AR.IsRecording;
        }
    }
}
