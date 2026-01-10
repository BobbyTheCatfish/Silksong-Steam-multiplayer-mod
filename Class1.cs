namespace SilksongMultiplayer
{
    using System.Reflection;
    using System.Runtime.ConstrainedExecution;
    using BepInEx;
    using BepInEx.Configuration;
    using BepInEx.Logging;
    using GlobalSettings;
    using HarmonyLib;
    using HutongGames.PlayMaker.Actions;
    using QuestPlaymakerActions;
    using SilksongMultiplayer.Chat;
    using SilksongMultiplayer.NetworkData;
    using Steamworks;
    using TeamCherry.Localization;
    using UnityEngine;

    //[HarmonyPatch(typeof(SomeGameClass), "TargetMethod")]
    public static class PatchClass
    {
        static void Prefix()
        {
            Debug.Log("The target method has been called!");
        }
    }


    [BepInPlugin("com.XvX", "XvX", "0.10.20.0")]
    public class Plugin : BaseUnityPlugin
    {

       

        private ChatUI chat = new ChatUI();

        void Awake()
        {
            // Initialize the configuration (if the configuration file does not exist, default values ​​will be written).
            Configuration.CreateConfig(Config);

            // Plugin startup logic
            SilksongMultiplayerAPI.Logger = base.Logger;
            Logger.LogInfo($"Plugin XvX is loaded!");

            Harmony harmony = new Harmony("com.XvX");
            harmony.PatchAll();
            Harmony.CreateAndPatchAll(typeof(Chat.ChatUI), "com.XvX");

            Debug.Log("Initializing the lobby system.");

            SilksongMultiplayerAPI.RoomManagerObject = GameObject.Instantiate(new GameObject("LobbyManager"));
            SilksongMultiplayerAPI.RoomManagerObject.AddComponent<RoomManager>();
            SilksongMultiplayerAPI.RoomManagerObject.AddComponent<NetworkDataReceiver>();

            SilksongMultiplayerAPI.RoomManagerObject = GameObject.Find("LobbyManager(Clone)");

            GameObject.DontDestroyOnLoad(SilksongMultiplayerAPI.RoomManagerObject);
        }
    }

    [HarmonyPatch(typeof(StartManager), nameof(StartManager.SwitchToMenuScene))]
    static class RestoreLanguagePatch
    {
        // Preprocessing methods
        static void Prefix()
        {
            Debug.Log("[Harmony] RestoreLanguageSelection Called");

            SilksongMultiplayerAPI.RoomManagerObject = GameObject.Instantiate(new GameObject("LobbyManager"));
            SilksongMultiplayerAPI.RoomManagerObject.AddComponent<RoomManager>();
            SilksongMultiplayerAPI.RoomManagerObject.AddComponent<NetworkDataReceiver>();

            SilksongMultiplayerAPI.RoomManagerObject = GameObject.Find("LobbyManager(Clone)");

            GameObject.DontDestroyOnLoad(SilksongMultiplayerAPI.RoomManagerObject);

            //Logger.LogInfo($"LobbyManager is start");


            // If you want to directly replace the return value, you can:
            // __result = "en";
            // return false; // Prevent the original method from executing
        }
    }
}
