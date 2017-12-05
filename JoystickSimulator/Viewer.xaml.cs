using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using JoystickSimulator.Helpers;
using JoystickSimulator.Models;
using JoystickSimulator.Packets;
using SharpDX.DirectInput;
using TextAlignment = System.Windows.TextAlignment;

namespace JoystickSimulator
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : UserControl
    {

        public List<Point> SeatPoints { get; private set; }

        public List<TextBlock> muscleLabels { get; set; }

        private Rectangle neutralCursor;
        private Rectangle movementCursor;
        private Rectangle rotationPointCursor;

        private Rectangle armedFilter;

        private Polygon seatRepresentation;

        private double zoomFactor;

        private InputAction lastAction;

        private bool enabled;

        public EventHandler SliderValueChanged { get; set; }
        public EventHandler FileButtonClicked { get; set; }

        public Viewer()
        {
            InitializeComponent();
            zoomFactor = 4;
            muscleLabels = new List<TextBlock>();
            SeatPoints = null;
            lastAction = null;
            enabled = false;
            //SliderValueChanged = new EventHandler(SliderValueChanged);
        }

        public void Do(InputAction action, AxisState axisState)
        {
            switch (action.Name)
            {
                case "SwitchSimulatorState":
                    if (lastAction.Name != "SwitchSimulatorState")
                        SwitchSimulatorState();
                    break;
                case "MoveSimulator":
                    MoveRect(movementCursor, axisState.X, axisState.Y);
                    break;
                case "MoveRotationPoint":
                    MoveRect(rotationPointCursor, axisState.X, axisState.Y);
                    break;
                case "MoveNeutralPoint":
                    MoveRect(neutralCursor, axisState.X, axisState.Y);
                    break;
            }

            lastAction = action;
        }

        private void SwitchSimulatorState()
        {
            armedFilter.Opacity = armedFilter.Opacity > 0 ? 0 : 1;
            enabled = !enabled;
        }

        private void Canvas_Initialized(object sender, EventArgs e)
        {
            seatRepresentation = new Polygon
            {
                Stroke = Brushes.Pink,
                StrokeThickness = 0.6
            };

            movementCursor = new Rectangle
            {
                StrokeThickness = 2,
                Stroke = Brushes.Yellow,
                Fill = Brushes.Yellow,
                Width = 4,
                Height = 4
            };

            rotationPointCursor = new Rectangle
            {
                StrokeThickness = 2,
                Stroke = Brushes.Purple,
                Fill = Brushes.Purple,
                Width = 4,
                Height = 4
            };

            neutralCursor = new Rectangle
            {
                StrokeThickness = 2,
                Stroke = Brushes.Black,
                Fill = Brushes.Black,
                Width = 4,
                Height = 4
            };
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            InitCanvas();
        }

        private void InitCanvas()
        {
            DrawPanel.Children.Clear();
            muscleLabels.Clear();

            Canvas.SetLeft(neutralCursor, (DrawPanel.ActualWidth / 2) - neutralCursor.ActualWidth / 2);
            Canvas.SetTop(neutralCursor, (DrawPanel.ActualHeight / 2) - neutralCursor.ActualHeight / 2);

            Canvas.SetLeft(rotationPointCursor, (DrawPanel.ActualWidth / 2) - rotationPointCursor.ActualWidth / 2);
            Canvas.SetTop(rotationPointCursor, (DrawPanel.ActualHeight / 2) - rotationPointCursor.ActualHeight / 2);

            Canvas.SetLeft(movementCursor, (DrawPanel.ActualWidth / 2) - movementCursor.ActualWidth / 2);
            Canvas.SetTop(movementCursor, (DrawPanel.ActualHeight / 2) - movementCursor.ActualHeight / 2);

            DrawPanel.Children.Add(neutralCursor);
            DrawPanel.Children.Add(rotationPointCursor);
            DrawPanel.Children.Add(movementCursor);

            //Liste de point3d en liste de point2d
            //Point3D p = new Point3D();
            //List<double> motionSize = motionCalc.GetMuscleSize(motionCalc.Seat);
            List<double> motionSize = new double[] { 0, 0, 0, 0, 0, 0 }.OfType<double>().ToList(); //temporary
            for (int i = 0; i < SeatPoints.Count; i++)
            {
                muscleLabels.Add(new TextBlock());
                muscleLabels[i].Text = "Verrin " + (i + 1) + "\n " + $"{motionSize[i]:0.00}";
            }

            //Ajout de la liste de point au polygone afin de pouvoir trouver sa taille
            seatRepresentation.Points = new PointCollection(SeatPoints);

            //Transform group
            TransformGroup tg = new TransformGroup();

            //Zoom
            ScaleTransform zoom = new ScaleTransform(zoomFactor, zoomFactor);
            tg.Children.Add(zoom);

            //Centrage
            TranslateTransform uiOffset = new TranslateTransform(
                DrawPanel.ActualWidth / 2 - seatRepresentation.ActualWidth / 2 * zoomFactor,
                DrawPanel.ActualHeight / 2 - seatRepresentation.ActualHeight / 2 * zoomFactor);
            tg.Children.Add(uiOffset);

            //Rotation de 180°
            RotateTransform rotation = new RotateTransform(180, DrawPanel.ActualWidth / 2, DrawPanel.ActualHeight / 2);
            tg.Children.Add(rotation);

            //Application des transform
            seatRepresentation.RenderTransform = tg;
            DrawPanel.Children.Add(seatRepresentation);

            List<Point> transformedPoints = seatRepresentation.Points.Select(p => tg.Transform(p)).ToList(); //Black magic from StackOverflow || Applique le TransformGroup sur tout les points, on peut donc trouver les points post-tranform
            transformedPoints.Reverse(0, transformedPoints.Count);

            for (int i = 0; i < muscleLabels.Count / 2; i++) //On déplace les textblocks et on les affiche //Backup
            {
                //TODO: Dans le futur, calculer dynamiquement la largeur d'un textbox
                muscleLabels[i].RenderTransform =
                    new TranslateTransform { X = transformedPoints[i].X + 5, Y = transformedPoints[i].Y };
                DrawPanel.Children.Add(muscleLabels[i]);
            }
            for (int i = muscleLabels.Count / 2; i < muscleLabels.Count; i++) //Fait en 2 fois pour éviter de faire un if
            {
                muscleLabels[i].RenderTransform =
                    new TranslateTransform { X = transformedPoints[i].X - 45, Y = transformedPoints[i].Y };
                DrawPanel.Children.Add(muscleLabels[i]);
            }

            //Le filtre à la fin pour avoir la taille du drawPanel
            armedFilter = new Rectangle
            {
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)),
                Fill = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)),
                Width = DrawPanel.ActualWidth,
                Height = DrawPanel.ActualHeight
            };

            foreach (var muscleLabel in muscleLabels) //Linq possible, je pense
                muscleLabel.TextAlignment = TextAlignment.Center;

            DrawPanel.Children.Add(armedFilter);
        }

        /// <summary>
        /// Permet de set les points représentant le siège
        /// </summary>
        /// <param name="points">Points du siège</param>
        public void SetSeatPoint(List<Point3D> points)
        {
            SetSeatPoint(PointListHelper.ThreeToTwo(points));
        }

        /// <summary>
        /// Permet de set les points représentant le siège
        /// </summary>
        /// <param name="points">Points du siège</param>
        public void SetSeatPoint(List<Point> points)
        {
            SeatPoints = points;
        }

        private void MoveRect(Rectangle rect, int xValue, int yValue)
        {
            Canvas.SetLeft(rect, (xValue / (65535 / DrawPanel.ActualHeight)) * (enabled ? 1 : 0));
            Canvas.SetTop(rect, (yValue / (65535 / DrawPanel.ActualHeight)) * (enabled ? 1 : 0));
        }

        public void UpdateTextblocks(List<double> labelList)
        {

            var txtBlocksAndValues = labelList.Zip(muscleLabels, (v, t) => new { Value = v, TxtBlock = t });
            foreach (var item in txtBlocksAndValues.Select((pair, i) => new { i, pair }))
                item.pair.TxtBlock.Text = $" Verrin {item.i}\n{item.pair.Value:0.00}";
        }

        private void sensibilitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderValueChanged?.Invoke(sender, new SliderChangedEventArgs(((Slider)sender).Value));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileButtonClicked(sender, new FileButtonClickedEventArgs());
        }
    }
}