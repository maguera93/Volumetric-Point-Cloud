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
        private bool _hasPending;

        private float _speed = 10;
        private float _noiseFrequency = 1;
        private float _elapsed;

        private NativeArray<float3> _basePos;
        private NativeArray<VolumPoint> _result;
        private int CurrentFrame;


        public PointProcessor(VolumStreamConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            _basePos = new NativeArray<float3>(_config.PointCount, Allocator.Persistent);
            _result = new NativeArray<VolumPoint>(_config.PointCount, Allocator.Persistent);

            if (!_hasPending)
            {
                NextFrame();
            }
        }

        public bool TryAdvance(float deltaTime, out VolumFrame frame)
        {
            _elapsed += deltaTime;

            if (!_hasPending)
            {
                frame = default;
                return false;
            }

            _handle.Complete();
            _hasPending = false;

            CurrentFrame++;

            frame = new VolumFrame(_result, CurrentFrame, _elapsed);

            NextFrame();

            return true;
        }

        public void NextFrame()
        {
            var job = new TransformPointJob
            {
                Time = _elapsed,
                Speed = _speed,
                NoiseFrequency = _noiseFrequency,
                BasePos = _basePos,
                OutPut = _result
            };

            _handle = job.Schedule(_config.PointCount, BATCH_SIZE);
            _hasPending = true;
        }

        public void CompletePending()
        {
            if (_hasPending)
            {
                _handle.Complete();
                _hasPending = false;
            }
        }

        public void Dispose()
        {
            CompletePending();

            if (_basePos.IsCreated) _basePos.Dispose();
            if (_result.IsCreated) _result.Dispose();
        }
    }
}
