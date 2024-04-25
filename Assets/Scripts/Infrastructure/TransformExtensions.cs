using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Infrastructure
{
    public static class TransformExtensions
    {
        [CanBeNull]
        public static (Transform, float) Closest(this IEnumerable<Transform> transforms, Vector3 position)
        {
            var minimalDistance = float.MaxValue;
            Transform closestTransform = null;
            foreach (var transform in transforms)
            {
                var distance = Vector3.Distance(transform.position, position);

                if (distance < minimalDistance)
                {
                    minimalDistance = distance;
                    closestTransform = transform;
                }
            }

            return (closestTransform, minimalDistance);
        }
    }
}