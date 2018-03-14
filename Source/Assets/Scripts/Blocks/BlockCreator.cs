using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockCreator : MonoBehaviour
{
    public ChoiceBlockUI choiceBlockUIPrefab;
    public Canvas canvas;

    [Header("Block Type")]
    public Block16x16 block16x16Prefab;
    public CustomBlock16x16 customBlock16x16Prefab;
    [Space]
    public Block16x32 block16x32Prefab;
    public CustomBlock16x32 customBlock16x32Prefab;

    public List<Block> availableBlocks = new List<Block>();

    ChoiceBlockUI _choiceBlockUI;

    public void OpenChoiceBlockUI()
    {
        var instance = Instantiate(choiceBlockUIPrefab, Vector3.one, Quaternion.identity);
        var instanceTransform = instance.transform;
        instanceTransform.SetParent(canvas.transform);
        instanceTransform.position = Vector3.zero;
        instanceTransform.localScale = Vector3.one;
        // register button click event for 16x16
        var chooseBlock16x16BtnEvent = instance.chooseBlock16x16Btn.onClick;
        chooseBlock16x16BtnEvent.RemoveListener(CreateBlock16x16);
        chooseBlock16x16BtnEvent.AddListener(CreateBlock16x16);
        // register button click event for 16x32
        var chooseBlock16x32BtnEvent = instance.chooseBlock16x32Btn.onClick;
        chooseBlock16x32BtnEvent.RemoveListener(CreateBlock16x32);
        chooseBlock16x32BtnEvent.AddListener(CreateBlock16x32);
        _choiceBlockUI = instance;
        instance = null;
    }

    public void CreateBlock16x16()
    {
        _choiceBlockUI.gameObject.SetActive(false);
        var customBlockInstance = Instantiate(customBlock16x16Prefab, Vector3.one, Quaternion.identity);
		var surfaceTextureMaker = customBlockInstance.surfaceTexture.maker;
		var standardTextureMaker = customBlockInstance.standardTexture.maker;
        surfaceTextureMaker.CreateTexture(16, 16);
		standardTextureMaker.CreateTexture(16, 16);
        customBlockInstance.SetOKCallback(textures =>
        {
            var instance = Instantiate<Block16x16>(block16x16Prefab, Vector3.one, Quaternion.identity);
            availableBlocks.Add(instance);
            instance.SetTextures(textures);
            Destroy(_choiceBlockUI.gameObject);
        });
        customBlockInstance.SetCancelCallback(() =>
        {
            Destroy(_choiceBlockUI.gameObject);
        });
        var instanceTransform = customBlockInstance.transform;
        instanceTransform.SetParent(canvas.transform);
        instanceTransform.position = Vector3.zero;
        instanceTransform.localScale = Vector3.one;
    }

    public void CreateBlock16x32()
    {
        _choiceBlockUI.gameObject.SetActive(false);
        var customBlockInstance = Instantiate(customBlock16x32Prefab, Vector3.one, Quaternion.identity);
        var textureMakerAction = customBlockInstance.texture;
        var textureMaker = textureMakerAction.maker;
        textureMaker.CreateTexture(16, 32);
        customBlockInstance.SetOKCallback(textures =>
        {
            var instance = Instantiate<Block16x32>(block16x32Prefab, Vector3.one, Quaternion.identity);
            availableBlocks.Add(instance);
            instance.SetTextures(textures);
            Destroy(_choiceBlockUI.gameObject);
        });
        customBlockInstance.SetCancelCallback(() =>
        {
            Destroy(_choiceBlockUI.gameObject);
        });
        var instanceTransform = customBlockInstance.transform;
        instanceTransform.SetParent(canvas.transform);
        instanceTransform.position = Vector3.zero;
        instanceTransform.localScale = Vector3.one;
    }
}