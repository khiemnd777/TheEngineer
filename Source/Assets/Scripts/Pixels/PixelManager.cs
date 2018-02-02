using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PixelManager
{
    static PixelManager _instance;
    public static PixelManager instance
    {
        get
        {
            return _instance ?? (_instance = new PixelManager());
        }
    }

    List<Pixel> _pixels;

    ICollection<Pixel> pixels
    {
        get { return _pixels ?? (_pixels = new List<Pixel>()); }
    }

    public void AddPixel(Pixel pixel)
    {
        pixels.Add(pixel);
    }

    public void RemovePixel(int id)
    {
        foreach(var pixel in pixels){
            if(pixel.id == id)
            {
                pixels.Remove(pixel);
                return;    
            }
        }
    }

    public IEnumerable<Pixel> GetPixels(System.Func<Pixel, bool> predicate)
    {
        return pixels.Where(predicate);
    }

    public Pixel FindClosestPixel(Vector3 anchorPosition, float closestDistance, System.Func<Pixel, bool> filter = null)
    {
        Pixel bestPotential = null;
        var closestDistanceSqr = closestDistance * closestDistance;
        var currentPosition = anchorPosition;
        var pixels = this.pixels;
        var it = pixels
            .Where(x => {
                var directionToTarget = x.transform.position - currentPosition;
                var dSqrToTarget = directionToTarget.sqrMagnitude;
                return dSqrToTarget < closestDistanceSqr;
            });
        if(filter != null)
        {
            it = pixels.Where(filter);
        }
        var itArray = it.ToArray();
        foreach (var potentialTarget in itArray)
        {
            var directionToTarget = potentialTarget.transform.position - currentPosition;
            var dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestPotential = potentialTarget;
            }
        }
        itArray = null;
        pixels = null;
        it = null;
        return bestPotential;
    }
}