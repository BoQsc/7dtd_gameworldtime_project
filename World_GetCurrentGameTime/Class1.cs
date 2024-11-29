using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public class TimePollingModlet : IModApi
{
    private const float PollingIntervalSeconds = 1f; // Poll every 1 second
    private float _nextPollTime;
    private const string JsonFileName = "current_game_time.json";
    private string _jsonFilePath;

    public void InitMod(Mod _modInstance)
    {
        ModEvents.GameUpdate.RegisterHandler(GameUpdateHandler);
        SetJsonFilePath();
        Debug.Log($"Time Polling Modlet initialized. JSON file path: {_jsonFilePath}");
    }

    private void SetJsonFilePath()
    {
        string dllPath = Assembly.GetExecutingAssembly().Location;
        string dllDirectory = Path.GetDirectoryName(dllPath);
        _jsonFilePath = Path.Combine(dllDirectory, JsonFileName);
    }

    private void GameUpdateHandler()
    {
        if (Time.time >= _nextPollTime)
        {
            PollGameTime();
            _nextPollTime = Time.time + PollingIntervalSeconds;
        }
    }

    private void PollGameTime()
    {
        if (GameManager.Instance?.World == null)
        {
            return;
        }

        ulong worldTime = GameManager.Instance.World.worldTime;
        int days = GameUtils.WorldTimeToDays(worldTime);
        float totalHours = GameUtils.WorldTimeToHours(worldTime);
        float totalMinutes = GameUtils.WorldTimeToMinutes(worldTime);
        int hours = (int)Math.Floor(totalHours) % 24;
        int minutes = (int)Math.Floor(totalMinutes) % 60;

        string jsonContent = $"{{\"Current game time\": \"Day {days}, {hours:D2}:{minutes:D2}\"}}";

        try
        {
            File.WriteAllText(_jsonFilePath, jsonContent);
            //Debug.Log($"Game time saved to {_jsonFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing to JSON file: {ex.Message}");
        }
    }
}