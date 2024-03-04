using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TaskManager taskManager;
    [SerializeField]
    private HackerManager hackerManager;

    void Start()
    {
        GameSettings.InitializeData();
        GameSettings.GameSettingsData gameSettingsData = GameSettings.GetGameData();


        taskManager.BeginTaskSequence(3, gameSettingsData);

        hackerManager.InitializeHackerData(gameSettingsData);
        hackerManager.BeginHackerSequence();
        
        Debug.Log("Called game");
    }
}
