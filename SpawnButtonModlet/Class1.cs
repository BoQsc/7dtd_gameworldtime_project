using System;
using UnityEngine;
using HarmonyLib;
using System.Reflection;


namespace SpawnButtonMod
{
    public class SpawnButtonModlet : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            Debug.Log("Loading Spawn Button Modlet");
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(XUiC_MainMenu))]
        [HarmonyPatch("Init")]
        public class XUiC_MainMenu_Init_Patch
        {
            public static void Postfix(XUiC_MainMenu __instance)
            {
                XUiController childById = __instance.GetChildById("btnSpawn");
                if (childById != null)
                {
                    XUiV_Button spawnButton = childById.ViewComponent as XUiV_Button;
                    if (spawnButton != null)
                    {
                        spawnButton.Label = "Custom Spawn";
                    }
                }
            }
        }
    }
}