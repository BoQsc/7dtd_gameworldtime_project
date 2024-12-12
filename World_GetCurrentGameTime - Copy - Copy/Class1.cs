using UnityEngine;
using System;

public class NighttimeTimeScaleModlet : IModApi
{
    private const float DaytimeTimeScale = 1.0f;
    private const float NighttimeTimeScale = 0.1f;

    private float currentTimeScale = DaytimeTimeScale;
    private float delayTimer = 0f; // Timer for delay

    public void InitMod(Mod modInstance)
    {
        ModEvents.GameUpdate.RegisterHandler(GameUpdateHandler);
        Debug.Log("Nighttime Time Scale Modlet initialized");
    }

    private void GameUpdateHandler()
    {
        // Update timer
        delayTimer += Time.deltaTime;

        if (delayTimer >= 2f) // Check if 2 seconds have passed
        {
            delayTimer = 0f; // Reset timer

            bool isNighttime = IsNighttime();
            float desiredTimeScale = isNighttime ? NighttimeTimeScale : DaytimeTimeScale;

            //if (desiredTimeScale != Time.timeScale)
            //{
            //    Time.timeScale = desiredTimeScale;
            //    currentTimeScale = desiredTimeScale;
            //    Debug.Log($"Time scale set to {desiredTimeScale}");
            //}
            //else
            //{
            //    Debug.Log("Time scale remains at " + currentTimeScale);
            //}

            // Check for night start and end
            if (isNighttime)
            {
                ExecuteTimeOfDayIncPerSec(60);
                Debug.Log("Night started. Time of day increment per second set to 60.");
            }
            else
            {
                ExecuteTimeOfDayIncPerSec(6);
                Debug.Log("Night ended. Time of day increment per second set to 6.");
            }
        }
    }

    private bool IsNighttime()
    {
        if (GameManager.Instance != null && GameManager.Instance.World != null)
        {
            return !GameManager.Instance.World.IsDaytime();
        }
        else
        {
            Debug.LogWarning("GameManager or World is null. Defaulting to daytime.");
            return false;
        }
    }

    private void ExecuteTimeOfDayIncPerSec(int value)
    {
        EnumGameStats stats = EnumGameStats.TimeOfDayIncPerSec;
        object obj2 = GameStats.Parse(stats, value.ToString());
        GameStats.SetObject(stats, obj2);
        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(stats.ToStringCached<EnumGameStats>() + " set to " + obj2?.ToString());
    }
}