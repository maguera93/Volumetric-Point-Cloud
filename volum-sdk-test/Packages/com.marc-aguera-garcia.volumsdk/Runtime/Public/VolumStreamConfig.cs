using System;
using UnityEngine;

namespace Volum.SDK
{
    [Serializable]
    public struct VolumStreamConfig
    {
        public int PointCount;
        public int FrameRate;
        public float VolumRadius;
        public float Speed;
        public float NoiseFrequency;
        public uint Seed;
    }
}