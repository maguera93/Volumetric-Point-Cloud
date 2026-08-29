using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Volum.SDK
{
    /// <summary>
    /// Process a frame
    /// </summary>
    public class VolumFrame
    {
        public readonly NativeArray<VolumPoint> Points;
        public int PointCount;
        public int FrameIndex;
        public float TimeSeconds;

        internal VolumFrame(NativeArray<VolumPoint> points, int frameIndex, float timeSeconds)
        {
            Points = points;
            PointCount = points.Length;
            FrameIndex = frameIndex;
            TimeSeconds = timeSeconds;
        }
    }
}