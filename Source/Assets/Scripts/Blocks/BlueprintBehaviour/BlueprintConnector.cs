using UnityEngine;

public class BlueprintConnector : MonoBehaviour
{
    public static BlueprintConnector current;

    public RectTransform headA;
    public RectTransform headB;

    [System.NonSerialized]
    public BlueprintEntity a;
    [System.NonSerialized]
    public BlueprintEntity b;

    public Transform anchorA;
    public Transform anchorB;

    void Update()
    {
        
        // detect position of A and snag into head of A
        if(headA != null && !headA.Equals(null) && anchorA != null)
        {
            headA.position = anchorA.position;
        }
        // detect position of B and snag into head of B
        if(headB != null && !headB.Equals(null) && anchorB != null)
        {
            headB.position = anchorB.position;
        }
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