using UnityEngine;
using System;

public class NighttimeTimeScaleModlet : IModApi
{
    private const float DaytimeTimeScale = 1.0f;
    private const float NighttimeTimeScale = 0.1f;
    private const int NightStartHour = 18; // 6 PM
    private const int NightEndHour = 6;    // 6 AM

    private float currentTimeScale = DaytimeTimeScale;

    public void InitMod(Mod modInstance)
    {
        ModEvents.GameUpdate.RegisterHandler(GameUpdateHandler);
        Debug.Log("Nighttime Time Scale Modlet initialized");
    }

    private void GameUpdateHandler()
    {
        int currentHour = GetInGameHour();
        bool isNighttime = IsNighttime(currentHour);
        float desiredTimeScale = isNighttime ? NighttimeTimeScale : DaytimeTimeScale;

        if (desiredTimeScale != Time.timeScale)
        {
            Time.timeScale = desiredTimeScale;
            currentTimeScale = desiredTimeScale;
            Debug.Log($"Time scale set to {desiredTimeScale} at hour {currentHour}");
        }
        else
        {
            Debug.Log("Time scale remains at " + currentTimeScale);
        }
    }

    private int GetInGameHour()
    {
        if (GameManager.Instance != null && GameManager.Instance.World != null)
        {
            ulong worldTime = GameManager.Instance.World.worldTime;
            int hours = (int)(worldTime % 24);
            Debug.Log($"worldTime: {worldTime}, hours: {hours}");
            return hours;
        }
        else
        {
            Debug.LogWarning("GameManager or World is null. Defaulting hour to 0.");
            return 0;
        }
    }

    private bool IsNighttime(int hour)
    {
        bool isNight = hour >= NightStartHour || hour < NightEndHour;
        Debug.Log($"Hour {hour} is nighttime: {isNight}");
        return isNight;
    }
}