using System;
using System.Collections.Generic;
using UnityEngine;

namespace KarenKrill.Diagnostics
{
    using Abstractions;
    using Utilities;

    public class DiagnosticsProvider : MonoBehaviour, IDiagnosticsProvider
    {
        public PerfomanceInfo PerfomanceInfo => _perfomanceInfo;

#nullable enable
        public event Action<PerfomanceInfo>? PerfomanceInfoChanged;
#nullable restore

        [SerializeField]
        private float _fpsUpdatePeriod = 0.1f;
        [SerializeField]
        private int _fpsWindowSize = 20;

        private int _fpsCounter = 0;
        private float _nextFpsUpdateTime = 0;
        private readonly Queue<float> _fpsWindow = new();
        private PerfomanceInfo _perfomanceInfo = new(0, 0, 0, 0);

        private void Start()
        {
            _nextFpsUpdateTime = Time.realtimeSinceStartup + _fpsUpdatePeriod;
        }
        private void Update()
        {
            UpdateFps();
        }

        private void UpdateFps()
        {
            _fpsCounter++;
            var delta = Time.realtimeSinceStartup - _nextFpsUpdateTime;
            if (delta >= 0)
            {
                var timeSinceLastFpsUpdate = _fpsUpdatePeriod + delta;
                var currentFps = _fpsCounter / timeSinceLastFpsUpdate;
                _fpsCounter = 0;
                _nextFpsUpdateTime += timeSinceLastFpsUpdate;
                if (_fpsWindow.Count == _fpsWindowSize)
                {
                    _ = _fpsWindow.Dequeue();
                }
                _fpsWindow.Enqueue(currentFps);
                _fpsWindow.MinAvgMax(out var min, out var avg, out var max);
                _perfomanceInfo = new(currentFps, avg, min, max);
                PerfomanceInfoChanged?.Invoke(_perfomanceInfo);
            }
        }
    }
}
