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
        private List<Food> orderPoints;

        public Car(string title, PointLatLng point) : base(title) { this.point = point; }
        public override double getDistance(PointLatLng point)
        {
            double dist;

            double lat = (double) Math.Abs(point.Lat - this.point.Lat);
            double lng = (double) Math.Abs(point.Lng - this.point.Lng);

            dist = Math.Sqrt(Math.Pow(lat, 2)+ Math.Pow(lng, 2));

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
                    Width = 32, // ширина маркера
                    Height = 32, // высота маркера
                    ToolTip = this.getTitle(), // всплывающая подсказка
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/car.png")) // картинка
                }
            };
            return markCar;
        }

        // событие прибытия
        public event EventHandler Arrived;
        // метод перемещения по маршруту
        public GMapMarker moveTo(PointLatLng startPoint, PointLatLng endPoint)
        {
            MapRoute route = BingMapProvider.Instance.GetRoute(startPoint, endPoint, false, false, (int)15);
            if (route != null)
            {
                Route r = new Route("Route", route.Points);
                List<PointLatLng> RoutePoints = route.Points;
                this.route = new Route("route", RoutePoints);
                
                return this.route.getMarker();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось найти маршрут");
                return null;
            }

        }
        private void MoveByRoute(Route route)
        {
            PointLatLng prepoint = route.getPoints().First();
            foreach (var point in route.getPoints())
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    getMarker().Position = point;
                    this.turnMarker(prepoint ,point);
                    prepoint = point;
                });
                Thread.Sleep(500);
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

    }
}
