using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TransformUtility
{
    public static T FindClosestObjectsOfType<T>(Vector3 anchorPosition, float closestDistance, System.Func<T, bool> filter = null) where T : Component
    {
        var objs = GameObject.FindObjectsOfType<T>();
        if(filter != null){
            objs = objs.Where(filter).ToArray();
        }
        var bestPotential = FindClosestObjectsBySpecific<T>(anchorPosition, closestDistance, objs);
        objs = null;
        return bestPotential;
    }

    public static T FindClosestObjectsOfType<T>(IEnumerable<T> objs, Vector3 anchorPosition, float closestDistance) where T : Component
    {
        var bestPotential = FindClosestObjectsBySpecific<T>(anchorPosition, closestDistance, objs.ToArray());
        objs = null;
        return bestPotential;
    }

    public static T FindClosestObjectsBySpecific<T>(Vector3 anchorPosition, float closestDistance, params T[] specificObjects) where T : Component
    {
        T bestPotential = null;
        var closestDistanceSqr = closestDistance * closestDistance;
        var currentPosition = anchorPosition;
        foreach (var potentialTarget in specificObjects)
        {
            var directionToTarget = potentialTarget.transform.position - currentPosition;
            var dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestPotential = potentialTarget;
            }
        }
        return bestPotential;
    }

    public static Vector3 ComputeCenterPoint(Vector3[] points)
    {
        var center = Vector3.zero;
        for(var i = 0; i < points.Length; i++){
            center += points[i];
        }
        return center / points.Length;
    }

    public static Vector2 ComputeCenterPoint2(Vector2[] points)
    {
        var center = Vector2.zero;
        for(var i = 0; i < points.Length; i++){
            center += points[i];
        }
        return center / points.Length;
    }
}