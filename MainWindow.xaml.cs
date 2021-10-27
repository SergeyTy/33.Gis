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


        Dictionary<Food, double> orderWearhouse = new Dictionary<Food, double>();
        Dictionary<Car, double> car = new Dictionary<Car, double>();
        Dictionary<Food, double> sortOrderWearhose = new Dictionary<Food, double>();
        Dictionary<Car, double> sortCar = new Dictionary<Car, double>();
        List<MapObject> listOfAllObj = new List<MapObject>();
        int setTool; // 0 - arrow; 1 - car; 2 - human; 3 - food;
        string searchName = "";
        Human deliveryAddress = null;
        private Car deliveryCar;
        List<Food> orderFood = new List<Food>();


        private Food w_curd = new Food("сырок", new PointLatLng(55.012823, 82.950359), 0);
        private Food _curd = new Food("сырок", new PointLatLng(55.012823, 82.950359), 0);
        private Food w_meat = new Food("Мясо", new PointLatLng(55.011823, 82.960359), 3);
        private Food _meat = new Food("Мясо", new PointLatLng(55.011823, 82.960359), 3);
        private Food w_milk = new Food("Молоко", new PointLatLng(55.011, 82.952), 2);
        private Food _milk = new Food("Молоко", new PointLatLng(55.011, 82.952), 2);
        private Food w_watermelon = new Food("Арбуз", new PointLatLng(55.010, 82.94), 1);
        private Food _watermelon = new Food("Арбуз", new PointLatLng(55.010, 82.94), 1);
        private Food w_egg = new Food("Яйцо", new PointLatLng(55.013, 82.962), 4);
        private Food _egg = new Food("Яйцо", new PointLatLng(55.013, 82.962), 4);
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
                    car.gMap = Map;

                    Map.Markers.Add(car.getMarker());
                    listOfAllObj.Add(car);

                    break;
                case 2:
                    int lastHu = listOfAllObj.FindAll(FindHu).Count();
                    Human human = new Human($"Human {lastHu + 1}", point);
                    deliveryAddress = human;
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

            BingMapProvider.Instance.ClientKey = "ApJ8ub6gtG0eZWE3FRDTudqLUizCZbLIEzJPZHYPc1pzxOT3u7TmxUFfMWOkZ8M5";
            // установка провайдера карт
            Map.MapProvider = BingMapProvider.Instance;

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

            Map.Markers.Add(w_curd.getMarker());
            Map.Markers.Add(_curd.getMarker());
            Map.Markers.Add(w_meat.getMarker());
            Map.Markers.Add(_meat.getMarker());
            Map.Markers.Add(w_milk.getMarker());
            Map.Markers.Add(_milk.getMarker());
            Map.Markers.Add(w_watermelon.getMarker());
            Map.Markers.Add(_watermelon.getMarker());
            Map.Markers.Add(w_egg.getMarker());
            Map.Markers.Add(_egg.getMarker());

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
            orderFood = new List<Food>();
            List<MapObject> warehouse = new List<MapObject>();
            //создание списка пунктов складов
            //pointsRoute.Add(deliveryAddress);
            if ((bool)chb_curd.IsChecked) { warehouse.Add(w_curd); }
            if ((bool)chb_egg.IsChecked) { warehouse.Add(w_egg); }
            if ((bool)chb_milk.IsChecked) { warehouse.Add(w_milk); }
            if ((bool)chb_meat.IsChecked) { warehouse.Add(w_meat); }
            if ((bool)chb_watermelon.IsChecked) { warehouse.Add(w_watermelon); }
            //сортировка списка по расстоянию:
            ////поиск ближайшей точки к адресу доставки из всех точек складов
            ////поиск ближайшей точки к последнему в списке пути из всех точек складов
            while (warehouse.Count > 0)
            {
                orderFood.Add(searchNearestPoint(deliveryAddress.getFocus(), warehouse));
            }
            ////поиск ближайшей машины к последнему в спискепути из всех машин
            deliveryCar = searchNearestCar(orderFood.Last().getFocus(), listOfAllObj.FindAll(FindCar));
            //переворот списка
            orderFood.Reverse();

            log.Text += "Список мест:\n";
            for (int i = 0; i < orderFood.Count; i++)
            {
                log.Text += " " + i + ". " + orderFood[i].getTitle() + "\n";
            }

            deliveryCar.Arrived += orderFood[0].CarArrived;
            orderFood[0].foodInCar += deliveryCar.foodInCar;

            if (orderFood.Count > 1)
            {
                for (int i = 0; i < orderFood.Count - 1; i++)
                {
                    orderFood[i].foodInCar += deliveryCar.foodInCar;
                    orderFood[i].nextLoc = orderFood[i + 1];
                    log.Text += orderFood[i].getTitle() + " → " + orderFood[i + 1].getTitle() + "\n";
                }
            }

            orderFood.Last().foodInCar += deliveryCar.foodInCar;
            orderFood[orderFood.Count - 1].nextLoc = deliveryAddress;
            log.Text += orderFood[orderFood.Count - 1].getTitle() + " → " + deliveryAddress.getTitle() + "\n";

            Map.Markers.Add(deliveryCar.moveTo(orderFood[0].getFocus()));

            //1) создание Route по этим точкам
            //2) создание Route по этим точкам через MapRoute
            //Map.Markers.Add(deliveryCar.moveTo(pointsRoute));
        }

        private Food searchNearestPoint(PointLatLng point, List<MapObject> list)
        {
            foreach (Food obj in list)
            {
                orderWearhouse.Add(obj, obj.getDistance(point));
            }
            sortOrderWearhose = orderWearhouse.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            list.Remove(sortOrderWearhose.Keys.First());
            orderWearhouse.Clear();
            return sortOrderWearhose.Keys.ElementAt(0);
        }
        private Car searchNearestCar(PointLatLng point, List<MapObject> list)
        {
            foreach (Car obj in list)
            {
                car.Add(obj, obj.getDistance(point));
            }
            sortCar = car.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            car.Clear();
            return sortCar.Keys.ElementAt(0);
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
            orderWearhouse.Clear();
            orderFood.Clear();
            car.Clear();
            sortOrderWearhose.Clear();
            sortCar.Clear();
            deliveryCar = null;
            deliveryAddress = null;


            w_curd = new Food("сырок", new PointLatLng(55.012823, 82.950359), 0);
            _curd = new Food("сырок", new PointLatLng(55.012823, 82.950359), 0);
            w_meat = new Food("Мясо", new PointLatLng(55.011823, 82.960359), 3);
            _meat = new Food("Мясо", new PointLatLng(55.011823, 82.960359), 3);
            w_milk = new Food("Молоко", new PointLatLng(55.011, 82.952), 2);
            _milk = new Food("Молоко", new PointLatLng(55.011, 82.952), 2);
            w_watermelon = new Food("Арбуз", new PointLatLng(55.010, 82.94), 1);
            _watermelon = new Food("Арбуз", new PointLatLng(55.010, 82.94), 1);
            w_egg = new Food("Яйцо", new PointLatLng(55.013, 82.962), 4);
            _egg = new Food("Яйцо", new PointLatLng(55.013, 82.962), 4);

            Map.Markers.Add(w_curd.getMarker());
            Map.Markers.Add(_curd.getMarker());
            Map.Markers.Add(w_meat.getMarker());
            Map.Markers.Add(_meat.getMarker());
            Map.Markers.Add(w_milk.getMarker());
            Map.Markers.Add(_milk.getMarker());
            Map.Markers.Add(w_watermelon.getMarker());
            Map.Markers.Add(_watermelon.getMarker());
            Map.Markers.Add(w_egg.getMarker());
            Map.Markers.Add(_egg.getMarker());
        }

    }
}