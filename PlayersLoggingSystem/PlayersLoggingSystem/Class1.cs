using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerDataLoggingModlet : IModApi
{
    private const float PollingIntervalSeconds = 4f; // Poll every 4 seconds
    private float _nextPollTime;
    private string _logFilePath;

    public void InitMod(Mod _modInstance)
    {
        ModEvents.GameUpdate.RegisterHandler(GameUpdateHandler);
        _logFilePath = Path.Combine(GameIO.GetSaveGameDir(), "PlayerDataLog.txt");
        Debug.Log("Player Data Logging Modlet initialized");
    }

    private void GameUpdateHandler()
    {
        if (Time.time >= _nextPollTime)
        {
            LogPlayerData();
            _nextPollTime = Time.time + PollingIntervalSeconds;
        }
    }

    private void LogPlayerData()
    {
        if (GameManager.Instance?.World == null || ConnectionManager.Instance.Clients.Count == 0)
        {
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Player Data Log - {DateTime.Now}");
        sb.AppendLine("----------------------------------------");

        foreach (ClientInfo cInfo in ConnectionManager.Instance.Clients.List)
        {
            EntityPlayer player = GameManager.Instance.World.Players.dict[cInfo.entityId];
            if (player != null)
            {
                sb.AppendLine($"Player: {cInfo.playerName}");
                sb.AppendLine($"EntityId: {cInfo.entityId}");
                sb.AppendLine($"Position: {player.GetPosition()}");
                sb.AppendLine($"Health: {player.Health:F1}/{player.Stats.Health.Max:F1}");
                sb.AppendLine($"Level: {player.Progression.Level}");
                sb.AppendLine($"Experience: {player.Progression.ExpToNextLevel:F0}/{player.Progression.ExpForNextLevel:F0}");

                sb.AppendLine("Inventory:");
                for (int i = 0; i < player.inventory.slots.Count; i++)
                {
                    ItemStack itemStack = player.inventory.GetItem(i);
                    if (itemStack.IsEmpty()) continue;
                    sb.AppendLine($"  - Slot {i}: {itemStack.itemValue.ItemClass.GetItemName()} x{itemStack.count}");
                }

                sb.AppendLine("----------------------------------------");
            }
        }

        try
        {
            File.AppendAllText(_logFilePath, sb.ToString());
            Debug.Log($"Player data logged to {_logFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing to log file: {ex.Message}");
        }
    }
}