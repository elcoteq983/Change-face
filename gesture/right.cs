using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Controls;

namespace KinectFaceTracking
{
    enum GestureState
    {
        None = 0,
        Success = 1,
        Failure = 2,
        InProgress = 3,
    }

    enum RPosition
    {
        None = 0,
        Start = 1,
        Middle = 2,
        Last = 3,
    }

    struct RGestureTracker
    {
        public int IterationCount;
        public GestureState State;
        public long Timestamp;
        public RPosition StartPosition;
        public RPosition CurrentPosition;

        public void Reset()
        {
            IterationCount = 0;
            State = GestureState.None;
            Timestamp = 0;
            StartPosition = RPosition.None;
            CurrentPosition = RPosition.None;
        }

        public void UpdateState(GestureState state, long timestamp)
        {
            State = state;
            Timestamp = timestamp;
        }

        public void UpdatePosition(RPosition position, long timestamp)
        {
            if (CurrentPosition != position)
            {
                if (position == RPosition.Start)
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

    public class right
    {
        private const int Z_MOVEMENT_TIMEOUT = 5000;
        private const int REQUIRED_ITERATIONS = 3;

        private RGestureTracker _PlayerRTracker;

        private Pose[] poseLibrary_right;
        public right()
        {
            PopulatePoseLibrary_right();
        }
        private void PopulatePoseLibrary_right()
        {
            this.poseLibrary_right = new Pose[3];
             
            this.poseLibrary_right[0] = new Pose();
            this.poseLibrary_right[0].Title = "R1";
            this.poseLibrary_right[0].Angles = new PoseAngle[1];
            this.poseLibrary_right[0].Angles[0] = new PoseAngle(JointType.ElbowRight, JointType.WristRight,90, 20);

            this.poseLibrary_right[1] = new Pose();
            this.poseLibrary_right[1].Title = "R2";
            this.poseLibrary_right[1].Angles = new PoseAngle[1];
            this.poseLibrary_right[1].Angles[0] = new PoseAngle(JointType.ElbowRight, JointType.WristRight, 135, 20);

            this.poseLibrary_right[2] = new Pose();
            this.poseLibrary_right[2].Title = "R3";
            this.poseLibrary_right[2].Angles = new PoseAngle[1];
            this.poseLibrary_right[2].Angles[0] = new PoseAngle(JointType.ElbowRight, JointType.WristRight, 180, 20);
        }

        public bool Update(Skeleton skeleton, long frameTimestamp, KinectSensor KinectDevice, Grid LayoutRoot)
        {
            if (skeleton.TrackingState != SkeletonTrackingState.NotTracked)
            {
                if (TrackR(skeleton, ref this._PlayerRTracker, frameTimestamp, KinectDevice, LayoutRoot))
                    return true;
            }
            else
            {
                this._PlayerRTracker.Reset();
            }
            return false;
        }

        private bool TrackR(Skeleton skeleton, ref RGestureTracker tracker, long timestamp, KinectSensor KinectDevice, Grid LayoutRoot)
        {
            Joint hand = skeleton.Joints[JointType.HandRight];
            Joint elbow = skeleton.Joints[JointType.ElbowRight];

            if (hand.TrackingState != JointTrackingState.NotTracked && elbow.TrackingState != JointTrackingState.NotTracked)
            {

                if (tracker.State == GestureState.InProgress && tracker.Timestamp + Z_MOVEMENT_TIMEOUT < timestamp)
                {
                    tracker.UpdateState(GestureState.Failure, timestamp);
                }

                if (PoseStuck.IsPose(skeleton,this.poseLibrary_right[0], KinectDevice, LayoutRoot))
                {
                    tracker.UpdatePosition(RPosition.Start, timestamp);
                }
                else if (PoseStuck.IsPose(skeleton, this.poseLibrary_right[1], KinectDevice, LayoutRoot))
                {
                    tracker.UpdatePosition(RPosition.Middle, timestamp);
                }
                else if (PoseStuck.IsPose(skeleton, this.poseLibrary_right[2], KinectDevice, LayoutRoot))
                {
                    tracker.UpdatePosition(RPosition.Last, timestamp);
                }


                if (tracker.State != GestureState.Success && tracker.IterationCount >= REQUIRED_ITERATIONS && tracker.CurrentPosition == RPosition.Last)
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

