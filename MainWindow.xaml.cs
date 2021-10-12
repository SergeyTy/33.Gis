using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using System.Windows.Forms;
using System;
using GMap.NET.WindowsPresentation;

namespace _33.Пшы
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        Dictionary<Food, double> foodDict = new Dictionary<Food, double>();
        Dictionary<Food, double> sortDistFoodDict = new Dictionary<Food, double>();
        List<MapObject> listOfAllObj = new List<MapObject>();
        List<Food> listOfAllFood = new List<Food>();
        int setTool; // 0 - arrow; 1 - car; 2 - human; 3 - food;
        string searchName = "";
        PointLatLng deliveryAddress = new PointLatLng();
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
            BingMapProvider.Instance.ClientKey = "aXAGSqm169O6Mc7UzDAE~mRwh4DxJnpIEWrHJtxwS-w~AgHeUYXq2mVyEdtUchGC1zmOqd1Ndc2w14IpdPRn86dXSi2_FRh04Lb8bVLE8wMg";

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

            Food food1 = new Food("сырок", new PointLatLng(55.012823, 82.950359));
            Food food2 = new Food("Мясо", new PointLatLng(55.011823, 82.960359));
            Food food3 = new Food("Молоко", new PointLatLng(55.011, 82.952));
            Food food4 = new Food("Арбуз", new PointLatLng(55.010, 82.94));
            Food food5 = new Food("Яйцо", new PointLatLng(55.013, 82.962));
            Map.Markers.Add(food1.getMarker());
            Map.Markers.Add(food2.getMarker());
            Map.Markers.Add(food3.getMarker());
            Map.Markers.Add(food4.getMarker());
            Map.Markers.Add(food5.getMarker());

            listOfAllFood.Add(food1);
            listOfAllFood.Add(food2);
            listOfAllFood.Add(food3);
            listOfAllFood.Add(food4);
            listOfAllFood.Add(food5);




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
            PointLatLng firstFood = searchNearestFood(listOfAllObj.FindLast(FindCar).getFocus());
            List<PointLatLng> list = FindRoute(firstFood, listOfAllObj.FindLast(FindCar).getFocus());
            Route r = new Route("Route", list);
            foreach (Food item in sortDistFoodDict.Keys)
            {
                r.addPointToPoints(item.getFocus());

            }
                r.addPointToPoints(deliveryAddress);


            Map.Markers.Add(r.getMarker());
            listOfAllObj.Add(r);
        }

        private PointLatLng searchNearestFood(PointLatLng point)
        {
            foreach (Food obj in listOfAllFood)
            {
                foodDict.Add(obj, obj.getDistance(point));
            }
            sortDistFoodDict = foodDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return sortDistFoodDict.Keys.ElementAt(0).getFocus();
        }

        private List<PointLatLng> FindRoute(PointLatLng startPoint, PointLatLng endPoint) {
            // Получить путь
            //MapRoute route = BingMapProvider.Instance.GetRoute(startPoint, endPoint, false, false, (int)this.Map.Zoom);
            //if (route != null)
            //{
            List<PointLatLng> list = new List<PointLatLng>();
            list.Add(startPoint);
            list.Add(endPoint);
            return list;
                
            //}
            //else
            //{
            //    System.Windows.Forms.MessageBox.Show("Не удалось найти маршрут");
            //}
        }

        private void btn_deliver_car_create_Click(object sender, RoutedEventArgs e)
        {
            setTool = 1;
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            setTool = 0;
            Map.Markers.Clear();
            listOfAllFood.Clear();
            listOfAllObj.Clear();
            foodDict.Clear();
        }
    }
}
