using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Controls;

namespace KinectFaceTracking
{
    enum LPosition
    {
        None = 0,
        Start = 1,
        Middle = 2,
        Last = 3,
    }

    struct LGestureTracker
    {
        public int IterationCount;
        public GestureState State;
        public long Timestamp;
        public LPosition StartPosition;
        public LPosition CurrentPosition;

        public void Reset()
        {
            IterationCount = 0;
            State = GestureState.None;
            Timestamp = 0;
            StartPosition = LPosition.None;
            CurrentPosition = LPosition.None;
        }

        public void UpdateState(GestureState state, long timestamp)
        {
            State = state;
            Timestamp = timestamp;
        }

        public void UpdatePosition(LPosition position, long timestamp)
        {
            if (CurrentPosition != position)
            {
                if (position == LPosition.Start)
                {
                    if (State != GestureState.InProgress)
                    {
                        State = GestureState.InProgress;
                        IterationCount = 0;
                        StartPosition = position;
                    }

                }
                IterationCount++;
                CurrentPosition = position;
                Timestamp = timestamp;
            }
        }
    }

    public class left
    {
        private const int Z_MOVEMENT_TIMEOUT = 5000;
        private const int REQUIRED_ITERATIONS = 3;

        private LGestureTracker _PlayerLTracker;

        private Pose[] poseLibrary_left;
        public left()
        {
            PopulatePoseLibrary_left();
        }
        private void PopulatePoseLibrary_left()
        {
            this.poseLibrary_left = new Pose[3];

            this.poseLibrary_left[0] = new Pose();
            this.poseLibrary_left[0].Title = "L1";
            this.poseLibrary_left[0].Angles = new PoseAngle[1];
            this.poseLibrary_left[0].Angles[0] = new PoseAngle(JointType.ElbowLeft, JointType.WristLeft, 90, 20);

            this.poseLibrary_left[1] = new Pose();
            this.poseLibrary_left[1].Title = "L2";
            this.poseLibrary_left[1].Angles = new PoseAngle[1];
            this.poseLibrary_left[1].Angles[0] = new PoseAngle(JointType.ElbowLeft, JointType.WristLeft, 45, 20);

            this.poseLibrary_left[2] = new Pose();
            this.poseLibrary_left[2].Title = "L3";
            this.poseLibrary_left[2].Angles = new PoseAngle[1];
            this.poseLibrary_left[2].Angles[0] = new PoseAngle(JointType.ElbowLeft, JointType.WristLeft, 0, 20);
        }

        public bool Update(Skeleton skeleton, long frameTimestamp, KinectSensor KinectDevice, Grid LayoutRoot)
        {
            if (skeleton.TrackingState != SkeletonTrackingState.NotTracked)
            {
                if (TrackL(skeleton, ref this._PlayerLTracker, frameTimestamp, KinectDevice, LayoutRoot))
                    return true;
            }
            else
            {
                this._PlayerLTracker.Reset();
            }
            return false;
        }

        private bool TrackL(Skeleton skeleton, ref LGestureTracker tracker, long timestamp, KinectSensor KinectDevice, Grid LayoutRoot)
        {
            Joint hand = skeleton.Joints[JointType.HandRight];
            Joint elbow = skeleton.Joints[JointType.ElbowRight];

            if (hand.TrackingState != JointTrackingState.NotTracked && elbow.TrackingState != JointTrackingState.NotTracked)
            {

                if (tracker.State == GestureState.InProgress && tracker.Timestamp + Z_MOVEMENT_TIMEOUT < timestamp)
                {
                    tracker.UpdateState(GestureState.Failure, timestamp);
                }

                if (PoseStuck.IsPose(skeleton, this.poseLibrary_left[0], KinectDevice, LayoutRoot))
                {
                    tracker.UpdatePosition(LPosition.Start, timestamp);
                }
                else if (PoseStuck.IsPose(skeleton, this.poseLibrary_left[1], KinectDevice, LayoutRoot))
                {
                    tracker.UpdatePosition(LPosition.Middle, timestamp);
                }
                else if (PoseStuck.IsPose(skeleton, this.poseLibrary_left[2], KinectDevice, LayoutRoot))
                {
                    tracker.UpdatePosition(LPosition.Last, timestamp);
                }


                if (tracker.State != GestureState.Success && tracker.IterationCount >= REQUIRED_ITERATIONS && tracker.CurrentPosition == LPosition.Last)
                {
                    tracker.UpdateState(GestureState.Success, timestamp);
                    return true;
                }
            }
            else
            {
                tracker.Reset();
            }
            return false;
        }
    }
}

