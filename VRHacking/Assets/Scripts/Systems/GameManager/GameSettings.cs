using System;
using UnityEngine;

public static class GameSettings
{
    private static GameSettingsData currentGameData;
    private static RunData currentRunData;
    private const string constSettingsPath = "ScriptableObjects/Settings_";

    public static void InitializeData(GameSettingsData.GameMode mode) {
        string path = constSettingsPath + Enum.GetName(typeof(GameSettingsData.GameMode), mode);
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

public class RunData {
    public int hackersDefeated;
    public int tasksCompleted;
    public int bugsUploaded;
    public int bugsReceived;
    public float averageTaskCompletionTime;
}