using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using Zenject;

using KarenKrill.UniCore.Interactions.Abstractions;

using HeroesOfHarvest.Abstractions;
using HeroesOfHarvest.Input.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    public class ResourceFactoryInteractable : MapObjectInteractable, IInteractable
    {
        [field: SerializeField]
        public ResourceType ProducedResource { get; private set; }
        [field: SerializeField]
        public int ResourceAmountPerSecond { get; private set; } = 1;
        [field: SerializeField]
        public int MaxResourceAmount { get; private set; } = 20;
        [field: SerializeField]
        public string MiningAnimationTrigger { get; private set; } = "StartDigging";
        [field: SerializeField]
        public string MiningStopAnimationTrigger { get; private set; } = "StopWork";

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
            _progressTextMesh.text = $"{_resourceAmount}/{MaxResourceAmount}";
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
                    var resAmount = ResourceAmount + ResourceAmountPerSecond;
                    if (resAmount > MaxResourceAmount)
                    {
                        resAmount = MaxResourceAmount;
                    }
                    ResourceAmount = resAmount;
                }
                yield return new WaitForSeconds(1);
            }
            _produceCoroutine = null;
        }
    }
}
