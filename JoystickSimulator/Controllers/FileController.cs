using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using JoystickSimulator.Helpers;

namespace JoystickSimulator.Controllers
{
    /// <summary>
    /// Va permettre d'accéder proprement au système de fichier, en attendant c'est la foire
    /// </summary>
    class FileController {

        public ConfigManager Cm { get; set; }

        public FileController() {
            Cm = new ConfigManager();
        }

        public List<Point3D> GetSeatPoints() {
            return Cm.Seat;
        }
    }
}
