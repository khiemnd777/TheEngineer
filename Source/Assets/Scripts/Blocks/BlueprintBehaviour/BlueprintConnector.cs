using UnityEngine;

public class BlueprintConnector : MonoBehaviour
{
    public static BlueprintConnector current;

    public BlueprintEntity a;
    public BlueprintEntity b;

    public void SetEntityA(BlueprintEntity a)
    {
        this.a = a;
    }

    public void SetEntityB(BlueprintEntity b)
    {
        this.b = b;
    }
}