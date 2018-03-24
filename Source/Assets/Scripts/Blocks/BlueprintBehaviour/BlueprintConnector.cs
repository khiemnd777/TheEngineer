using UnityEngine;
using UnityEngine.UI.Extensions;

public class BlueprintConnector : MonoBehaviour
{
    public static BlueprintConnector current;

    public RectTransform headA;
    public RectTransform headB;
    public UILineRenderer lineRenderer;

    [System.NonSerialized]
    public BlueprintEntity a;
    [System.NonSerialized]
    public BlueprintEntity b;

    [System.NonSerialized]
    public Transform anchorA;
    [System.NonSerialized]
    public Transform anchorB;

    void Start()
    {
        
    }

    void Update()
    {
        lineRenderer.gameObject.SetActive(anchorA != null && anchorB != null);
        // detect position of A and snag into head of A
        if(headA != null && !headA.Equals(null) && anchorA != null)
        {
            headA.position = anchorA.position;

            var rectA = anchorA.GetComponent<RectTransform>();
            var rectA_anchoredPosition = rectA.anchoredPosition;
            var rectParentA = anchorA.parent.GetComponent<RectTransform>();
            var rectParentA_anchoredPosition = rectParentA.anchoredPosition;
            var widthParentA = rectParentA.sizeDelta.x;
            var anchorA_realAnchoredPositionX = rectParentA_anchoredPosition.x + widthParentA * rectParentA.pivot.x;
            var anchorA_realAnchoredPositionY = rectParentA_anchoredPosition.y + rectA_anchoredPosition.y;
            var realAnchoredPositionA = new Vector2(anchorA_realAnchoredPositionX, anchorA_realAnchoredPositionY);

            lineRenderer.Points[0] = realAnchoredPositionA;
        }
        // detect position of B and snag into head of B
        if(headB != null && !headB.Equals(null) && anchorB != null)
        {
            headB.position = anchorB.position;
            // naming of line renderer's Points make me confusion
            // in fact, it is a difference between point of A and one of B
            // because of the first of line's position is always zero.
            // therefore, point of B will be zero adding diff position. 
            // var dist = ((anchorB.parent.position - anchorB.position) 
            //     - (anchorA.parent.position - anchorA.position)).sqrMagnitude;
            var rectB = anchorB.GetComponent<RectTransform>();
            var rectB_anchoredPosition = rectB.anchoredPosition;
            var rectParentB = anchorB.parent.GetComponent<RectTransform>();
            var rectParentB_anchoredPosition = rectParentB.anchoredPosition;
            var widthParentB = rectParentB.sizeDelta.x;
            var anchorB_realAnchoredPositionX = rectParentB_anchoredPosition.x - widthParentB * rectParentB.pivot.x;
            var anchorB_realAnchoredPositionY = rectParentB_anchoredPosition.y + rectB_anchoredPosition.y;
            var realAnchoredPositionB = new Vector2(anchorB_realAnchoredPositionX, anchorB_realAnchoredPositionY);

            // var rectA = anchorA.GetComponent<RectTransform>();
            // var rectA_anchoredPosition = rectA.anchoredPosition;
            // var rectParentA = anchorA.parent.GetComponent<RectTransform>();
            // var rectParentA_anchoredPosition = rectParentA.anchoredPosition;
            // var widthParentA = rectParentA.sizeDelta.x;
            // var anchorA_realAnchoredPositionX = rectParentA_anchoredPosition.x + widthParentA * rectParentA.pivot.x;
            // var anchorA_realAnchoredPositionY = rectParentA_anchoredPosition.y + rectA_anchoredPosition.y;
            // var realAnchoredPositionA = new Vector2(anchorA_realAnchoredPositionX, anchorA_realAnchoredPositionY);

            // var dist = realAnchoredPositionB - realAnchoredPositionA;
            // lineRenderer.Points[1] = dist;
            lineRenderer.Points[1] = realAnchoredPositionB;
        }
        lineRenderer.OnRebuildRequested();
    }

    public void SetEntityA(BlueprintEntity a, Transform anchorA)
    {
        this.a = a;
        this.anchorA = anchorA;
    }

    public void SetEntityB(BlueprintEntity b, Transform anchorB)
    {
        this.b = b;
        this.anchorB = anchorB;
    }
}