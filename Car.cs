using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using GMap.NET.MapProviders;

namespace _33.Пшы
{
    class Car : MapObject
    {
        private PointLatLng point;
        private Route route;
        private List<Food> orderPoints = new List<Food>();
        private GMapMarker carMarker = null;
        public GMapControl gMap;


        public Car(string title, PointLatLng point) : base(title) { this.point = point; }
        public override double getDistance(PointLatLng point)
        {
            double dist;

            double lat = (double)Math.Abs(point.Lat - this.point.Lat);
            double lng = (double)Math.Abs(point.Lng - this.point.Lng);

            dist = Math.Sqrt(Math.Pow(lat, 2) + Math.Pow(lng, 2));

            return dist;
        }

        public override PointLatLng getFocus()
        {
            return point;
        }

        public override GMapMarker getMarker()
        {
            GMapMarker markCar = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 24, // ширина маркера
                    Height = 24, // высота маркера
                    ToolTip = this.getTitle(), // всплывающая подсказка
                    Margin = new System.Windows.Thickness(-12, -12, 0, 0),
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/backpack.png")) // картинка
                }
            };
            return carMarker = markCar;
        }

        // метод перемещения по маршруту
        public GMapMarker moveTo(PointLatLng pointRoute)
        {
            List<PointLatLng> AllpointRoute = new List<PointLatLng>();

            MapRoute route = BingMapProvider.Instance.GetRoute(this.point, pointRoute, false, false, (int)15);
            if (route != null)
            {
                AllpointRoute = AllpointRoute.Concat(route.Points).ToList();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось найти маршрут");
            }

            this.route = new Route("Route", AllpointRoute);

            Thread newThread = new Thread(new ThreadStart(MoveByRoute));
            newThread.Start();

            return this.route.getMarker();

        }
        // событие прибытия
        public event EventHandler Arrived;

        private void MoveByRoute()
        {
            // последовательный перебор точек маршрута
            foreach (var point in route.getPoints())
            {
                // делегат, возвращающий управление в главный поток
                Application.Current.Dispatcher.Invoke(delegate
                {
                    // изменение позиции маркера
                    carMarker.Position = point;
                    this.point = point;
                    if (orderPoints != null)
                    {
                        foreach (var food in orderPoints)
                        {
                            food.FMarker.Position = point;
                        }
                    }
                });
                // задержка 500 мс
                Thread.Sleep(200);
            }
            // отправка события о прибытии после достижения последней точки маршрута
            Arrived?.Invoke(this, null);
        }

        private void turnMarker(PointLatLng point, PointLatLng nextPoint)
        {
            // вычисление разницы между двумя соседними точками по широте и долготе
            double latDiff = nextPoint.Lat - point.Lat;
            double lngDiff = nextPoint.Lng - point.Lng;
            // вычисление угла направления движения
            // latDiff и lngDiff - катеты прямоугольного треугольника
            double angle = Math.Atan2(lngDiff, latDiff) * 180.0 / Math.PI;
            // установка угла поворота маркера
            getMarker().Shape.RenderTransform = new RotateTransform { Angle = angle };
        }

        public void foodInCar(object sender, EventArgs e)
        {
            Food f = (Food)sender;
            orderPoints.Add(f);
            
            Application.Current.Dispatcher.Invoke(delegate
            {
                gMap.Markers.Add(this.moveTo(f.nextLoc.getFocus()));
            });

        }
    }
}
