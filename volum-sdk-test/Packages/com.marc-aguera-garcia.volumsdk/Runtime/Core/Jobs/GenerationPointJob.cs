using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Volum.SDK.Core.Jobs
{
    [GenerateTestsForBurstCompatibility]
    internal struct GenerationPointJob : IJobParallelFor
    {
        public NativeArray<float3> BasePositions;
        public float Radius;
        public uint Seed;

        public void Execute(int index)
        {
            Random rnd = Random.CreateFromIndex((uint) index * 0x9E3779B1u + Seed);

            float3 dir = rnd.NextFloat3Direction();
            float r = math.pow(rnd.NextFloat(), 1f / 3f) * Radius;

            BasePositions[index] = dir * r;

        }
    }
}
