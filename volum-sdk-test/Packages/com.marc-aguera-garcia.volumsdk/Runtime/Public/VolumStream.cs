using System;
using UnityEngine;
using UnityEngine.Audio;
using Volum.SDK.Core;

namespace Volum.SDK
{
    public class VolumStream
    {
        public event Action<VolumFrame> OnFrameReady;
        public VolumStreamConfig Config { get; }

        private PointProcessor _processor;

        private float _accumulator;
        private float _fixedDeltaTime;

        internal VolumStream(VolumStreamConfig config)
        {
            Config = config;
            _fixedDeltaTime = 1f / Config.FrameRate;
            _processor = new PointProcessor(config);
        }

        public void Start()
        {
            _processor.Initialize();
            _accumulator = 0f;

            VolumPlayerLoop.Register(Tick);
        }

        // Update call
        public void Tick(float deltaTime)
        {
            _accumulator += deltaTime;

            while (_accumulator >= _fixedDeltaTime)
            {
                _accumulator -= _fixedDeltaTime;
                if (_processor.TryAdvance(deltaTime, out VolumFrame frame))
                {
                    OnFrameReady?.Invoke(frame);
                }
                
            }
        }

        public void Dispose()
        {
            Stop();
            _processor.Dispose();
        }

        public void Stop()
        {
            VolumPlayerLoop.Unregister(Tick);
            _processor.CompletePending();
        }
    }
}