using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;

namespace JoystickSimulator.Helpers
{
    /// <summary>
    /// Va gérer la confuguration du simulateur
    /// </summary>
    class ConfigManager
    {
        public List<Point3D> Support { get; set; }
        public List<Point3D> Seat { get; set; }
        public Vector3D Offset { get; set; }
        public Point3D RotationPoint { get; set; }
        public double MuscleMin { get; set; }
        public double MuscleMax { get; set; }
        //public double VoltMin { get; set; }
        //public double VoltMax { get; set; }
        public List<double[]> VoltCurve { get; set; }

        public ConfigManager(List<Point3D> support, List<Point3D> seat, Vector3D offset, Point3D rotationPoint, double muscleMin, double muscleMax, /*double voltMin, double voltMax,*/ List<double[]> voltCurve)
        {
            Support = support;
            Seat = seat;
            Offset = offset;
            RotationPoint = rotationPoint;
            MuscleMin = muscleMin;
            MuscleMax = muscleMax;
            //VoltMin = voltMin;
            //VoltMax = voltMax;
            VoltCurve = voltCurve;
        }

        /// <summary>
        /// Contient une configuration par défaut
        /// </summary>
        public ConfigManager()
        {
            Support = new List<Point3D> {
                new Point3D(51,     0,      3.5),
                new Point3D(0,      82.5,   0),
                new Point3D(0,      93.5,   0),
                new Point3D(113,    93.5,   0),
                new Point3D(113,    82.5,   0),
                new Point3D(62,     0,      3.5)
            };

            Seat = new List<Point3D> {
                new Point3D(0,      0,      3.5),
                new Point3D(0,      9,      0),
                new Point3D(15.5,   89,     0),
                new Point3D(60.25,  89,     0),
                new Point3D(75.5,   9,      0),
                new Point3D(75.5,   0,      3.5)
            };

            Offset = new Vector3D(-18.75, -32.5, 67);
            RotationPoint = new Point3D(37.75, 44.5, 0);
            MuscleMin = 66.8;
            MuscleMax = 80.5;
            //VoltMin = 0.0;
            //VoltMax = 10.0;

            #region Ancienne courbe
            //VoltCurve = new List<double[]> {
            //    new double[] { 0.0, 66.5 },
            //    new double[] { 0.5, 66.5 },
            //    new double[] { 1.0, 66.2 },
            //    new double[] { 1.5, 65.9 },
            //    new double[] { 2.0, 65.5 },
            //    new double[] { 2.5, 64.7 },
            //    new double[] { 3.0, 63.7 },
            //    new double[] { 3.5, 62.7 },
            //    new double[] { 4.0, 61.5 },
            //    new double[] { 4.5, 60.1 },
            //    new double[] { 5.0, 59.0 },
            //    new double[] { 5.5, 57.9 },
            //    new double[] { 6.0, 57.0 },
            //    new double[] { 6.5, 56.2 },
            //    new double[] { 7.0, 55.6 },
            //    new double[] { 7.5, 55.0 },
            //    new double[] { 8.0, 54.4 },
            //    new double[] { 8.5, 53.9 },
            //    new double[] { 9.0, 53.5 },
            //    new double[] { 9.5, 53.1 },
            //    new double[] { 10.0, 52.8 }
            //};
            #endregion

            VoltCurve = new List<double[]> {
                new double[] { 0.0, 80.5 },
                new double[] { 0.5, 80.5 },
                new double[] { 1.0, 80.2 },
                new double[] { 1.5, 79.9 },
                new double[] { 2.0, 79.5 },
                new double[] { 2.5, 78.7 },
                new double[] { 3.0, 77.7 },
                new double[] { 3.5, 76.7 },
                new double[] { 4.0, 75.5 },
                new double[] { 4.5, 74.1 },
                new double[] { 5.0, 73.0 },
                new double[] { 5.5, 71.9 },
                new double[] { 6.0, 71.0 },
                new double[] { 6.5, 70.2 },
                new double[] { 7.0, 69.6 },
                new double[] { 7.5, 69.0 },
                new double[] { 8.0, 68.4 },
                new double[] { 8.5, 67.9 },
                new double[] { 9.0, 67.5 },
                new double[] { 9.5, 67.1 },
                new double[] { 10.0, 66.8 }
            };
        }

        /// <summary>
        /// Charge la configuration depuis une string json
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ConfigManager GetConfigFromJson(String config)
        {
            return JsonConvert.DeserializeObject<ConfigManager>(config);
        }

        /// <summary>
        /// Charge la configuration depuis un fichier json
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ConfigManager LoadConfigfromFile(string filePath)
        {
            StreamReader file = new StreamReader(filePath);
            string config = file.ReadToEnd();
            file.Close();
            return GetConfigFromJson(config);
        }

        /// <summary>
        /// Retourne la configuration en format json
        /// </summary>
        /// <returns></returns>
        public string GetJsonFromConfig()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Sauvegarde la configuration dans un fichier en format json
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveConfigToFile(string filePath)
        {
            StreamWriter file = new StreamWriter(filePath);
            file.WriteLine(JsonConvert.SerializeObject(this));
            file.Close();
        }
    }
}
