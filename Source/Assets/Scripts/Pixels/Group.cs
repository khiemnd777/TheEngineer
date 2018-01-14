using UnityEngine;
using System.Linq;

public class Group : MonoBehaviour
{
    public Pivot pivot;

    public static void Create()
    {
        var pixels = FindObjectsOfType<Pixel>();
        var selectedPixels = pixels.Where(x => x.selecting && !x.grouping);
        if (selectedPixels.Any())
        {
            var selectedPoints = selectedPixels.Select(x => x.transform.position).ToArray();
            var centerPoint = TransformUtility.ComputeCenterPoint(selectedPoints);
            var groupPrefab = Resources.Load<Group>(Constants.GROUP_PREFAB);
            var group = Instantiate<Group>(groupPrefab, centerPoint, Quaternion.identity);
            foreach (var pixel in selectedPixels)
            {
                pixel.grouping = true;
                pixel.transform.SetParent(group.transform);
            }
            selectedPoints = null;
            group = null;
            groupPrefab = null;
        }
    }

    public static void Ungroup(){
        var pixels = FindObjectsOfType<Pixel>();
        var selectedPixels = pixels.Where(x => x.selecting && x.grouping);
        if (selectedPixels.Any())
        {
            foreach(var pixel in selectedPixels){
                var groupOfPixel = pixel.GetComponentInParent<Group>();
                pixel.grouping = false;
                pixel.transform.parent = null;
                var pixelsInGroup = groupOfPixel.GetComponentsInChildren<Pixel>();
                if(pixelsInGroup.Length == 0)
                {
                    Destroy(groupOfPixel.gameObject);
                }
                groupOfPixel = null;
                pixelsInGroup = null;
            }
        }
    }
}