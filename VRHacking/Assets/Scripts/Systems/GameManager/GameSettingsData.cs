using System;
using UnityEngine;

[CreateAssetMenu(menuName = "VRHacking/GameSettingsData")]
public class GameSettingsData : ScriptableObject
{
    public enum GameMode {
        Null, Campaign, Endless
    }
    public GameMode thisGameMode;
    [Range(1,2)]
    public float difficulty;
    public int level;



        //Endless Mode Values
    [Header("Endless Mode")]
    public int defeatedHackers;
    public int maximumDifficultyByHackerAmount;
    public float newInvasionMaxIntervalTime;
}