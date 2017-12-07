using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace JoystickSimulator.Helpers
{
    /// <summary>
    /// Methodes permettant de faire des manipulations sur des liste de points
    /// </summary>
    public class PointListHelper
    {
        /// <summary>
        /// Prend une liste de points 3D et renvoie une liste de points2D
        /// </summary>
        /// <param name="pl">Liste de points en 3D</param>
        /// <returns>La liste de points en 2 dimensions</returns>
        public static List<Point> ThreeToTwo(List<Point3D> pl) //TODO linq
        {
            List<Point> newList = new List<Point>();
            foreach (Point3D p3d in pl)
            {
                newList.Add(new Point(p3d.X, p3d.Y));
            }
            return newList;
        }
    }
}
