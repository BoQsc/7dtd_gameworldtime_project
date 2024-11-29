using UnityEngine;

public class MyMod : IModApi
{
    public void InitMod(Mod _modInstance)
    {
        Debug.Log("MyMod initialized!");
        ModEvents.GameUpdate.RegisterHandler(OnGameUpdate);
    }

    private void OnGameUpdate()
    {
        // Your mod logic here
    }
}