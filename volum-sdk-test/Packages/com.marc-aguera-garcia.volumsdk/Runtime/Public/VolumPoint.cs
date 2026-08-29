using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace Volum.SDK
{
    /// <summary>
    /// Voulum point info
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VolumPoint
    {
        public float3 Position;
        public float4 Color;
    }
}