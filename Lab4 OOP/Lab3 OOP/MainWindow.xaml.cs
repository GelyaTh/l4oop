﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;

namespace Lab3_OOP
{
    public partial class MainWindow : Window
    {
        List<MapObject> objs = new List<MapObject>();
        List<MapObject> objsInList = new List<MapObject>();
        List<PointLatLng> point = new List<PointLatLng>();
        List<PointLatLng> pts = new List<PointLatLng>();

        //для фокуса на такси
        public static GMapControl map;

        Human taxiClient;
        public static GMapMarker marker_taxiClient;
        Car taxiCar;
        public static GMapMarker marker_taxiCar;
        Location taxiClientDestination;
        public static GMapMarker marker_taxiClientDestination;
        

        public MainWindow()
        {
            InitializeComponent();
            cb1.IsChecked = true;
            map = Map;
        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            // настройка доступа к данным
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            // установка провайдера карт
            Map.MapProvider = OpenStreetMapProvider.Instance;

            // установка зума карты
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;

            // установка фокуса карты
            Map.Position = new PointLatLng(55.012823, 82.950359);

            // настройка взаимодействия с картой
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;

            PointLatLng point = new PointLatLng(55.016511, 82.946152);
        }

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            if (cb1.IsChecked == true)
                pts.Add(point);

            if (cb2.IsChecked == true)
            {
                objectList.Items.Clear();
                objsInList.Clear();
                //лямбда-функция для сортировки списка по указанному критерию
                objs.Sort((obj1, obj2) => obj1.getDistance(point).CompareTo(obj2.getDistance(point)));
                foreach (MapObject mapObject in objs)
                {
                    objsInList.Add(mapObject);
                    objectList.Items.Add("Расстояние до " + mapObject.getTitle() + " - " + mapObject.getDistance(point).ToString("0.00") + " метров");
                }
            }
        }

        private void addObj_Click(object sender, RoutedEventArgs e)
        {
            if (objType.SelectedIndex > -1 && objTitle.Text != "")
            {
                if (objType.SelectedIndex == 0)
                {
                    if (pts.Count == 1)
                    {
                        Car car = new Car(objTitle.Text, pts[0]);
                        objs.Add(car);
                        Map.Markers.Add(car.getMarker());
                        //objsInList.Add(car);
                        //objectList.Items.Add(car.getTitle());
                    }

                    else
                    {
                        MessageBox.Show("Нужна одна точка");
                    }
                }

                if (objType.SelectedIndex == 1)
                {
                    if (pts.Count == 1)
                    {
                        Human human = new Human(objTitle.Text, pts[0]);
                        objs.Add(human);
                        Map.Markers.Add(human.getMarker());
                        //objsInList.Add(human);
                        //objectList.Items.Add(human.getTitle());
                    }

                    else
                    {
                        MessageBox.Show("Нужна одна точка");
                    }

                }

                if (objType.SelectedIndex == 2)
                {
                    if (pts.Count == 1)
                    {
                        Location location = new Location(objTitle.Text, pts[0]);
                        objs.Add(location);
                        Map.Markers.Add(location.getMarker());
                        //objsInList.Add(location);
                        //objectList.Items.Add(location.getTitle());
                    }

                    else
                    {
                        MessageBox.Show("Нужна одна точка");
                    }
                }

                if (objType.SelectedIndex == 3)
                {
                    if (pts.Count > 2)
                    {
                        Area area = new Area(objTitle.Text, pts);
                        objs.Add(area);
                        Map.Markers.Add(area.getMarker());
                        //objsInList.Add(area);
                        //objectList.Items.Add(area.getTitle());
                    }

                    else
                    {
                        MessageBox.Show("Необходимо 3 или более точек");
                    }

                }

                if (objType.SelectedIndex == 4)
                {
                    if (pts.Count > 1)
                    {
                        Route route = new Route(objTitle.Text, pts);
                        objs.Add(route);
                        Map.Markers.Add(route.getMarker());
                        //objsInList.Add(route);
                        //objectList.Items.Add(route.getTitle());
                    }

                    else
                    {
                        MessageBox.Show("Необходимо 2 или более точек");
                    }
                }
            }
            else
            {
                MessageBox.Show("Введите название объекта");
            }

            //Map.Markers.Clear();
            objectList.Items.Clear();
            objsInList.Clear();

            //foreach (MapObject mapObject in objs)
            //{
            //    Map.Markers.Add(mapObject.getMarker());
            //    objsInList.Add(mapObject);
            //    objectList.Items.Add(mapObject.getTitle());
            //}

            pts.Clear();
        }

