using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Volum.SDK;

namespace Volum.SDK.Core.Jobs
{
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Standard)]
    internal struct TransformPointJob : IJobParallelFor
    {
        private const float Y_DISPLACEMENT = 30F;
        private const float Z_DISPLACEMENT = 70F;
        private const float MOVEMENT = 0.6F;
        private const float ANGLE_MOVEMENT = 0.15F;

        public float Time;
        public float Speed;
        public float NoiseFrequency;
        public NativeArray<float3> BasePos;
        public NativeArray<VolumPoint> OutPut;

        //  Execute in parallel
        public void Execute(int index)
        {
            float3 basePos = BasePos[index];
            float acceleration = Time * Speed;

            // Perlin noise
            float x = noise.snoise(new float4(basePos * NoiseFrequency, acceleration));
            float y = noise.snoise(new float4(basePos.yzx * NoiseFrequency, acceleration + Y_DISPLACEMENT));
            float z = noise.snoise(new float4(basePos.zxy * NoiseFrequency, acceleration + Z_DISPLACEMENT));

            float3 newPos = basePos + new float3(x, y, z) * MOVEMENT;

            // Wave effect
            float angle = acceleration * ANGLE_MOVEMENT;
            float cs = math.cos(angle);
            float sn = math.sin(angle);
            newPos = new float3(newPos.x * cs - newPos.z * sn, newPos.y, newPos.x * sn + newPos.z * cs);

            float4 color = new float4(255, 255, 255, 255);

            OutPut[index] = new VolumPoint { Position = newPos, Color = color};
        }
    }
}