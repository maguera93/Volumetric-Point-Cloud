#if UNITY_EDITOR
using UnityEditor;
using Volum.SDK.Core;

namespace Volum.SDK.Editor
{
    internal static class VolumEditorLifetime
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= VolumLifetimeGuard.ForceDisposeAll;
            AssemblyReloadEvents.beforeAssemblyReload += VolumLifetimeGuard.ForceDisposeAll;
        }
    }
}
#endif
