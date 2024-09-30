using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Ground_Canvas : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(2340, 1080); // 레퍼런스 해상도
    public Vector2 scaleFactor = new Vector2(1, 1); // 캔버스 스케일 팩터

    private Camera _camera;
    private Canvas _canvas;
    private RectTransform _rt;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _rt = GetComponent<RectTransform>();

        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    void Start()
    {
        AdjustCanvasSize();
    }

    void AdjustCanvasSize()
    {
        float cameraHeight = 2f * _camera.orthographicSize;
        float cameraWidth = cameraHeight * _camera.aspect;

        float referenceAspect = referenceResolution.x / referenceResolution.y;
        float currentAspect = cameraWidth / cameraHeight;

        Vector2 scale = Vector2.one;

        if (currentAspect >= referenceAspect)
        {
            scale.x = scaleFactor.x * (cameraHeight * referenceAspect / referenceResolution.x);
            scale.y = scaleFactor.y * (cameraHeight / referenceResolution.y)*2;
        }
        else
        {
            scale.x = scaleFactor.x * (cameraWidth / referenceResolution.x);
            scale.y = scaleFactor.y * (cameraWidth / referenceResolution.y / referenceAspect)*2;
        }

        _rt.sizeDelta = referenceResolution;
        _rt.localScale = new Vector3(scale.x, scale.y, 1);
    }


    private void Update()
    {
        AdjustCanvasSize ();
    }

}
