using System;
using UnityEngine;

public static class GameSettings
{
    private static GameSettingsData currentGameData;

    public static void InitializeData() {
        if(currentGameData == null)
            currentGameData = Resources.Load<GameSettingsData>("ScriptableObjects/Settings_Endless");

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