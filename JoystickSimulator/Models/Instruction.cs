using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace JoystickSimulator.Models
{
    /// <summary>
    /// Représente une série d'instruction pour les calculs du simulateur
    /// </summary>
    public class Instruction
    {
        public double Roll { set; get; }
        public double Pitch { set; get; }
        public double Yaw { set; get; }
        public Point3D RotationPoint { set; get; }
        public Vector3D Translation { set; get; }


        public Instruction(double roll, double pitch, double yaw, Point3D rotationPoint, Vector3D translation)
        {
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
            RotationPoint = rotationPoint;
            Translation = translation;
        }

        public Instruction(List<double> infos)
        {
            Roll = infos[0];
            Pitch = infos[1];
            Yaw = infos[2];

            RotationPoint = new Point3D(infos[3], infos[4], infos[5]);
            Translation = new Vector3D(infos[6], infos[7], infos[8]);
        }

        public Instruction(double roll, double pitch, double yaw, double rotationPointX, double rotationPointY, double rotationPointZ, double translationX, double translationY, double translationZ)
        {
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
            RotationPoint = new Point3D(rotationPointX, rotationPointY, rotationPointZ);
            Translation = new Vector3D(translationX, translationY, translationZ);
        }

        /// <summary>
        /// Renvoie les variable de la classe dans une liste
        /// </summary>
        /// <returns>Une liste représantant l'objet</returns>
        public List<double> GetPropertiesAsList()
        {
            return new List<double> { Roll, Pitch, Yaw, RotationPoint.X, RotationPoint.Y, RotationPoint.Z, Translation.X, Translation.Y, Translation.Z };
        }

        /// <summary>
        /// Ajoute deux liste de valeurs ensemble.
        /// Est utilisé dans l'oversampling.
        /// </summary>
        /// <param name="sample">Liste de valeur à ajouter</param>
        /// <returns>L'addition des deux listes</returns>
        public List<double> AddSample(List<double> sample)
        {
            return GetPropertiesAsList().Zip(sample, (x, y) => x + y).ToList();
        }

    }
}
