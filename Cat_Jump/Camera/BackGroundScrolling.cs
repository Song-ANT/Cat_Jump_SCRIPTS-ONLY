using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrolling : MonoBehaviour
{
    public float speed;
    public Transform[] background;

    private float bottomPosY = 0;
    private float TopPosY = 0;

    private float xScreenHalfSize;
    private float yScreenHalfSize;

    private void Start()
    {
        yScreenHalfSize = Camera.main.orthographicSize;
        xScreenHalfSize = yScreenHalfSize * Camera.main.aspect;

        bottomPosY = -(yScreenHalfSize * 2);
        TopPosY = yScreenHalfSize * 2 * background.Length;
    }
}
