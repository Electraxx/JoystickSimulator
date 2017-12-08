using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace JoystickSimulator.Models
{
    public class MotionCalculation
    {

        public List<Point3D> Support { get; private set; }
        public List<Point3D> Seat { get; private set; }
        public Vector3D Offset { get; private set; }
        public Point3D RotationPoint { get; private set; }
        public double MuscleMin { get; private set; }
        public double MuscleMax { get; private set; }
        public List<double[]> VoltCurve { get; private set; }

        public MotionCalculation(List<Point3D> support, List<Point3D> seat, Vector3D offset, Point3D rotationPoint, double muscleMin, double muscleMax, List<double[]> voltCurve)
        {
            Support = support;
            Seat = seat;
            Offset = offset;
            RotationPoint = rotationPoint;
            MuscleMin = muscleMin;
            MuscleMax = muscleMax;
            VoltCurve = voltCurve;
        }

        /// <summary>
        /// Retourne la position d'une forme après une transformation
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="roll"></param>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <param name="rotationPoint"></param>
        /// <param name="translation"></param>
        /// <returns></returns>
        public static List<Point3D> Transform(List<Point3D> shape, double roll, double pitch, double yaw, Point3D rotationPoint, Vector3D translation)
        {
            Matrix3D transform = MathMatrix.Calculation.GetModificationMatrix(yaw, pitch, roll, rotationPoint, translation);
            List<Point3D> transformed = new List<Point3D>();

            foreach (Point3D point in shape)
            {
                transformed.Add(Point3D.Multiply(point, transform));
            }
            return transformed;
        }

        public static List<Point3D> Transform(List<Point3D> shape, Instruction instructions)
        {
            return Transform(shape, instructions.Roll, instructions.Pitch, instructions.Yaw, instructions.RotationPoint, instructions.Translation);
        }

        /// <summary>
        /// Retourne la distance entre deux Point3D
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static double GetDistance(Point3D A, Point3D B)
        {
            Vector3D delta = Point3D.Subtract(A, B); // Récupère le delta entre 2 points
            double distance = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2) + Math.Pow(delta.Z, 2)); // Trouve la distance avec le delta
            return distance;
        }

        /// <summary>
        /// Retourne la taille des vérins
        /// </summary>
        /// <returns></returns>
        public List<double> GetMuscleSize(List<Point3D> seatPoints)
        {
            Matrix3D transform = MathMatrix.Calculation.GetModificationMatrix(0, 0, 0, new Point3D(), Offset); // Créer une matrice de transformation pour appliquer l'offset sur le support

            List<Point3D> transformedSupport = new List<Point3D>();
            foreach (Point3D s in Support)
            {
                transformedSupport.Add(Point3D.Multiply(s, transform));
            }


            List<double> muscles = new List<double>();
            var points = transformedSupport.Zip(seatPoints, (su, se) => new { Support = su, Seat = se });

            foreach (var p in points)
            {
                muscles.Add(GetDistance(p.Support, p.Seat));
            }

            return muscles;
        }

        /// <summary>
        /// Applique un sampling aux paramètres de la transformation du siège
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="delta"></param>
        /// <param name="sampling"></param>
        /// <returns></returns>
        public Instruction Sampler(Instruction instructions, List<double> delta, double sampling)
        {
            //Instructions newIn = new Instructions(instructions.AddSample(delta.Select(r => r * sampling).ToList()));
            //return Transform(Seat, newIn);
            return new Instruction(instructions.AddSample(delta.Select(r => r * sampling).ToList()));
        }

        /// <summary>
        /// Retourne la position du siège en prenant en compte la taille des vérins
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="instructions"></param>
        /// <param name="previousInstructions"></param>
        /// <returns></returns>
        public Instruction Oversampling(int samples, Instruction instructions, Instruction previousInstructions)
        {

            double ratio = 0.5;

            List<double> muscles = GetMuscleSize(Transform(Seat, instructions.Yaw, instructions.Pitch, instructions.Roll, instructions.RotationPoint, instructions.Translation));
            List<double> delta = new List<double>(instructions.GetPropertiesAsList().Zip(previousInstructions.GetPropertiesAsList(), (d1, d2) => d1 - d2).ToArray());

            double sampling = 0;


            if (muscles.Any(x => x > MuscleMax || x < MuscleMin))
            {
                double tmpSampling = 0;

                for (int i = 1; i < samples; i++)
                {
                    tmpSampling = sampling + Math.Pow(ratio, i);
                    List<double> tmpMuscles = GetMuscleSize(Transform(Seat, Sampler(previousInstructions, delta, tmpSampling)));

                    if (!tmpMuscles.Any(x => x > MuscleMax || x < MuscleMin))
                    {
                        sampling = tmpSampling;
                    }

                    //Console.WriteLine("Sampling: " + sampling);

                }
                return Sampler(previousInstructions, delta, sampling);
            }
            return instructions;
        }

        /// <summary>
        /// Fonction d'interpolation linéaire
        /// </summary>
        /// <param name="X"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double LinearInterpolation(double x, List<double[]> values, bool invert = false)
        {
            int a = invert ? 1 : 0;
            int b = invert ? 0 : 1;

            double result = -1;
            for (int i = 1; i < values.Count; i++)
            {
                if (x <= values[i - 1][a] && x >= values[i][a])
                {
                    result = values[i - 1][b] + ((x - values[i - 1][a]) * (values[i][b] - values[i - 1][b]) / (values[i][a] - values[i - 1][a])); // Interpolation
                }
            }

            if (x > values[0][a])
            {
                result = values[0][b];
            }
            else if (x < values[values.Count - 1][a])
            {
                result = values[values.Count - 1][b];
            }
            return result;
        }

        /// <summary>
        /// Converti les distances en Volts
        /// </summary>
        /// <param name="x"></param>
        /// <param name="minDist"></param>
        /// <param name="maxDist"></param>
        /// <param name="minVolt"></param>
        /// <param name="maxVolt"></param>
        /// <returns></returns>
        public List<double> DistsToVolts(List<double> distances)
        {
            List<double> result = new List<double>();
            foreach (double d in distances)
            {
                result.Add(LinearInterpolation(d, VoltCurve, true));
            }
            return result;
        }

    }
}
