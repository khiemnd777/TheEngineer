using UnityEngine;

public interface IPrefabricated
{
    bool isPrefab { get; set; }
    GameObject Prefabricate(GameObject patternObject, Transform prefabContainer);
    GameObject Unprefabricate(GameObject patternObject);
}