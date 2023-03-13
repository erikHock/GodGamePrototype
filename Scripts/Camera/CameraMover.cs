using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraSystem))]
public class CameraMover : MonoBehaviour
{
    private CameraSystem _cameraSystem;

    private void Awake()
    {
        _cameraSystem = GetComponent<CameraSystem>();
    }
    private void OnEnable()
    {
        IslandBoundaries.OnIslandBoundariesGetted += IslandBoundaries_OnIslandBoundariesGetted;
    }
    private void OnDisable()
    {
        IslandBoundaries.OnIslandBoundariesGetted -= IslandBoundaries_OnIslandBoundariesGetted;
    }

    private void IslandBoundaries_OnIslandBoundariesGetted(BoundariesOfIsland boundaries)
    {
        float gridHeight = (boundaries.maxZ - boundaries.minZ) / 2f;
        float gridWidth = (boundaries.maxX - boundaries.minX) / 2f;

        // Get center of boundaries
        Vector3 center = new Vector3(boundaries.minX + gridWidth, transform.position.y, boundaries.minZ + gridHeight);
        
        // Move to center
        StartCoroutine(MoveCameraXZ(center, 1f));

        // Zoom out
        StartCoroutine(ForwardZoomOut(new Vector3(_cameraSystem.GetFollowOffset().x, 394.92f, -276.45f),1f));
      
    }

    private IEnumerator MoveCameraXZ(Vector3 newPos, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, (elapsedTime / duration));
           elapsedTime += Time.deltaTime;

           yield return null;
        }
        transform.position = newPos;
        yield break;

    }

    public IEnumerator ForwardZoomOut(Vector3 newFollowOffset, float duration)
    {
        _cameraSystem.SetIsZoomActive(false);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _cameraSystem.GetVirtualCamera().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(_cameraSystem.GetVirtualCamera().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, newFollowOffset, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        _cameraSystem.GetVirtualCamera().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = newFollowOffset;
        

        _cameraSystem.SetIsZoomActive(true);
        yield break;
    }



}
