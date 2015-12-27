using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectFaceTracking
{
    public struct Pose
    {
        public string Title;
        public PoseAngle[] Angles;

        public Pose(string title, PoseAngle[] angles)
        {
            Title = title;
            Angles = angles;
        }
    }
}
