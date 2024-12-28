using HarmonyLib;
using UnityEngine;

public class DisableZombieDespawnModlet : IModApi
{
    public void InitMod(Mod modInstance)
    {
        var harmony = new Harmony("com.example.disablezombiedespawn");
        harmony.PatchAll();
        Debug.Log("Disable Zombie Despawn Modlet initialized");
    }
}

[HarmonyPatch(typeof(EntityEnemy))]
[HarmonyPatch("canDespawn")]
public class DisableZombieDespawnPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = false; // Prevent despawning
        return false; // Skip original method execution
    }
}
