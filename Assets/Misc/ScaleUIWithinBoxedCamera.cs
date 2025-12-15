using UnityEngine;

// keeps UI elements in an overlay canvas withing the Camera's viewport when the camera is windowboxed/letterboxed/etc
// I often use this with a windowboxed pixel perfect camera
[RequireComponent(typeof(RectTransform))]
public class ScaleUIWithinBoxedCamera : MonoBehaviour
{
    public Canvas overlayCanvas;
    public RectTransform rectToScale; // the immediate parent of the overlay canvas
    public Camera boxedCamera;

    Vector2 canvasSizeLastFrame;
    Vector2 CanvasSize { get => (overlayCanvas.transform as RectTransform).sizeDelta; }

    void Start()
    {
        canvasSizeLastFrame = CanvasSize;
        ScaleUI();
    }

    void ScaleUI()
    {
        // center
        rectToScale.anchorMax = Vector2.one/2f;
        rectToScale.anchorMin = Vector2.one/2f;
        rectToScale.anchoredPosition = Vector2.zero;

        // scale
        rectToScale.sizeDelta = CanvasSize * boxedCamera.rect.size;
    }

    [ExecuteInEditMode]
    void Update()
    {
        if (CanvasSize != canvasSizeLastFrame)
        {
            ScaleUI();
            canvasSizeLastFrame = CanvasSize;
        }
    }
}
