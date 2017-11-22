using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace JoystickSimulator.Helpers
{
    public class PointListHelper
    {
        public static List<Point> ThreeToTwo(List<Point3D> pl)
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
