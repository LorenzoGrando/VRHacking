using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TaskManager taskManager;

    void Start()
    {
        GameSettings.InitializeData();
        taskManager.BeginTaskSequence(3, GameSettings.GetGameData());
        Debug.Log("Called game");
    }
}
