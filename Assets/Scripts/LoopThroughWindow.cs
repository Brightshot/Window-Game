using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopThroughWindow : MonoBehaviour
{
    private Vector2 ScreenResolution => new Vector2
    {
        x = Screen.currentResolution.width,
        y = Screen.currentResolution.height
    };

    private Camera MainCamera;

    private void Start()
    {
        MainCamera = Camera.main;
    }

    void Update()
    {
        var screenPosition = MainCamera.WorldToScreenPoint(transform.position);

        if (screenPosition.y < 0)
        {
            transform.position = MainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, 1080,10));
        }

        if (screenPosition.x > ScreenResolution.x)
        {
            transform.position = MainCamera.ScreenToWorldPoint(new Vector3(0, screenPosition.y, 10));
        }

        if (screenPosition.x < 0)
        {
            transform.position = MainCamera.ScreenToWorldPoint(new Vector3(ScreenResolution.x, screenPosition.y, 10));
        }
    }
}
