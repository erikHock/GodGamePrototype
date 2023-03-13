using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameStates currentState;
    public static event Action<GameStates> OnGameStateChanged;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameStates.CreatingIsland);
    }
    private void Update()
    {
        UpdateGameState(currentState);
    }

    public void UpdateGameState(GameStates newState)
    {
        currentState = newState;

        switch(newState)
        {
            case GameStates.CreatingIsland: 
                break;

            case GameStates.GameLoop:
                break;

            case GameStates.ChangingWeather: 
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }
}
public enum GameStates 
{
    CreatingIsland, 
    ChangingWeather,
    GameLoop
}
