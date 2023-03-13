#define DEBUGGING
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycaster : MonoBehaviour
{
    public static MouseRaycaster Instance;
    public RaycastHit hit { get; private set; }
    public bool isHit { get; private set; }

    public Camera screenCamera;

    private void Awake()
    {
        Instance = this;

#if DEBUGGING
        if (screenCamera == null)
        {
            GameDebugLog.LogMessage("Assign screen camera");
        }
#endif

    }

    void Update()
    {
        if (screenCamera != null)
        {
            RaycastFromScreenToWorld();
        }
    }

    private void RaycastFromScreenToWorld()
    {
        Ray ray = screenCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            isHit = true;
            this.hit = hit;
        }
        else
        {
            isHit= false;
        }
       

       
    }

    public Camera GetCamera()
    {
        return screenCamera;
    }
}
