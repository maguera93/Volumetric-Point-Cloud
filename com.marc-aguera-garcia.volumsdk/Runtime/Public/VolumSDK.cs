using UnityEngine;

namespace Volum.SDK
{
    /// <summary>
    /// Entry point of the Volum SDK
    /// </summary>
    public static class VolumSDK
    {

        /// <summary>
        /// Creates a new Volum Stream with the given configuration.
        /// </summary>
        public static VolumStream InitStream(VolumStreamConfig config)
        {
            return new VolumStream(config);
        }
    }
}
