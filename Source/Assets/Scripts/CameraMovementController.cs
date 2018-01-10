using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovementController : MonoBehaviour
{
    public float defaultOrthographicSize = 9f;
    public float orthographicSizeMin = 5f;
    public float orthographicSizeMax = 20f;
    [Space]
    public float zoomValue = 0.525f;
    public float normalizedZoomSpeed = 10f;

    Vector2 _anchorPointToMove;

    void Start()
    {
        Camera.main.orthographicSize = Mathf.Clamp(defaultOrthographicSize, orthographicSizeMin, orthographicSizeMax);
        MouseEventDetector.instance.onDoubleClick += () => {
            NormalizeZoom();
        };
    }

    void Update()
    {
        Zoom();
        Move();
    }

    public void ZoomOnPixel(Pixel pixel)
    {
        NormalizeCamera(pixel.transform.position);
    }

    public void NormalizeCamera(Vector2 position)
    {
        StartCoroutine(NormalizeCameraCoroutine(position));
    }

    IEnumerator NormalizeCameraCoroutine(Vector2 position)
    {
        var percent = 0f;
        var orginalSize = Camera.main.orthographicSize;
        var orginalPosition = Camera.main.transform.position;

        while (percent <= 1f)
        {
            percent += Time.deltaTime * normalizedZoomSpeed;

            Camera.main.orthographicSize = Mathf.Lerp(orginalSize, defaultOrthographicSize, percent);

            var camPosition = Camera.main.transform.position;
            var lerpCamPosition = Vector2.Lerp(orginalPosition, position, percent);
            camPosition.x = lerpCamPosition.x;
            camPosition.y = lerpCamPosition.y;
            Camera.main.transform.position = camPosition;

            yield return null;
        }
    }

    void NormalizeZoom()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;
        var mousePosition = Input.mousePosition;
        var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        NormalizeCamera(worldMousePosition);
    }

    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.orthographicSize >= orthographicSizeMax)
                return;
            Camera.main.orthographicSize += zoomValue;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.orthographicSize <= orthographicSizeMin)
                return;
            Camera.main.orthographicSize -= zoomValue;
        }
    }

    void Move()
    {
        if (Input.GetMouseButtonDown(2))
        {
            // press down middle mouse
            AnchorMovePoint();
        }
        if (Input.GetMouseButton(2))
        {
            // pressed and hold middle mouse
            MoveCamera();
        }
    }

    void AnchorMovePoint()
    {
        var mousePosition = Input.mousePosition;
        var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        _anchorPointToMove = worldMousePosition;
    }

    void MoveCamera()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            // move if mouse has any movement
            var mousePosition = Input.mousePosition;
            var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var camPosition = Camera.main.transform.position;
            camPosition.x -= worldMousePosition.x - _anchorPointToMove.x;
            camPosition.y -= worldMousePosition.y - _anchorPointToMove.y;
            Camera.main.transform.position = camPosition;
        }
    }
}
