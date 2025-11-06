using System;
using UnityEngine;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.Movement
{
    /// <summary>
    /// It only moves one unit at a time because I don't have time to make the class more versatile
    /// </summary>
    public class UnitMover : NavMeshMoveBehaviour, IUnitMover
    {
        public event Action<IUnit> UnitPathCancelled;
        public event Action<IUnit> UnitPathCompleted;

        public bool TrySend(IUnit unit, Vector3 position)
        {
            if (!unit.IsBusy)
            {
                if (TryGoTo(position))
                {
                    _lastSentUnit = unit;
                    return true;
                }
            }
            return false;
        }

        protected override bool IsDestinationValid(Vector3 destination) => (!_lastSentUnit?.IsBusy) ?? true;
        protected override void OnPathCanceled()
        {
            base.OnPathCanceled();
            if (_lastSentUnit != null)
            {
                try
                {
                    UnitPathCancelled?.Invoke(_lastSentUnit);
                }
                finally
                {
                    _lastSentUnit = null;
                }
            }
        }
        protected override void OnPathCompleted()
        {
            base.OnPathCompleted();
            if (_lastSentUnit != null)
            {
                UnitPathCompleted?.Invoke(_lastSentUnit);
            }
        }

        private IUnit _lastSentUnit;
    }
}
