using HeroesOfHarvest.Abstractions;
using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace HeroesOfHarvest.Interactions
{
    [Serializable]
    public class StaticMapObjectId : IMapObjectId, IEquatable<IMapObjectId>, IEqualityComparer, IStringSerializable
    {
        public StaticMapObjectId(Vector3 worldPosition)
        {
            _worldPosition = worldPosition;
        }
        public bool Equals(IMapObjectId other)
        {
            if(other is StaticMapObjectId otherId)
            {
                return (Mathf.Abs(_worldPosition.x - otherId._worldPosition.x) <= float.Epsilon) &&
                    (Mathf.Abs(_worldPosition.y - otherId._worldPosition.y) <= float.Epsilon) &&
                    (Mathf.Abs(_worldPosition.z - otherId._worldPosition.z) <= float.Epsilon);
            }
            return false;
        }
        public new bool Equals(object x, object y)
        {
            if(x is StaticMapObjectId xId && y is StaticMapObjectId yId)
            {
                return (Mathf.Abs(xId._worldPosition.x - yId._worldPosition.x) <= float.Epsilon) &&
                    (Mathf.Abs(xId._worldPosition.y - yId._worldPosition.y) <= float.Epsilon) &&
                    (Mathf.Abs(xId._worldPosition.z - yId._worldPosition.z) <= float.Epsilon);
            }
            return false;
        }
        public int GetHashCode(object obj)
        {
            if (obj is StaticMapObjectId id)
            {
                return id._worldPosition.GetHashCode();
            }
            return base.GetHashCode();
        }

        public string ToSerializedString() => JsonUtility.ToJson(_worldPosition);
        public void FromSerializedString(string serializedString)
        {
            JsonUtility.FromJsonOverwrite(serializedString, _worldPosition);
        }

        [SerializeField]
        private Vector3 _worldPosition;
    }
}
