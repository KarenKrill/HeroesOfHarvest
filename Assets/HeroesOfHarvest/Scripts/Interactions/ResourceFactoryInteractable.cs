using System.Collections;
using System.Threading;
using UnityEngine;

using KarenKrill.UniCore.Interactions.Abstractions;

using HeroesOfHarvest.Abstractions;

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

        public int ResourceAmount { get; set; }

        protected override bool OnInteraction(IInteractor interactor)
        {
            return true;
        }

        private CancellationTokenSource _cts = null;
        private Coroutine _produceCoroutine = null;

        private void OnEnable()
        {
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
            _cts.Cancel();
        }

        private IEnumerator ProduceCoroutine(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var resAmount = ResourceAmount + ResourceAmountPerSecond;
                if (resAmount > MaxResourceAmount)
                {
                    resAmount = MaxResourceAmount;
                }
                ResourceAmount = resAmount;
                yield return new WaitForSeconds(1);
            }
            _produceCoroutine = null;
        }
    }
}
