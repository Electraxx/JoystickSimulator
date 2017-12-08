using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using JoystickSimulator.Annotations;

namespace JoystickSimulator.Models
{
    /// <summary>
    /// Modèle représentant le simulateur
    /// </summary>
    public class Simulator : INotifyPropertyChanged
    {
        /// <summary>
        /// Liste des points du siège
        /// </summary>
        public List<Point3D> Seat { get; set; }

        /// <summary>
        /// Point de rotation du siège
        /// </summary>
        private Point3D rotationPoint;

        public Point3D RotationPoint {
            get { return rotationPoint; }
            set {
                rotationPoint = value;
                OnPropertyChanged(nameof(RotationPoint));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Simulator(List<Point3D> Seat, Point3D RotationPoint) {
            this.RotationPoint = RotationPoint;
            this.Seat = Seat;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
