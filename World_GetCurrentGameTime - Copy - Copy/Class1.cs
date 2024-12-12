using UnityEngine;
using System;

public class NighttimeTimeScaleModlet : IModApi
{
    private const float DaytimeTimeScale = 1.0f;
    private const float NighttimeTimeScale = 0.1f;
    private const int NightStartHour = 22; // 10 PM
    private const int NightEndHour = 4;    // 4 AM

    private float currentTimeScale = DaytimeTimeScale;
    private int lastHour = -1;

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

        //if (desiredTimeScale != Time.timeScale)
        //{
        //    Time.timeScale = desiredTimeScale;
        //    currentTimeScale = desiredTimeScale;
        //    Debug.Log($"Time scale set to {desiredTimeScale} at hour {currentHour}");
        //}
        //else
        //{
        //    Debug.Log("Time scale remains at " + currentTimeScale);
        //}

        // Check for night start and end
        if (IsNighttime(currentHour))
        {
            if (currentHour == NightStartHour)
            {
                ExecuteTimeOfDayIncPerSec(60);
                Debug.Log("Night started. Time of day increment per second set to 60.");
            }
            else if (currentHour == NightEndHour)
            {
                ExecuteTimeOfDayIncPerSec(6);
                Debug.Log("Night ended. Time of day increment per second set to 6.");
            }

            lastHour = currentHour;
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

    private void ExecuteTimeOfDayIncPerSec(int value)
    {
        EnumGameStats stats = EnumGameStats.TimeOfDayIncPerSec;
        object obj2 = GameStats.Parse(stats, value.ToString());
        GameStats.SetObject(stats, obj2);
        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(stats.ToStringCached<EnumGameStats>() + " set to " + obj2?.ToString());
    }
}