using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Volum.SDK.Core.Jobs;

namespace Volum.SDK.Core
{
    internal sealed class PointProcessor : IDisposable
    {
        private const int BATCH_SIZE = 64;

        private VolumStreamConfig _config;

        private JobHandle _handle;
        private bool _hasPending;
        private float _elapsed;

        private NativeArray<float3> _basePos;
        private NativeArray<VolumPoint> _bufferA;
        private NativeArray<VolumPoint> _bufferB;

        private int _writeIndex;

        public int FrameIndex { get; private set; }

        internal PointProcessor(VolumStreamConfig config)
        {
            _config = config;
        }

        // Initialize points procession
        internal void Initialize()
        {
            _basePos = new NativeArray<float3>(_config.PointCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _bufferA = new NativeArray<VolumPoint>(_config.PointCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _bufferB = new NativeArray<VolumPoint>(_config.PointCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            var genJob = new GenerationPointJob
            {
                BasePositions = _basePos,
                Radius = _config.VolumRadius
            };

            genJob.Schedule(_config.PointCount, BATCH_SIZE).Complete();

            _elapsed = 0;
            _writeIndex = 0;
            FrameIndex = 0;

            if (!_hasPending)
            {
                NextFrame();
            }
        }

        // Try to process next frame
        internal bool TryAdvance(float deltaTime, out VolumFrame frame)
        {
            _elapsed += deltaTime;

            if (!_hasPending)
            {
                frame = default;
                return false;
            }

            _handle.Complete();
            _hasPending = false;

            NativeArray<VolumPoint> ready = _writeIndex == 0 ? _bufferA : _bufferB;
            FrameIndex++;
            frame = new VolumFrame(ready, FrameIndex, _elapsed);

            // Change buffer
            _writeIndex = 1 - _writeIndex;
            NextFrame();

            return true;
        }

        // Process next frame, changes buffer to speed process
        internal void NextFrame()
        {
            NativeArray<VolumPoint> ready = _writeIndex == 0 ? _bufferA : _bufferB;

            var job = new TransformPointJob
            {
                Time = _elapsed,
                Speed = _config.Speed,
                NoiseFrequency = _config.NoiseFrequency,
                BasePos = _basePos,
                OutPut = ready
            };

            _handle = job.Schedule(_config.PointCount, BATCH_SIZE);
            _hasPending = true;
        }

        // Completes pending job
        internal void CompletePending()
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
            if (_bufferA.IsCreated) _bufferA.Dispose();
            if (_bufferB.IsCreated) _bufferB.Dispose();
        }
    }
}