        private void cb1_Checked(object sender, RoutedEventArgs e)
        {
            if (cb1.IsChecked == true)
            {
                cb2.IsChecked = false;

                objType.IsEnabled = true;
                objTitle.IsEnabled = true;
                addObj.IsEnabled = true;
                clearObj.IsEnabled = true;

                pts.Clear();
            }
        }

        private void clearObj_Click(object sender, RoutedEventArgs e)
        {
            Map.Markers.Clear();
            objectList.Items.Clear();
            objs.Clear();
        }

        private void cb2_Checked(object sender, RoutedEventArgs e)
        {
            if (cb2.IsChecked == true)
            {
                cb1.IsChecked = false;

                objType.IsEnabled = false;
                objTitle.IsEnabled = false;
                addObj.IsEnabled = false;
                clearObj.IsEnabled = false;

                pts.Clear();
            }
        }

        private void objectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (objectList.SelectedIndex >= 0)
            {
                PointLatLng p = objsInList[objectList.SelectedIndex].getFocus();
                Map.Position = p;
                objectList.UnselectAll();
            }                
        }

        private void findObj_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Map.Markers.Clear();
            objectList.UnselectAll();
            objectList.Items.Clear();
            objsInList.Clear();

            foreach (MapObject mapObject in objs)
            {
                if (mapObject.getTitle().Contains(findObj.Text))
                {
                    //Map.Markers.Add(mapObject.getMarker());
                    objsInList.Add(mapObject);
                    objectList.Items.Add(mapObject.getTitle());
                }
            }
        }

        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            Map.Markers.Clear();
            foreach (MapObject mapObject in objs)
            {
                Map.Markers.Add(mapObject.getMarker());
            }
            if (button_SetDestination.IsCancel && taxiClient != null)
            {
                taxiClientDestination = new Location("DestinationPoint", point);
                marker_taxiClientDestination = taxiClientDestination.getMarker();
                taxiClient.Destination = point;
                Map.Markers.Add(marker_taxiClient);
                Map.Markers.Add(marker_taxiClientDestination);
            }
            else
            {
                taxiClient = new Human("TaxiClient", point);
                taxiClientDestination = null;
                marker_taxiClient = taxiClient.getMarker();
                Map.Markers.Add(marker_taxiClient);
            }


        }

        private void Button_SetDestination_Click(object sender, RoutedEventArgs e)
        {
            if (button_SetDestination.IsCancel)
            {
                button_SetDestination.Content = "Выбрать точку назначения";
                button_SetDestination.IsCancel = false;
            }
            else
            {
                button_SetDestination.Content = "Отмена";
                button_SetDestination.IsCancel = true;
            }
        }

        private void focus_follow(object sender, EventArgs args)
        {
            
            {
                Car car = (Car)sender;
                indi.Value =  100 * car.pos / car.route.Count;
                Map.Position = car.getFocus();
                
            }
        }

            private void Button_CallTaxi_Click(object sender, RoutedEventArgs e)
        {
            Route route = new Route(objTitle.Text, pts);
            if (taxiClient == null || taxiClientDestination == null)
                return;
            int distanceToNearestCar = int.MaxValue;
            foreach (MapObject mapObject in objs)
            {
                if (mapObject is Car someCar && someCar.getDistance(taxiClient.getFocus()) < distanceToNearestCar)
                {
                    taxiCar = someCar;
                    //someCar.Follow += focus_follow;
                }
            }
            if (taxiCar != null)
            {
                marker_taxiCar = taxiCar.getMarker();
                Map.Markers.Clear();
                Map.Markers.Add(marker_taxiClient);
                Map.Markers.Add(marker_taxiClientDestination);
                Map.Markers.Add(marker_taxiCar);
                Map.Markers.Add(route.getMarker());
                taxiCar.Arrived += taxiClient.CarArrived;
                taxiClient.Seated += taxiCar.passengerSeated;
                var route2 = taxiCar.moveTo(taxiClient.getFocus());
                Map.Markers.Add(route2.getMarker());
            }
            else
            {
                MessageBox.Show("Нет машин");
            }
            taxiCar.Follow += focus_follow;

        }
    }
}
