//#define DEBUGGING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField] private bool  _lerp;
    [SerializeField] private float _speed = 0.2f;


    [SerializeField] private float maxZ = 110.6f;
    [SerializeField] private float minZ = -126.7f;
    [SerializeField] private float maxX = 60.1f;
    [SerializeField] private float minX = -58.6f;
    
    
   
    void Update()
    {
        if (minZ == 0 && maxZ == 0 && minX == 0 && maxX == 0)
        {
            GameDebugLog.LogError("Camera boundaries is null");
            return;
        }

        if (transform.position.x > maxX)
        {
            Vector3 posXmax = new Vector3(maxX, transform.position.y, transform.position.z);

            if (_lerp)
            {
                transform.position = Vector3.Lerp(transform.position, posXmax, Time.deltaTime * _speed);
            }
            else
            {
                transform.position = posXmax;
            }
        }

        if (transform.position.x < minX)
        {
            Vector3 posXmin = new Vector3(minX, transform.position.y, transform.position.z);

            if (_lerp)
            {
                transform.position = Vector3.Lerp(transform.position, posXmin, Time.deltaTime * _speed);
            }
            else
            {
                transform.position = posXmin;
            }
        }

        if (transform.position.z > maxZ)
        {
            Vector3 posZmin = new Vector3(transform.position.x, transform.position.y, maxZ);

            if (_lerp)
            {
                transform.position = Vector3.Lerp(transform.position, posZmin, Time.deltaTime * _speed);
            }
            else
            {
                transform.position = posZmin;
            }
        }

        if (transform.position.z < minZ)
        {
            Vector3 posZmax = new Vector3(transform.position.x, transform.position.y, minZ);

            if (_lerp)
            {
                transform.position = Vector3.Lerp(transform.position, posZmax, Time.deltaTime * _speed);
            }
            else
            {
                transform.position = posZmax;
            }
        }
    }
#if DEBUGGING
    private void OnDrawGizmos()
    {
        Color color = Color.white;
        float linesHeight = 10f;

        if (minZ != 0 && maxZ != 0 && minX != 0 && maxX != 0)
        {
            Vector3 bottomLeft = new Vector3(minX, linesHeight, minZ);
            Vector3 bottomRight = new Vector3(maxX, linesHeight, minZ);
            Vector3 topLeft = new Vector3(minX, linesHeight, maxZ);
            Vector3 topRight = new Vector3(maxX, linesHeight, maxZ);

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }
#endif
}
