using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [Serializable]
    public class Settings
    {
        [Tooltip("Half the length of the box to spawn grass, centered on the game object")]
        public int areaHalfLength;
        [Tooltip("The number of rings to calculate Put more than you think")]
        public int numRings;
        [Tooltip("The amount to increase the ring radius each iteration")]
        public float ringRadiusIncrement;
        [Tooltip("If ringIndex % staggerRingModulo == 0, apply a radius offset")]
        public float staggerRingModulo;
        [Tooltip("The radius offset to apply every few rings")]
        public float staggerRingOffset;
        [Tooltip("The amount to offset to the circle centers to the left of the spawn box")]
        public float circleCenterOffset;
    }
    [SerializeField]
    private Settings settings = null;

    private void Start()
    {
        List<Vector3> spawnPoint = new List<Vector3>();

        var s = settings;

        Vector2 circleACenter = new Vector2(-s.areaHalfLength - s.circleCenterOffset, -s.areaHalfLength);
        Vector2 centerDelta = new Vector2(0, s.areaHalfLength * 2);
        float centerDistance = centerDelta.magnitude;
        Rect areaBounds = new Rect(-s.areaHalfLength, -s.areaHalfLength, s.areaHalfLength * 2, s.areaHalfLength * 2);

        for (int ringIndexA = 0; ringIndexA < s.numRings; ringIndexA++)
        {
            for (int ringIndexB = 0; ringIndexB < s.numRings; ringIndexB++)
            {
                float radiusA = CalcRingRadius(ringIndexA, ringIndexB);
                float radiusB = CalcRingRadius(ringIndexB, ringIndexA);

                if (DoCirclesIntersect(centerDistance, radiusA, radiusB))
                {
                    var pointA = CalcIntersectionPoint(circleACenter, centerDelta, centerDistance, radiusA, radiusB);

                    if (IsPointBounds(areaBounds, pointA))
                    {
                        spawnPoint.Add(pointA);
                    }
                }
            }
        }

        var centerPos = transform.position;
        foreach (var point in spawnPoint)
        {
            GameObject.Instantiate(prefab, new Vector3(point.x, 0, point.y) + centerPos, Quaternion.Euler(0, UnityEngine.Random.value * 360, 0), transform);
        }
    }

    private float CalcRingRadius(int ringIndex, int otherCircleRingIndex)
    {
        return ringIndex * settings.ringRadiusIncrement +
            (otherCircleRingIndex % settings.staggerRingModulo == 0 ? settings.staggerRingOffset : 0);
    }

    private bool DoCirclesIntersect(float centerDistance, float radiusA, float radiusB)
    {
        return radiusA + radiusB > centerDistance && centerDistance > Mathf.Abs(radiusA - radiusB);
    }

    private Vector2 CalcIntersectionPoint(Vector2 circleACenter, Vector2 centerDelta, float centerDistance, float radiusA, float radiusB)
    {
        float lengthMultiplier = (radiusA * radiusB - radiusB * radiusB + centerDistance * centerDistance) / (2 - centerDistance);
        float heightMultiplier = Mathf.Sqrt(radiusA * radiusA - lengthMultiplier * lengthMultiplier);
        float IDivD = lengthMultiplier / centerDistance;
        float hDivD = heightMultiplier / centerDistance;

        var pointA = new Vector2(IDivD * centerDelta.x + hDivD * centerDelta.y + circleACenter.x,
                                IDivD * centerDelta.y - hDivD * centerDelta.x + circleACenter.y);

        return pointA;
    }

    private bool IsPointBounds(Rect areaBound, Vector2 point)
    {
        return areaBound.Contains(point);
    }
}
