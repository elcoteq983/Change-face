using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;
using System.Drawing;

namespace KinectFaceTracking
{
    using Point = System.Windows.Point;

    public partial class FaceImage : UserControl, IDisposable
    {
        MainWindow m = (MainWindow)App.Current.MainWindow;

        public static readonly DependencyProperty KinectProperty = DependencyProperty.Register(
            "Kinect",
            typeof(KinectSensor),
            typeof(FaceImage),
            new PropertyMetadata(
                null, (o, args) => ((FaceImage)o).OnSensorChanged((KinectSensor)args.OldValue, (KinectSensor)args.NewValue)));

        private const uint MaxMissedFrames = 5;

        private readonly Dictionary<int, SkeletonFaceTracker> trackedSkeletons = new Dictionary<int, SkeletonFaceTracker>();

        private byte[] colorImage;

        private ColorImageFormat colorImageFormat = ColorImageFormat.Undefined;

        private short[] depthImage;

        private DepthImageFormat depthImageFormat = DepthImageFormat.Undefined;

        private bool disposed;

        private Skeleton[] skeletonData;

        public FaceImage()
        {
            this.InitializeComponent();
        }

        ~FaceImage()
        {
            this.Dispose(false);
        }

        public KinectSensor Kinect
        {
            get
            {
                return (KinectSensor)this.GetValue(KinectProperty);
            }

            set
            {
                this.SetValue(KinectProperty, value);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.ResetFaceTracking();

                this.disposed = true;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            int i = 1;
            foreach (SkeletonFaceTracker faceInformation in this.trackedSkeletons.Values)
            {
                foreach (int key in this.trackedSkeletons.Keys)
                    if (this.trackedSkeletons[key] == faceInformation)
                        i = key;
                faceInformation.DrawFaceModel(drawingContext, Gesture.images[i]);
            }

        }

        private void OnAllFramesReady(object sender, AllFramesReadyEventArgs allFramesReadyEventArgs)
        {
            ColorImageFrame colorImageFrame = null;
            DepthImageFrame depthImageFrame = null;
            SkeletonFrame skeletonFrame = null;

            try
            {
                colorImageFrame = allFramesReadyEventArgs.OpenColorImageFrame();
                depthImageFrame = allFramesReadyEventArgs.OpenDepthImageFrame();
                skeletonFrame = allFramesReadyEventArgs.OpenSkeletonFrame();

                if (colorImageFrame == null || depthImageFrame == null || skeletonFrame == null)
                {
                    return;
                }

                // Check for image format changes.  The FaceTracker doesn't
                // deal with that so we need to reset.
                if (this.depthImageFormat != depthImageFrame.Format)
                {
                    this.ResetFaceTracking();
                    this.depthImage = null;
                    this.depthImageFormat = depthImageFrame.Format;
                }

                if (this.colorImageFormat != colorImageFrame.Format)
                {
                    this.ResetFaceTracking();
                    this.colorImage = null;
                    this.colorImageFormat = colorImageFrame.Format;
                }

                // Create any buffers to store copies of the data we work with
                if (this.depthImage == null)
                {
                    this.depthImage = new short[depthImageFrame.PixelDataLength];
                }

                if (this.colorImage == null)
                {
                    this.colorImage = new byte[colorImageFrame.PixelDataLength];
                }

                // Get the skeleton information
                if (this.skeletonData == null || this.skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                {
                    this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                colorImageFrame.CopyPixelDataTo(this.colorImage);
                depthImageFrame.CopyPixelDataTo(this.depthImage);
                skeletonFrame.CopySkeletonDataTo(this.skeletonData);

                // Update the list of trackers and the trackers with the current frame information
                int num = 0; Skeleton sk1 = null, sk2 = null;
                foreach (Skeleton skeleton in this.skeletonData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (num == 0)
                            sk1 = skeleton;
                        else
                            sk2 = skeleton;
                        num++;
                    }
                }

                if (sk1 != null)
                {
                    if (sk2 == null)
                    {
                        if (!this.trackedSkeletons.ContainsKey(sk1.TrackingId))
                        {
                            this.trackedSkeletons.Add(sk1.TrackingId, new SkeletonFaceTracker());
                            Gesture.images.Add(sk1.TrackingId, 1);
                        }
                    }
                    else
                    {
                        if (!this.trackedSkeletons.ContainsKey(sk1.TrackingId))
                            if (!this.trackedSkeletons.ContainsKey(sk2.TrackingId))//都无
                            {
                                this.trackedSkeletons.Add(sk1.TrackingId, new SkeletonFaceTracker());
                                this.trackedSkeletons.Add(sk2.TrackingId, new SkeletonFaceTracker());
                                Gesture.images.Add(sk1.TrackingId, 1);
                                Gesture.images.Add(sk2.TrackingId, 2);
                            }
                            else//1无2有
                            {
                                this.trackedSkeletons.Add(sk1.TrackingId, new SkeletonFaceTracker());
                                if (Gesture.images[sk2.TrackingId] == 1)
                                    Gesture.images.Add(sk1.TrackingId, 2);
                                else
                                    Gesture.images.Add(sk1.TrackingId, 1);
                            }
                        else
                        {
                            if (!this.trackedSkeletons.ContainsKey(sk2.TrackingId))//1有2无
                            {
                                this.trackedSkeletons.Add(sk2.TrackingId, new SkeletonFaceTracker());
                                if (Gesture.images[sk1.TrackingId] == 1)
                                    Gesture.images.Add(sk2.TrackingId, 2);
                                else
                                    Gesture.images.Add(sk2.TrackingId, 1);
                            }
                        }

                    }

                }

                SkeletonFaceTracker skeletonFaceTracker;
                if (sk1 != null)
                {
                    if (this.trackedSkeletons.TryGetValue(sk1.TrackingId, out skeletonFaceTracker))
                    {
                        skeletonFaceTracker.OnFrameReady(this.Kinect, colorImageFormat, colorImage, depthImageFormat, depthImage, sk1);
                        skeletonFaceTracker.LastTrackedFrame = skeletonFrame.FrameNumber;
                    }
                }
                if (sk2 != null)
                {
                    if (this.trackedSkeletons.TryGetValue(sk2.TrackingId, out skeletonFaceTracker))
                    {
                        skeletonFaceTracker.OnFrameReady(this.Kinect, colorImageFormat, colorImage, depthImageFormat, depthImage, sk2);
                        skeletonFaceTracker.LastTrackedFrame = skeletonFrame.FrameNumber;
                    }
                }

                this.RemoveOldTrackers(skeletonFrame.FrameNumber);

                this.InvalidateVisual();
            }
            finally
            {
                if (colorImageFrame != null)
                {
                    colorImageFrame.Dispose();
                }

                if (depthImageFrame != null)
                {
                    depthImageFrame.Dispose();
                }

                if (skeletonFrame != null)
                {
                    skeletonFrame.Dispose();
                }
            }
        }

        private void OnSensorChanged(KinectSensor oldSensor, KinectSensor newSensor)
        {
            if (oldSensor != null)
            {
                oldSensor.AllFramesReady -= this.OnAllFramesReady;
                this.ResetFaceTracking();
            }

            if (newSensor != null)
            {
                newSensor.AllFramesReady += this.OnAllFramesReady;
            }
        }

        /// <summary>
        /// Clear out any trackers for skeletons we haven't heard from for a while
        /// </summary>
        private void RemoveOldTrackers(int currentFrameNumber)
        {
            var trackersToRemove = new List<int>();

            int i = 0, j = 0, num = 0;
            foreach (var tracker in this.trackedSkeletons)
            {
                uint missedFrames = (uint)currentFrameNumber - (uint)tracker.Value.LastTrackedFrame;
                if (missedFrames > MaxMissedFrames)
                {
                    // There have been too many frames since we last saw this skeleton
                    trackersToRemove.Add(tracker.Key);
                    if (num == 0)
                        i++;
                    else
                        j++;
                }
                num++;
            }

            foreach (int trackingId in trackersToRemove)
            {
                this.RemoveTracker(trackingId);
                if (i != 0)
                    this.m.image1.Visibility = Visibility.Hidden;
                if (j != 0)
                    this.m.image2.Visibility = Visibility.Hidden;
            }
        }

        private void RemoveTracker(int trackingId)
        {
            this.trackedSkeletons[trackingId].Dispose();
            this.trackedSkeletons.Remove(trackingId);
            Gesture.images.Remove(trackingId);
        }

        private void ResetFaceTracking()
        {
            foreach (int trackingId in new List<int>(this.trackedSkeletons.Keys))
            {
                this.RemoveTracker(trackingId);
            }
            this.m.image1.Visibility = Visibility.Hidden;
            this.m.image2.Visibility = Visibility.Hidden;
        }

        private class SkeletonFaceTracker : IDisposable
        {
            MainWindow m = (MainWindow)App.Current.MainWindow;

            FaceTrackFrame frame;

            private EnumIndexableCollection<FeaturePoint, Microsoft.Kinect.Toolkit.FaceTracking.PointF> facePoints;

            private FaceTracker faceTracker;

            private bool lastFaceTrackSucceeded;

            private SkeletonTrackingState skeletonTrackingState;

            public int LastTrackedFrame { get; set; }

            public void Dispose()
            {
                if (this.faceTracker != null)
                {
                    this.faceTracker.Dispose();
                    this.faceTracker = null;
                }
            }

            public void DrawFaceModel(DrawingContext drawingContext, int i)
            {
                if (this.frame == null || this.facePoints == null || this.faceTracker == null)
                {
                    if (i == 1)
                        m.image1.Visibility = Visibility.Hidden;
                    else
                        m.image2.Visibility = Visibility.Hidden;
                    return;
                }

                double temp = System.Math.Atan2(this.facePoints[0].Y - this.facePoints[10].Y, this.facePoints[0].X - this.facePoints[10].X);
                temp *= 180 / 3.14159; temp += 90;//用来确定旋转度数
                double length = System.Math.Sqrt((this.facePoints[0].Y - this.facePoints[10].Y) * (this.facePoints[0].Y - this.facePoints[10].Y) + (this.facePoints[0].X - this.facePoints[10].X) * (this.facePoints[0].X - this.facePoints[10].X));//用来确定尺寸
                //this.facePoints[10]用来确定位置

                TransformGroup transformGroup = new TransformGroup();

                ScaleTransform scaletrans = new ScaleTransform(length / 135.0*1.5, length / 135.0*1.5, 60.0, 135.0);
                RotateTransform rotatetrans = new RotateTransform(temp, 60.0, 135.0);
                TranslateTransform translatetrans = new TranslateTransform(this.facePoints[10].X*1.5 +80, this.facePoints[10].Y*1.5 - 135.0);

                transformGroup.Children.Add(scaletrans);
                transformGroup.Children.Add(rotatetrans);
                transformGroup.Children.Add(translatetrans);

                if (i == 1)
                {
                    m.image1.RenderTransform = transformGroup;
                    m.image1.Visibility = Visibility.Visible;
                }
                else
                {
                    m.image2.RenderTransform = transformGroup;
                    m.image2.Visibility = Visibility.Visible;
                }
            }

            // Updates the face tracking information for this skeleton
            internal void OnFrameReady(KinectSensor kinectSensor, ColorImageFormat colorImageFormat, byte[] colorImage, DepthImageFormat depthImageFormat, short[] depthImage, Skeleton skeletonOfInterest)
            {
                this.skeletonTrackingState = skeletonOfInterest.TrackingState;

                if (this.skeletonTrackingState != SkeletonTrackingState.Tracked)
                {
                    // nothing to do with an untracked skeleton.
                    return;
                }

                if (this.faceTracker == null)
                {
                    try
                    {
                        this.faceTracker = new FaceTracker(kinectSensor);
                    }
                    catch (InvalidOperationException)
                    {
                        Debug.WriteLine("AllFramesReady - creating a new FaceTracker threw an InvalidOperationException");
                        this.faceTracker = null;
                    }
                }

                if (this.faceTracker != null)
                {
                    frame = this.faceTracker.Track(
                        colorImageFormat, colorImage, depthImageFormat, depthImage, skeletonOfInterest);

                    this.lastFaceTrackSucceeded = frame.TrackSuccessful;
                    if (this.lastFaceTrackSucceeded)
                    {
                        this.facePoints = frame.GetProjected3DShape();
                    }
                }
            }
        }
    }
}