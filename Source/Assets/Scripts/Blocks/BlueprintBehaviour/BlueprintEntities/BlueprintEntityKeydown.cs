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

        input.removeConnectorCallback = () => {
            ((EntityKeydown)behaviourEntity).input = null;
        };

        output.removeConnectorCallback = () => {
            ((EntityKeydown)behaviourEntity).output = null;
        };
    }

    public override void Update()
    {
        base.Update();
        var textLen = keyName.text.Length;
        if(textLen > 1){
            var newText = keyName.text.Substring(textLen - 1, textLen - 1);
            keyName.text = newText;
        }
        keyName.text = keyName.text.ToUpperInvariant();
    }

    public override void Remove()
    {
        if(input.blueprintConnector != null && !input.blueprintConnector.Equals(null))
            input.blueprintConnector.Remove();
        if(output.blueprintConnector != null && !output.blueprintConnector.Equals(null))
            output.blueprintConnector.Remove();
        base.Remove();
    }
}