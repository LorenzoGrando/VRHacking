using System;
using UnityEngine;
using System.IO;

public static class GameSettings
{
    private static GameSettingsData currentGameData;
    private static RunData currentRunData;
    public static RunData CurrentRunData {
        get {
            return currentRunData;
        }
        set {
            currentRunData = value;
        }
    }
    private const string constSettingsPath = "ScriptableObjects/Settings_";
    private static string saveDirectory = Application.persistentDataPath + "/SavedData";
    private static string savedRunName = "/BestRunData";

    public static void InitializeData(GameSettingsData.GameMode mode) {
        string path = constSettingsPath + Enum.GetName(typeof(GameSettingsData.GameMode), mode);
        currentGameData = Resources.Load<GameSettingsData>(path);

        currentGameData.difficulty = 1;
        currentGameData.level = 0;
        currentGameData.defeatedHackers = 0;
        
        if(currentRunData == null)
            currentRunData = new RunData();
        currentRunData.ResetData();
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
    
    public static void TryUpdateBestRun(RunData newRun)
    {
        RunData existingData = TryGetBestRunData();
        if (existingData == null)
        {
            currentRunData = newRun;
            SaveRunData();
        }    
        else
        {
            //Compare runs by priority
            currentRunData = CompareBestPerformingRun(existingData, newRun);
            SaveRunData();
        }
    }

    private static RunData CompareBestPerformingRun(RunData currentData, RunData newData)
    {
        if (newData.hackersDefeated > currentData.hackersDefeated)
            return newData;
        else if (newData.hackersDefeated < currentData.hackersDefeated)
            return currentData;
        else
        {
            if (newData.tasksCompleted > currentData.tasksCompleted)
                return newData;
            else if (newData.tasksCompleted < currentData.tasksCompleted)
                return currentData;
            else
            {
                if (newData.averageTaskCompletionTime < currentData.averageTaskCompletionTime)
                    return newData;
                else if (newData.averageTaskCompletionTime > currentData.averageTaskCompletionTime)
                    return currentData;
                else
                {
                    if (newData.bugsUploaded > currentData.bugsUploaded)
                        return newData;
                    else if (newData.bugsUploaded < currentData.bugsUploaded)
                        return currentData;
                    else
                    {
                        if (newData.bugsReceived < currentData.bugsReceived)
                            return newData;
                        else if (newData.bugsReceived > currentData.bugsReceived)
                            return currentData;
                        else
                        {
                            return currentData;
                        }
                    }
                }
            }
        }
    }

    private static void SaveRunData()
    {
        CreateRunDataDirectory();
        if (File.Exists(saveDirectory + savedRunName))
        {
            File.Delete(saveDirectory + savedRunName);
        }
        
        string json = JsonUtility.ToJson(currentRunData, true);
        File.WriteAllText(saveDirectory + savedRunName, json);
    }

    public static RunData TryGetBestRunData()
    {
        RunData data = null;
        if(System.IO.Directory.Exists(saveDirectory))
        {
            if (System.IO.File.Exists((saveDirectory + savedRunName)))
            {
                string json = File.ReadAllText(saveDirectory + savedRunName);
                data = JsonUtility.FromJson<RunData>(json);
            }
        }

        return data;
    }

    private static void CreateRunDataDirectory()
    {
        if (!System.IO.Directory.Exists(saveDirectory))
        {
            System.IO.Directory.CreateDirectory(saveDirectory);
        }
    }
}

[System.Serializable]
public class RunData {
    public int hackersDefeated;
    public int tasksCompleted;
    public int bugsUploaded;
    public int bugsReceived;
    public float averageTaskCompletionTime;

    public void ResetData()
    {
        hackersDefeated = 0;
        tasksCompleted = 0;
        bugsUploaded = 0;
        bugsReceived = 0;
        averageTaskCompletionTime = 999;
    }
}