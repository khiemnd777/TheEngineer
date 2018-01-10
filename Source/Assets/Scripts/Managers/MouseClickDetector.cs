using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void OnClick();

public class MouseClickDetector : MonoBehaviour
{
    #region Singleton
    static MouseClickDetector _instance;

    public static MouseClickDetector instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MouseClickDetector>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active MouseClickDetector script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    public float doubleClickTimeLimit = 0.25f;

    public OnClick onDoubleClick;
    public OnClick onSingleClick;

    protected void Start()
    {
        StartCoroutine(InputListener());
    }

    // Update is called once per frame
    private IEnumerator InputListener()
    {
        while (enabled)
        { //Run as long as this is activ

            if (Input.GetMouseButtonDown(0))
                yield return ClickEvent();

            yield return null;
        }
    }

    private IEnumerator ClickEvent()
    {
        //pause a frame so you don't pick up the same mouse down event.
        yield return new WaitForEndOfFrame();

        float count = 0f;
        while (count < doubleClickTimeLimit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoubleClick();
                yield break;
            }
            count += Time.deltaTime;// increment counter by change in time between frames
            yield return null; // wait for the next frame
        }
        SingleClick();
    }


    private void SingleClick()
    {
        if(onSingleClick.IsNotNull()){
            onSingleClick.Invoke();
        }
    }

    private void DoubleClick()
    {
        if(onDoubleClick.IsNotNull()){
            onDoubleClick.Invoke();
        }
    }
}