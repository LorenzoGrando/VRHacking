using System;
using UnityEngine;

public static class GameSettings
{
    private static GameSettingsData currentGameData;
    private const string constPath = "ScriptableObjects/Settings_";

    public static void InitializeData(GameSettingsData.GameMode mode) {
        string path = constPath + Enum.GetName(typeof(GameSettingsData.GameMode), mode);
        currentGameData = Resources.Load<GameSettingsData>(path);

        currentGameData.difficulty = 1;
        currentGameData.level = 0;
        currentGameData.defeatedHackers = 0;
    }

    public static GameSettingsData GetGameData() {
        return currentGameData;
    }

    public static GameSettingsData SetGetGameData(GameSettingsData gameSettingsData) {
        currentGameData= gameSettingsData;

        if(currentGameData.thisGameMode == GameSettingsData.GameMode.Endless) {
            //Recalculate Difficulty
            currentGameData.difficulty = Mathf.Lerp(1, 2, Mathf.Clamp(currentGameData.defeatedHackers, 0, currentGameData.maximumDifficultyByHackerAmount)/currentGameData.maximumDifficultyByHackerAmount);
        }

        return currentGameData;
    }
}