using HarmonyLib;
using UnityEngine;

public class DisableDespawnModlet : IModApi
{
    public void InitMod(Mod modInstance)
    {
        var harmony = new Harmony("com.example.disabledespawn");
        harmony.PatchAll();
        Debug.Log("Disable Despawn Modlet initialized");
    }
}

// Patch for EntityEnemy canDespawn
[HarmonyPatch(typeof(EntityEnemy))]
[HarmonyPatch("canDespawn")]
public class DisableEnemyDespawnPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = false; // Prevent despawning for EntityEnemy
        return false; // Skip original method execution
    }
}

// Patch for EntityAlive CheckDespawn
[HarmonyPatch(typeof(EntityAlive))]
[HarmonyPatch("CheckDespawn")]
public class DisableEntityAliveDespawnPatch
{
    public static bool Prefix(EntityAlive __instance)
    {
        // Prevent despawn logic in CheckDespawn
        Debug.Log($"Preventedz despawn for entity: {__instance.entityName}");
        return false; // Skip original method execution
    }
}


[HarmonyPatch(typeof(EntityAlive))]
[HarmonyPatch("MarkToUnload")]
public class PreventEntityUnloadPatch
{
    public static bool Prefix()
    {
        Debug.Log("Preventedx MarkToUnload for an entity.");
        return false; // Prevent unloading
    }
}





[HarmonyPatch(typeof(Entity))]
[HarmonyPatch("MarkToUnload")]
public class PreventEntityMarkToUnloadPatch
{
    public static bool Prefix(Entity __instance)
    {
        Debug.Log($"Prevented1MarkToUnload for entity  ");
        return false; // Prevent the method from executing, thereby stopping unload
    }
}




// New Patch for EntityAlive Despawn
[HarmonyPatch(typeof(EntityAlive))]
[HarmonyPatch("Despawn")]
public class PreventEntityAliveDespawnPatch
{
    public static bool Prefix(EntityAlive __instance)
    {
        // Set IsDespawned to false
        __instance.IsDespawned = false;
        Debug.Log($"iPrevented Despawn for entity: {__instance.entityId}");
        return false; // Skip original method execution
    }
}


[HarmonyPatch(typeof(GameEventManager))]
[HarmonyPatch("RemoveSpawnedEntry")]
public class PreventRemoveSpawnedEntryPatch
{
    public static bool Prefix(GameEventManager __instance, Entity spawnedEntity)
    {
        // Check if the despawn should be prevented
        Debug.Log($"rPreventing despawn for entity: {spawnedEntity.entityId}");
        return false; // Skip original method execution
    }
}
