using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

using KarenKrill.UniCore.Interactions.Abstractions;

using HeroesOfHarvest.Input.Abstractions;

namespace HeroesOfHarvest.Interactions
{
    [Serializable]
    public class ResourceFactoryInteractable : MapObjectInteractable, IInteractable
    {
        public override event Action StateChanged;

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

        protected override void Awake()
        {
            _infoUiPanel.transform.SetParent(_worldUiCanvas.transform);
            _infoUiPanel.gameObject.SetActive(false);
            base.Awake();
        }
        protected override void OnDestroy()
        {
            if (!_isAppQuit && _infoUiPanel != null)
            {
                Destroy(_infoUiPanel.gameObject);
            }
            base.OnDestroy();
        }
        protected virtual void OnApplicationQuit()
        {
            _isAppQuit = true;
        }

        protected override bool OnInteraction(IInteractor interactor)
        {
            return true;
        }
        protected override void OnInteractionAvailabilityChanged(bool available)
        {
            base.OnInteractionAvailabilityChanged(available);
            if (available)
            {
                Cursor.SetCursor(_interactionCursor.texture2D, _interactionCursor.hotspot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        [SerializeField]
        private UIBehaviour _infoUiPanel;
        [SerializeField]
        private TextMeshProUGUI _progressTextMesh;
        [SerializeField]
        private CursorSettings _interactionCursor;
        [SerializeField, HideInInspector]
        private int _resourceAmount = 0;

        private IStrategyGameActionsProvider _actionsProvider;
        /// <summary>
        /// For world space UI elements (world progress bar, which not implemented yet).
        /// </summary>
        private Canvas _worldUiCanvas;
        private Camera _mainCamera;
        private CancellationTokenSource _cts = null;
        private Coroutine _produceCoroutine = null;
        private bool _isAppQuit = false;

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
            OnLookOrMove(Vector2.zero);
            _infoUiPanel.gameObject.SetActive(true);
        }
        private void OnDisable()
        {
            _actionsProvider.CameraLook -= OnLookOrMove;
            _actionsProvider.CameraMove -= OnLookOrMove;
            _cts.Cancel();
            if (!_isAppQuit && _infoUiPanel != null && _infoUiPanel.gameObject != null)
            {
                _infoUiPanel.gameObject.SetActive(false);
            }
        }

        private void OnResourceAmountChanged()
        {
            _progressTextMesh.text = $"{BuildConfig.ProducedResource}{Environment.NewLine}{_resourceAmount}/{BuildConfig.MaxResourceAmount}";
            StateChanged?.Invoke();
        }
        private void OnLookOrMove(Vector2 delta)
        {
            _infoUiPanel.transform.forward = _mainCamera.transform.forward;
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

        [Serializable]
        private class CursorSettings
        {
            public Texture2D texture2D;
            public Vector2 hotspot;
        }
    }

}
