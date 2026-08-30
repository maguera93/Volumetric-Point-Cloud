using System;
using UnityEngine;

namespace Volum.SDK
{
    /// <summary>
    /// Volum Streaming Configuration
    /// </summary>
    [Serializable]
    public struct VolumStreamConfig
    {
        public int PointCount;
        public int FrameRate;
        public float VolumRadius;
        public float Speed;
        public float NoiseFrequency;
    }
}