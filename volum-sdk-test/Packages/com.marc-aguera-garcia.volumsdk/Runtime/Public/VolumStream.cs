using UnityEngine;

namespace Volum.SDK
{
    public class VolumStream
    {
        public VolumStreamConfig Config { get; }

        internal VolumStream(VolumStreamConfig config)
        {
            Config = config;
        }
    }
}