using System;
using System.Collections.Generic;
using UnityEngine;

public class TimePollingModlet : IModApi
{
    private const float PollingIntervalSeconds = 5f; // Poll every 5 seconds
    private float _nextPollTime;

    public void InitMod(Mod _modInstance)
    {
        ModEvents.GameUpdate.RegisterHandler(GameUpdateHandler);
        Debug.Log("Time Polling Modlet initialized");
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

        int hours = (int)Math.Floor(totalHours);

        int minutes = (int)Math.Floor(totalMinutes);

        Debug.Log($"Current game time: Day {days}, {hours:D2}:{minutes:D2}");

        // Add your custom logic here based on the game time
        // For example:
        if (hours >= 22 || hours < 4)
        {
            Debug.Log("It's nighttime!");
        }
        else
        {
            Debug.Log("It's daytime!");
        }
    }
}