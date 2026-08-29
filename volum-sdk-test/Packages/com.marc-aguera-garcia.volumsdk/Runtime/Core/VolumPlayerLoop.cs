using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Volum.SDK.Core
{
    /// <summary>
    /// Custom Update loop
    /// </summary>
    internal static class VolumPlayerLoop
    {
        private struct VolumUpdate { }

        private static List<Action<float>> _listeners = new List<Action<float>>();
        private static bool _installed;

        internal static void Register(Action<float> listener)
        {
            EnsureInstalled();
            if (!_listeners.Contains(listener)) 
            {
                _listeners.Add(listener);
            }
        }

        internal static void Unregister(Action<float> listener)
        {
            _listeners.Remove(listener);
        }

        private static void EnsureInstalled()
        {
            if (_installed) return;

            PlayerLoopSystem root = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem[] subsystems = root.subSystemList;

            for (int i = 0; i < subsystems.Length; i++)
            {
                if (subsystems[i].type != typeof(Update)) continue;

                PlayerLoopSystem updatePhase = subsystems[i];
                var inner = new List<PlayerLoopSystem>(
                    updatePhase.subSystemList ?? Array.Empty<PlayerLoopSystem>())
                {
                    new PlayerLoopSystem
                    {
                        type = typeof(VolumUpdate),
                        updateDelegate = Advance
                    }
                };

                updatePhase.subSystemList = inner.ToArray();
                subsystems[i] = updatePhase;
                break;
            }

            root.subSystemList = subsystems;
            PlayerLoop.SetPlayerLoop(root);
            _installed = true;

            Application.quitting += Uninstall;
        }

        private static void Advance()
        {
            float dt = Time.deltaTime;

            // Plain indexed loop over a List<T> allocates nothing; listeners are only
            // added/removed from Start()/Stop(), never from within this callback.
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i](dt);
            }
        }

        private static void Uninstall()
        {
            _listeners.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticsForNewSession()
        {
            // Guards against stale delegates surviving an Editor "enter Play Mode without
            // domain reload" session boundary, where static fields would otherwise persist.
            _listeners.Clear();
            _installed = false;
        }
    }
}