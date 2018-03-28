using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintConnectorRemoveButton : MonoBehaviour
    , IPointerUpHandler
    , IPointerDownHandler
{
    [System.NonSerialized]
    public BlueprintConnector connector;

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        connector.Remove();
    }
}