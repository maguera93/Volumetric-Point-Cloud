using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Volum.SDK.Core.Jobs;

namespace Volum.SDK.Core
{
    public class PointProcessor
    {
        private const int BATCH_SIZE = 64;

        private VolumStreamConfig _config;

        private JobHandle _handle;
        public Transform[] spheres;

        public float _speed = 10;
        public float _noiseFrequency = 1;

        private NativeArray<float3> _basePos;
        public NativeArray<VolumPoint> result;


        public PointProcessor(VolumStreamConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            _basePos = new NativeArray<float3>(spheres.Length, Allocator.Persistent);
            result = new NativeArray<VolumPoint>(spheres.Length, Allocator.Persistent);

            for (int i = 0; i < spheres.Length; i++)
            {
                _basePos[i] = spheres[i].transform.position;
            }
        }

        public void Tick()
        {
            _handle.Complete();

            for (int i = 0; i < spheres.Length; i++)
            {
                spheres[i].position = result[i].Position;
            }

            var job = new TransformPointJob
            {
                Time = Time.time,
                Speed = _speed,
                NoiseFrequency = _noiseFrequency,
                BasePos = _basePos,
                OutPut = result
            };

            _handle = job.Schedule(spheres.Length, 64);
        }

        public void CompletePending()
        {
            _handle.Complete();
        }

        public void Dispose()
        {
            _handle.Complete();

            if (_basePos.IsCreated) _basePos.Dispose();
            if (result.IsCreated) result.Dispose();
        }
    }
}
