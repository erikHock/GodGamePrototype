using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaterLoD : MonoBehaviour
{
    [SerializeField] private Transform _detailedWater;
    [SerializeField] private Transform _planeWater;

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
            StartCoroutine(EnableByDelay(1f));
        }
        else 
        {
            _planeWater.gameObject.SetActive(false);
            _detailedWater.gameObject.SetActive(true);
        }
       
    }

    IEnumerator EnableByDelay(float time)
    {
        yield return new WaitForSeconds(time);

        _detailedWater.gameObject.SetActive(false);
        _planeWater.gameObject.SetActive(true);
    }
}
