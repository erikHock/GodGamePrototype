#define DEBUGGING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WeatherSystem : MonoBehaviour
{
    [Header("Rain"),Tooltip("Scale for X and Z")]
    [SerializeField] private float _rainGameobjectScale;
    [SerializeField] private GameObject _rainPrefab;
    private List<RainParticlesHandler> _rainHandler = new List<RainParticlesHandler>();


    [SerializeField] private Volume _skyConfig;
    [SerializeField] private Light _sunLight;
    private HDAdditionalLightData _lightData;

    public WeatherTypes currentWeather;


    private void Awake()
    {
        _lightData = _sunLight.GetComponent<HDAdditionalLightData>();
    }

    private void SpawnRainParticles(BoundariesOfIsland boundaries)
    {
        SpawnRain(boundaries);
    }

    private void OnEnable()
    {
        IslandBoundaries.OnIslandBoundariesGetted += SpawnRainParticles;
    }

    private void OnDisable()
    {
        IslandBoundaries.OnIslandBoundariesGetted -= SpawnRainParticles;
    }

    private void Start()
    {
        UpdateWeatherType(WeatherTypes.Sunny);
    }
    private void Update()
    {
        UpdateWeatherType(currentWeather);
    }
    
    public void UpdateWeatherType(WeatherTypes newWeather)
    {
        currentWeather = newWeather;

        // Cache clouds from shared profile
        var sharedProfile = _skyConfig.sharedProfile;
        sharedProfile.TryGet<VolumetricClouds>(out var clouds);
        sharedProfile.TryGet<Exposure>(out var expo);
        sharedProfile.TryGet<Fog>(out var fog);
        sharedProfile.TryGet<PhysicallyBasedSky>(out var sky);

        switch (newWeather)
        {
            case WeatherTypes.Sunny:
                clouds.cloudPreset.Override(VolumetricClouds.CloudPresets.Sparse);
                expo.fixedExposure.Override(13f);
                fog.enabled.Override(false);
                sky.aerosolDensity.Override(0.3f);
                _sunLight.colorTemperature = 15000f;
                _lightData.intensity = 130000f;
                Raining(false);
                break;

            case WeatherTypes.Rainy:
                clouds.cloudPreset.Override(VolumetricClouds.CloudPresets.Stormy);
                expo.fixedExposure.Override(10.0f);
                fog.enabled.Override(true);
                sky.aerosolDensity.Override(0.500f);
                _sunLight.colorTemperature = 6500f;
                _lightData.intensity = 20000f;
                Raining(true);
                
                break;
        }
    }
    private void SpawnRain(BoundariesOfIsland boundaries)
    {
        if(boundaries.isNull())
        {
            return;
        }
        else
        {
            var minX = Mathf.RoundToInt(boundaries.minX);
            var maxX = Mathf.RoundToInt(boundaries.maxX);
            var minZ = Mathf.RoundToInt(boundaries.minZ);
            var maxZ = Mathf.RoundToInt(boundaries.maxZ);

            int sizeOfSpawnedObject = Mathf.RoundToInt(50f);
            Vector3 spawnPos = Vector3.zero;
            float heightOfSpawnPos = 10f;


            int gridHeight = (maxZ - minZ);
            int gridWidth = (maxX - minX);

            int gridHeightHalf = gridHeight / 2;
            int gridWidthHalf = gridWidth / 2;

            GameObject weatherGridParent = new GameObject("WeatherGridParent");
            weatherGridParent.transform.parent = null;
            weatherGridParent.transform.position = Vector3.zero;

            // If grid XZ is less than object scale
            if (gridWidth < sizeOfSpawnedObject && gridHeight < sizeOfSpawnedObject)
            {
                GameDebugLog.LogMessage("Grid size is less than size of object");

                // Set vector to center of grid
                spawnPos = new Vector3((minX + gridWidthHalf), heightOfSpawnPos, (minZ + gridHeightHalf));
                GameObject rainPrefab = Instantiate(_rainPrefab, spawnPos, Quaternion.identity);
                _rainHandler.Add(rainPrefab.GetComponent<RainParticlesHandler>());
                rainPrefab.transform.parent = weatherGridParent.transform;
            }
            else
            {
                GameDebugLog.LogMessage("Weather grid for x and z created");

                for (int x = minX ; x <= maxX; x += sizeOfSpawnedObject)
                {
                    for (int z = minZ; z <= maxZ; z += sizeOfSpawnedObject)
                    {
                        spawnPos = new Vector3(x, heightOfSpawnPos, z);
                        GameObject rainPrefab = Instantiate(_rainPrefab, spawnPos, Quaternion.identity);
                        _rainHandler.Add(rainPrefab.GetComponent<RainParticlesHandler>());
                        rainPrefab.transform.parent = weatherGridParent.transform;
                    }
                }
            }
            


        }
    }
    private void Raining(bool isRaining)
    {
        if (_rainHandler.Count.Equals(0))
        {
            return;
        }

        foreach (RainParticlesHandler r in _rainHandler)
        {
            if (isRaining)
            {
                r.StartParticles();
            }
            else
            {
                r.StopParticles();
            }
        }
    }

}

public enum WeatherTypes
{
    Sunny,
    Rainy
}

