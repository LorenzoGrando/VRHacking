using System;
using UnityEngine;

public static class GameSettings
{
    public struct GameSettingsData {
        [Range(1,2)]
        public float difficulty;
        public int level;
    }

    private static GameSettingsData currentGameData;

    public static void InitializeData() {
        currentGameData.difficulty = 1;
        currentGameData.level = 0;
    }

    public static GameSettingsData GetGameData() {
        return currentGameData;
    }

    public static void SetGameData(float difficultyValue) {
        currentGameData.difficulty = Mathf.Clamp(difficultyValue, 1, 2);
    }
}