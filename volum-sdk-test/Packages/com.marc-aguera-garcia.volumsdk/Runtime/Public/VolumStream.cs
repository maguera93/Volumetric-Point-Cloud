using System;
using UnityEngine;
using UnityEngine.Audio;
using Volum.SDK.Core;

namespace Volum.SDK
{
    /// <summary>
    /// Public handle to runing volumetric point stream
    /// 
    /// <code>
    /// var config = new VolumStreamConfig { PointCount = 50000, FrameRate = 60 };
    /// var stream = VolumSDK.InitStrean(config);
    /// stream.OnFrameReady += UpdateMesh;
    /// stream.Start();
    /// </code>
    /// </summary>
    public sealed class VolumStream : IDisposable
    {
        /// <summary>
        /// Raised once per streamed frame
        /// </summary>
        public event Action<VolumFrame> OnFrameReady;

        /// <summary>
        /// Configuration of this stream
        /// </summary>
        public VolumStreamConfig Config { get; }

        /// <summary>
        /// True when volum stream is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// True when volum stream is disposed. A disposed stream cannot be restarted
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Index of the most recently made frame
        /// </summary>
        public int FrameIndex => _processor.FrameIndex;

        private PointProcessor _processor;
        private float _accumulator;
        private float _fixedDeltaTime;

        internal VolumStream(VolumStreamConfig config)
        {
            Config = config;
            _fixedDeltaTime = 1f / Config.FrameRate;
            _processor = new PointProcessor(config);
            VolumLifetimeGuard.Register(this);
        }

        /// <summary>
        /// Call on Initialize
        /// </summary>
        public void Start()
        {
            if (IsRunning)
                return;

            _processor.Initialize();
            _accumulator = 0f;
            VolumPlayerLoop.Register(Tick);
            IsRunning = true;
        }

        /// <summary>
        /// Call every frame
        /// </summary>
        /// <param name="deltaTime"></param>
        private void Tick(float deltaTime)
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

        /// <summary>
        /// Stops the stream and releases memory.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            Stop();
            _processor.Dispose();
            VolumLifetimeGuard.Unregister(this);
            IsDisposed = true;
        }

        /// <summary>
        /// Stops ticking, but memory stays allocated
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
                return;

            VolumPlayerLoop.Unregister(Tick);
            _processor.CompletePending();
            IsRunning = false;
        }
    }
}