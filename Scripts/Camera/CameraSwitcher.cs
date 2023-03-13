using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] _cameras;

    [SerializeField] private CinemachineVirtualCamera _activeCamera;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameStates state)
    {
        if(state == GameStates.ChangingWeather)
        {
            ActivateWeatherCamera();
        }
        else 
        {
            ActivateIslandCamera();
        }
    }

   
    private void ActivateWeatherCamera()
    {
        foreach (CinemachineVirtualCamera cam in _cameras)
        {
            if (cam.CompareTag("WeatherCamera") && cam != _activeCamera)
            {
                _activeCamera = cam;
                _activeCamera.Priority = 20;
            }
            else
            {
                _activeCamera = null;
                cam.Priority = 0;
            }
        }

    }

    private void ActivateIslandCamera()
    {
        foreach (CinemachineVirtualCamera cam in _cameras)
        {
            if (cam.CompareTag("IslandCamera") && cam != _activeCamera)
            {
                _activeCamera = cam;
                _activeCamera.Priority = 20;
            }
            else
            {
                _activeCamera = null;
                cam.Priority = 0;
            }
        }
    }
}
