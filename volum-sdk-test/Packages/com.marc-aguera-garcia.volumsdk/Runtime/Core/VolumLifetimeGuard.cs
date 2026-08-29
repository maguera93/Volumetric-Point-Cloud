using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Volum.SDK.Core
{
    /// <summary>
    /// Safety net: force disposes all Volum Stream on Application Quit
    /// </summary>
    internal static class VolumLifetimeGuard
    {
        private static readonly List<VolumStream> _active = new List<VolumStream>();
        private static bool _hooked;

        internal static void Register(VolumStream stream)
        {
            EnsureHooked();
            _active.Add(stream);
        }

        internal static void Unregister(VolumStream stream)
        {
            _active.Remove(stream);
        }

        private static void EnsureHooked()
        {
            if (_hooked)
                return;

            Application.quitting += ForceDisposeAll;
            _hooked = true;
        }

        internal static void ForceDisposeAll()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                _active[i].Dispose();
            }

            _active.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticsForNewSession()
        {
            _active.Clear();
            _hooked = false;
        }
    }
}
