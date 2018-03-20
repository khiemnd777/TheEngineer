using UnityEngine;
using UnityEngine.UI;

public class BlueprintEntityKeydown : BlueprintEntity
{
    public InputField keyName;
    public BlueprintEntityPin input;
    public BlueprintEntityPin output;

    public override void Start()
    {
        base.Start();
        input.entity = this;
        output.entity = this;

        behaviourEntity = new EntityKeydown();

        input.dropToConnectorCallback = blockConnector => {
            ((EntityKeydown)behaviourEntity).input = blockConnector;
        };

        output.dropToConnectorCallback = blockConnector => {
            ((EntityKeydown)behaviourEntity).output = blockConnector;
        };
    }

    void Update()
    {
        var textLen = keyName.text.Length;
        if(textLen > 1){
            var newText = keyName.text.Substring(textLen - 1, textLen - 1);
            keyName.text = newText;
        }
        keyName.text = keyName.text.ToUpperInvariant();
    }
}