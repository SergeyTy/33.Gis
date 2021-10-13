using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using System.Windows.Forms;
using System;
using GMap.NET.WindowsPresentation;
using System.Threading;

namespace _33.Пшы
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        Dictionary<MapObject, double> pointRouteDict = new Dictionary<MapObject, double>();
        Dictionary<MapObject, double> sortPointRouteDict = new Dictionary<MapObject, double>();
        List<MapObject> listOfAllObj = new List<MapObject>();
        int setTool; // 0 - arrow; 1 - car; 2 - human; 3 - food;
        string searchName = "";
        PointLatLng deliveryAddress = new PointLatLng();

        Food warehouse_curd = new Food("сырок", new PointLatLng(55.012823, 82.950359), 0);
        Food warehouse_meat = new Food("Мясо", new PointLatLng(55.011823, 82.960359), 3);
        Food warehouse_milk = new Food("Молоко", new PointLatLng(55.011, 82.952), 2);
        Food warehouse_watermelon = new Food("Арбуз", new PointLatLng(55.010, 82.94), 1);
        Food warehouse_egg = new Food("Яйцо", new PointLatLng(55.013, 82.962), 4);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            string log = "x: " + (int)e.GetPosition(Map).X + "; y:" + (int)e.GetPosition(Map).Y;

            switch (setTool)
            {
                case 0:
                    break;
                case 1:
                    int lastCar = listOfAllObj.FindAll(FindCar).Count();
                    Car car = new Car($"Car {lastCar + 1}", point);
                    Map.Markers.Add(car.getMarker());
                    listOfAllObj.Add(car);

                    break;
                case 2:
                    deliveryAddress = point;
                    int lastHu = listOfAllObj.FindAll(FindHu).Count();
                    Human human = new Human($"Human {lastHu + 1}", point);
                    Map.Markers.Add(human.getMarker());
                    listOfAllObj.Add(human);
                    break;
                case 3:

                    break;
                default:
                    break;
            }
        }

        private bool FindRo(MapObject obj)
        {
            return obj.getTitle().StartsWith("R");
        }
        private bool FindArea(MapObject obj)
        {
            return obj.getTitle().StartsWith("A");
        }

        private bool FindHu(MapObject obj)
        {
            return obj.getTitle().StartsWith("H");
        }

        private bool FindCar(MapObject obj)
        {
            return obj.getTitle().StartsWith("C");
        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            // настройка доступа к данным
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            // установка провайдера карт
            Map.MapProvider = BingMapProvider.Instance;
            BingMapProvider.Instance.ClientKey = "sw8a7MkIoE2MepwcaRuW~CPVpB235mqNxC1wnSOowow~AmoJWWGQsB_RN31sQRcokJbF4w3ZRyPnxi56aoJmhDHm1vTpfj18RgyvbTLVkegK";

            // установка зума карты    
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;
            // установка фокуса карты
            Map.Position = new PointLatLng(55.012823, 82.950359);
            // настройка взаимодействия с картой
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Middle;

            Map.Markers.Add(warehouse_curd.getMarker());
            Map.Markers.Add(warehouse_meat.getMarker());
            Map.Markers.Add(warehouse_milk.getMarker());
            Map.Markers.Add(warehouse_watermelon.getMarker());
            Map.Markers.Add(warehouse_egg.getMarker());

        }


        private bool FindByName(MapObject obj)
        {
            if (obj.getTitle().StartsWith(searchName))
            {
                return true;
            }
            return false;
        }



        private void btn_pointB_Click(object sender, RoutedEventArgs e)
        {
            setTool = 2;
        }

        private void btn_go_Click(object sender, RoutedEventArgs e)
        {
            List<PointLatLng> pointsRoute = new List<PointLatLng>();
            List<MapObject> warehouse = new List<MapObject>();
            //создание списка пунктов складов
            pointsRoute.Add(deliveryAddress);
            if ((bool)chb_curd.IsChecked) { warehouse.Add(warehouse_curd); }
            if ((bool)chb_egg.IsChecked) { warehouse.Add(warehouse_egg); }
            if ((bool)chb_milk.IsChecked) { warehouse.Add(warehouse_milk); }
            if ((bool)chb_meat.IsChecked) { warehouse.Add(warehouse_meat); }
            if ((bool)chb_watermelon.IsChecked) { warehouse.Add(warehouse_watermelon); }
            //сортировка списка по расстоянию:
            ////поиск ближайшей точки к адресу доставки из всех точек складов
            ////поиск ближайшей точки к последнему в списке пути из всех точек складов
            while (warehouse.Count > 0)
            {
                pointsRoute.Add(searchNearestPoint(pointsRoute.Last(), warehouse));
            }
            ////поиск ближайшей машины к последнему в спискепути из всех машин
            Car deliveryCar = (Car)searchNearestCar(pointsRoute.Last(), listOfAllObj.FindAll(FindCar));
            pointsRoute.Add(deliveryCar.getFocus());
            pointsRoute.Reverse();
            //1) создание Route по этим точкам
            Route route = new Route("Route", pointsRoute);
            Map.Markers.Add(route.getMarker());
            deliveryCar.moveTo(route.getPoints().Last());
            
            //2) создание Route по этим точкам через MapRoute



        }

        private PointLatLng searchNearestPoint(PointLatLng point, List<MapObject> list)
        {
            foreach (MapObject obj in list)
            {
                pointRouteDict.Add(obj, obj.getDistance(point));
            }
            sortPointRouteDict = pointRouteDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            list.Remove(sortPointRouteDict.Keys.First());
            pointRouteDict.Clear();
            return sortPointRouteDict.Keys.ElementAt(0).getFocus();
        }
        private MapObject searchNearestCar(PointLatLng point, List<MapObject> list)
        {
            foreach (MapObject obj in list)
            {
                pointRouteDict.Add(obj, obj.getDistance(point));
            }
            sortPointRouteDict = pointRouteDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            pointRouteDict.Clear();
            return sortPointRouteDict.Keys.ElementAt(0);
        }

        private void FindRoute(PointLatLng startPoint, PointLatLng endPoint)
        {
            MapRoute route = BingMapProvider.Instance.GetRoute(startPoint, endPoint, false, false, (int)this.Map.Zoom);
            if (route != null)
            {
                Route r = new Route("Route", route.Points);
                Map.Markers.Add(r.getMarker());
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не удалось найти маршрут");
            }
        }

        private void btn_deliver_car_create_Click(object sender, RoutedEventArgs e)
        {
            setTool = 1;
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            setTool = 0;
            Map.Markers.Clear();
            listOfAllObj.Clear();
            pointRouteDict.Clear();

            Map.Markers.Add(warehouse_curd.getMarker());
            Map.Markers.Add(warehouse_meat.getMarker());
            Map.Markers.Add(warehouse_milk.getMarker());
            Map.Markers.Add(warehouse_watermelon.getMarker());
            Map.Markers.Add(warehouse_egg.getMarker());
        }

        private void btn_test_Click(object sender, RoutedEventArgs e)
        {
            FindRoute(warehouse_curd.getFocus(),warehouse_meat.getFocus());
        }
    }
}