using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bankd.Core
{
    [Serializable]
    public class CallbackData
    {
        public float _timeRemaining;
        public Action Callback;
    }

    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _sInstance;

        private readonly Dictionary<Guid, Coroutine> _runningCoroutines = new();
        private readonly Dictionary<Guid, CallbackData> _runningInvokes = new();

        public static CoroutineManager Instance => _sInstance;

        private void Awake()
        {
            _sInstance ??= this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Starts a managed coroutine with a specific duration and callback. 
        /// Tracks the coroutine using a unique ID for control and cleanup after completion.
        /// </summary>
        /// /// <param name="routine">The coroutine to start.</param>
        public Guid StartManagedCoroutine(IEnumerator routine)
        {
            var id = Guid.NewGuid();
            var coroutine = StartCoroutine(TrackCoroutine(routine, id));
            _runningCoroutines.Add(id, coroutine);

            return id;
        }

        /// <summary>
        /// Starts a coroutine and tracks it using a unique ID. 
        /// This allows for managing its lifecycle, such as stopping it or checking its state.
        /// </summary>
        /// /// <param name="routine">The coroutine to start.</param>
        public Guid StartTrackedCoroutine(IEnumerator routine)
        {
            var id = Guid.NewGuid();
            var coroutine = StartCoroutine(TrackCoroutine(routine, id));

            _runningCoroutines.Add(id, coroutine);

            return id;
        }

        public void StopCoroutine(Guid id)
        {
            if (!_runningCoroutines.TryGetValue(id, out var coroutine)) return;

            StopCoroutine(coroutine);
            _runningCoroutines.Remove(id);
        }

        public void AbortAllCoroutines()
        {
            foreach (var coroutine in _runningCoroutines.Values)
                StopCoroutine(coroutine);

            _runningCoroutines.Clear();
        }

        private IEnumerator TrackCoroutine(IEnumerator routine, Guid id)
        {
            yield return routine;

            _runningCoroutines.Remove(id);
        }

        /// <summary>
        /// Starts a coroutine with the specified duration and callback.
        /// </summary>
        /// <param name="duration">Time to wait before invoking the callback.</param>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>A unique ID for the coroutine, used to stop it if needed.</returns>
        public Guid AddInvoke(float duration, Action callback)
        {
            var id = Guid.NewGuid();
            _runningInvokes[id] = new CallbackData
            {
                _timeRemaining = duration,
                Callback = callback,
            };

            Debug.Log($"started invoke with id: {id}");
            return id;
        }

        public void CancelInvoke(Guid id)
        {
            if(!_runningInvokes.ContainsKey(id)) return;

            _runningInvokes.Remove(id);
        }

        public void AbortAllInvokes()
        {
            _runningInvokes.Clear();
        }
        
        private Coroutine _tickHandlerCoroutine;
        private void OnEnable()
        {
            _tickHandlerCoroutine = StartCoroutine(TickHandler());
        }

        private void OnDisable()
        {
            if (_tickHandlerCoroutine is not null)
                StopCoroutine(_tickHandlerCoroutine);
        }

        private IEnumerator TickHandler()
        {
            while (true)
            {
                foreach (var key in new List<Guid>(_runningInvokes.Keys))
                {
                    var data = _runningInvokes[key];

                    data._timeRemaining -= Time.deltaTime;

                    if (!(data._timeRemaining <= 0)) continue;

                    data.Callback?.Invoke();
                    _runningInvokes.Remove(key);
                }

                yield return null;
            }
        }
    }
}