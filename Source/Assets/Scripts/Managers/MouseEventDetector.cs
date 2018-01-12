using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void OnClick(Vector2 position);

public class MouseEventDetector : MonoBehaviour
{
    #region Singleton
    static MouseEventDetector _instance;

    public static MouseEventDetector instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MouseEventDetector>();
                if (!_instance)
                {
                    Debug.LogError("There needs to be one active MouseEventDetector script on a GameObject in your scene.");
                }
                else
                {

                }
            }
            return _instance;
        }
    }
    #endregion

    public float doubleClickThreshold = 0.1925f;
    public float singleMouseUpThreshold = 0.15f;

    public OnClick onDoubleClick;
    public OnClick onSingleClick;
    public OnClick onDoubleMouseUp;
    public OnClick onSingleMouseUp;

    bool singleMouseUpIsFired;

    protected void Start()
    {
        StartCoroutine(InputListener());
    }

    // Update is called once per frame
    IEnumerator InputListener()
    {
        while (enabled)
        { //Run as long as this is activ

            if (Input.GetMouseButtonDown(0)){
                StartCoroutine(MouseUpEvent(() => {
                    singleMouseUpIsFired = true;
                }, singleMouseUpThreshold));
                yield return ClickEvent(Input.mousePosition);
            }
            yield return null;
        }
    }

    IEnumerator ClickEvent(Vector2 position)
    {
        //pause a frame so you don't pick up the same mouse down event.
        yield return new WaitForSeconds(Time.deltaTime);
        var count = 0f;
        while (count < doubleClickThreshold)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoubleClick(position);
                yield return MouseUpEvent(() => {
                    DoubleMouseUp(position);
                }, doubleClickThreshold);
                yield break;
            }
            count += Time.deltaTime;// increment counter by change in time between frames
            yield return null; // wait for the next frame
        }
        SingleClick(position);
    }

    IEnumerator MouseUpEvent(System.Action action, float threshold)
    {
        var count = 0f;
        while(count < threshold){
            if(Input.GetMouseButtonUp(0)){
                if(action.IsNotNull())
                    action.Invoke();
                yield break;
            }
            count += Time.deltaTime;
            yield return null;
        }
    }

    void SingleClick(Vector2 position)
    {
        if (onSingleClick.IsNotNull())
        {
            onSingleClick.Invoke(position);
        }
        if(singleMouseUpIsFired){
            singleMouseUpIsFired = false;
            if(onSingleMouseUp.IsNotNull())
                onSingleMouseUp.Invoke(position);
        }
    }

    void DoubleClick(Vector2 position)
    {
        singleMouseUpIsFired = false;
        if (onDoubleClick.IsNotNull())
        {
            onDoubleClick.Invoke(position);
        }
    }

    void DoubleMouseUp(Vector2 position)
    {
        if (onDoubleMouseUp.IsNotNull())
        {
            onDoubleMouseUp.Invoke(position);
        }
    }
}