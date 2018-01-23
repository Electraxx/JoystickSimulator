using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
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
            AR.Record(action, (AxisState)state.Clone());
        }

        public bool SwitchRecorderState()
        {
            return AR.SwitchRecorderState();
        }

        /// <summary>
        /// retourne le json et retourne l'état du recorder
        /// </summary>
        /// <returns></returns>
        public bool GetJson(string filename)
        {
            //Console.WriteLine(AR.GetJson());
            File.WriteAllText(filename, AR.GetJson());
            return AR.IsRecording;
        }

        /// <summary>
        /// Détérmine si il est possible de sauvegarder le fichier
        /// </summary>
        /// <returns></returns>
        public bool IsAbleToSave()
        {
            return (AR.ActionList.Count > 0 && !AR.IsRecording);
        }

        public bool GetRecorderState()
        {
            return AR.IsRecording;
        }

        /// <summary>
        /// Permet d'obtenir le coutenu d'un fichier à partir de son chemin
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetContent(string path)
        {
            return File.ReadAllText(path);
        }

        public void OpenHelp()
        {
            if (!File.Exists("files/help.pdf"))
                ExtractHelp();

            ShowPdf("files/help.pdf");
        }

        private void ShowPdf(string helpPdf) {
            Process.Start(AssemblyDirectory()+"/files/help.pdf");
        }

        private void ExtractHelp()
        {
            byte[] pdf = Properties.Resources.Aide_JoystickSimulator;
            MemoryStream ms = new MemoryStream(pdf);

            //Create PDF File From Binary of resources folders help.pdf
            Directory.CreateDirectory("files");
            FileStream f = new FileStream("files/help.pdf", FileMode.CreateNew);

            //Writes Bytes into Our pdf

            ms.WriteTo(f);
            f.Close();
            ms.Close();
        }

        private string AssemblyDirectory() {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
