using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace _33.Пшы
{
    class Food : MapObject
    {
        PointLatLng point;
        PointLatLng destination;
        int type = 0; //0 -  curd; 1 - watermelon; 2 - milk; 3 - meat; 4 - egg;
        public GMapMarker FMarker = null;
        public GMapMarker WFMarker = null;
        public MapObject nextLoc = null;

        public Food(string title, PointLatLng point, int type) : base(title)
        {
            this.point = point;
            this.type = type;
        }

        public PointLatLng getDestination()
        {
            return destination;
        }
        public void moveTo(PointLatLng p)
        {
            this.destination = p;
        }

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
            FMarker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 32, // ширина маркера
                    Height = 32, // высота маркера
                    ToolTip = this.getTitle(), // всплывающая подсказка
                    Margin = new System.Windows.Thickness(-12, -12, 0, 0),
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/curd.png")) // картинка
                }
            };
            switch (type)
            {
                case 0:
                    FMarker = new GMapMarker(point)
                    {
                        Shape = new Image
                        {
                            Width = 32, // ширина маркера
                            Height = 32, // высота маркера
                            ToolTip = this.getTitle(), // всплывающая подсказка
                            Margin = new System.Windows.Thickness(-12, -12, 0, 0),
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/curd.png")) // картинка
                        }
                    };
                    break;
                case 1:
                    FMarker = new GMapMarker(point)
                    {
                        Shape = new Image
                        {
                            Width = 32, // ширина маркера
                            Height = 32, // высота маркера
                            ToolTip = this.getTitle(), // всплывающая подсказка
                            Margin = new System.Windows.Thickness(-12, -12, 0, 0),
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/watermelon.png")) // картинка
                        }
                    };
                    break;
                case 2:
                    FMarker = new GMapMarker(point)
                    {
                        Shape = new Image
                        {
                            Width = 32, // ширина маркера
                            Height = 32, // высота маркера
                            ToolTip = this.getTitle(), // всплывающая подсказка
                            Margin = new System.Windows.Thickness(-12, -12, 0, 0),
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/milk.png")) // картинка
                        }
                    };
                    break;
                case 3:
                    FMarker = new GMapMarker(point)
                    {
                        Shape = new Image
                        {
                            Width = 32, // ширина маркера
                            Height = 32, // высота маркера
                            ToolTip = this.getTitle(), // всплывающая подсказка
                            Margin = new System.Windows.Thickness(-12, -12, 0, 0),
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/meat.png")) // картинка
                        }
                    };
                    break;
                case 4:
                    FMarker = new GMapMarker(point)
                    {
                        Shape = new Image
                        {
                            Width = 32, // ширина маркера
                            Height = 32, // высота маркера
                            ToolTip = this.getTitle(), // всплывающая подсказка
                            Margin = new System.Windows.Thickness(-12, -12, 0, 0),
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/egg.png")) // картинка
                        }
                    };
                    break;
                default:
                    break;
            }
            return FMarker;
        }

        public event EventHandler foodInCar;

        public override void CarArrived(object sender, EventArgs e)
        {
            Car car = (Car)sender;
            if (nextLoc != null)
            {
                car.Arrived -= this.CarArrived;
                MessageBox.Show("Я забрал "+this.getTitle());
                car.Arrived += this.nextLoc.CarArrived;
                //car.moveTo(nearstwarehouse.getFocus());
                foodInCar?.Invoke(this, EventArgs.Empty);
                this.foodInCar -= car.foodInCar;
            }
            else
            {
                MessageBox.Show("Доставлено");
            }
        }
    }
}
