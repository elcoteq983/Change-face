using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Windows.Controls;

namespace KinectFaceTracking
{
    class Gesture
    {
        public string basicoutput;

        MainWindow m = (MainWindow)App.Current.MainWindow;

        ObservableCollection<BitmapImage> bmList;
        public static Dictionary<int, int> images = new Dictionary<int, int>();

        public int index1 = 0;
        public int index2 = 1;

        public right r = new right();
        public left l = new left();

        public Gesture()
        {
            InitList();
        }

        public void InitList()
        {
            bmList = new ObservableCollection<BitmapImage>();
            string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 1; i < 15; i++)
            {
                BitmapImage bmImg = new BitmapImage(new Uri(applicationPath + "image\\" + i.ToString() + ".png"));
                bmList.Add(bmImg);
            }
        }

        public bool CheckSinglePoint(Skeleton skeleton, long frameTimestamp,KinectSensor KinectDevice, Grid LayoutRoot)
        {
            if (r.Update(skeleton, frameTimestamp, KinectDevice, LayoutRoot))
            {
                //更换图片
                Random temp = new Random();
                index1 = temp.Next(14);
                basicoutput = "向右挥手";

                int i;
                if (images.TryGetValue(skeleton.TrackingId, out i))
                {
                    if (i == 1)
                        m.image1.Source = bmList[index1];
                    else
                        m.image2.Source = bmList[index1];
                }

                return true;
            }
            if (l.Update(skeleton, frameTimestamp, KinectDevice, LayoutRoot))
            {
                //更换图片
                Random temp = new Random();
                index2 = temp.Next(14);
                basicoutput = "向左挥手";

                int i;
                if (images.TryGetValue(skeleton.TrackingId, out i))
                {
                    if (i == 1)
                        m.image1.Source = bmList[index2];
                    else
                        m.image2.Source = bmList[index2];
                }

                return true;
            }
            return false;
        }
    }
}
