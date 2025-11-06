using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions.Abstractions;

using HeroesOfHarvest.Input.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    [Serializable]
    public class ResourceFactoryInteractable : MapObjectInteractable, IInteractable
    {
        [field: SerializeField]
        public FactoryBuildConfig BuildConfig { get; private set; }
        public int ResourceAmount
        {
            get => _resourceAmount;
            set
            {
                if (_resourceAmount != value)
                {
                    _resourceAmount= value;
                    OnResourceAmountChanged();
                }
            }
        }
        public bool ProducingEnabled { get; set; } = true;

        [Inject]
        public void Initialize(Canvas worldUiCanvas, IStrategyGameActionsProvider strategyGameActionsProvider)
        {
            _actionsProvider = strategyGameActionsProvider;
            _mainCamera = Camera.main;
            _worldUiCanvas = worldUiCanvas;
            _worldUiCanvas.worldCamera = _mainCamera;
        }
        public override string ToSerializedString() => _resourceAmount.ToString();
        public override void FromSerializedString(string serializedString)
        {
            if (int.TryParse(serializedString, out var resAmount))
            {
                _resourceAmount = resAmount;
            }
        }

        protected override bool OnInteraction(IInteractor interactor)
        {
            return true;
        }

        [SerializeField]
        private TextMeshPro _progressTextMesh;

        private IStrategyGameActionsProvider _actionsProvider;
        /// <summary>
        /// For world space UI elements (world progress bar, which not implemented yet).
        /// </summary>
        private Canvas _worldUiCanvas;
        private Camera _mainCamera;
        private CancellationTokenSource _cts = null;
        private Coroutine _produceCoroutine = null;
        [SerializeField, HideInInspector]
        private int _resourceAmount = 0;

        private void OnEnable()
        {
            _actionsProvider.CameraLook += OnLookOrMove;
            _actionsProvider.CameraMove += OnLookOrMove;
            if (_produceCoroutine != null)
            {
                StopCoroutine(_produceCoroutine);
            }
            _cts?.Dispose();
            _cts = new();
            OnResourceAmountChanged();
            _produceCoroutine = StartCoroutine(ProduceCoroutine(_cts.Token));
        }
        private void OnDisable()
        {
            _actionsProvider.CameraLook -= OnLookOrMove;
            _actionsProvider.CameraMove -= OnLookOrMove;
            _cts.Cancel();
        }

        private void OnResourceAmountChanged()
        {
            _progressTextMesh.text = $"{BuildConfig.ProducedResource}{Environment.NewLine}{_resourceAmount}/{BuildConfig.MaxResourceAmount}";
        }
        private void OnLookOrMove(Vector2 delta)
        {
            _progressTextMesh.transform.LookAt(_mainCamera.transform, _mainCamera.transform.up);
            _progressTextMesh.transform.Rotate(0, 180, 0);
        }
        private IEnumerator ProduceCoroutine(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (ProducingEnabled)
                {
                    var resAmount = ResourceAmount + BuildConfig.ResourceAmountPerSecond;
                    if (resAmount > BuildConfig.MaxResourceAmount)
                    {
                        resAmount = BuildConfig.MaxResourceAmount;
                    }
                    ResourceAmount = resAmount;
                }
                yield return new WaitForSeconds(1);
            }
            _produceCoroutine = null;
        }
    }
}
