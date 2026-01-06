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

    [BepInPlugin("com.XvX", "XvX", "0.10.8.0")]
    public class Plugin : BaseUnityPlugin
    {

        // Define configuration items
        private ConfigEntry<bool> enablePvP;
        private ConfigEntry<float> BossHPmultiplier;
        private ConfigEntry<float> EnemyHPmultiplier;

        private ConfigEntry<string> SkinName;
        private ConfigEntry<string> SkinLink1;
        private ConfigEntry<string> SkinLink2;
        private ConfigEntry<string> SkinLink3;
        private ConfigEntry<string> SkinLink4;

        private ConfigEntry<bool> ShowComments;
        private ConfigEntry<bool> ShowNametags;


        void Awake()
        {
            // Initialize the configuration (if the configuration file does not exist, default values ​​will be written).
            enablePvP = Config.Bind("General", "enablePvP", false, "Enable PvP?");

            SilksongMultiplayerAPI.enablePvP = enablePvP.Value;

            ShowComments = Config.Bind("General", "ShowComments", true, "Should comments be enabled?");
            SilksongMultiplayerAPI.showComments = ShowComments.Value;

            ShowNametags = Config.Bind("General", "ShowNames", true, "Should names be shown above players?");
            SilksongMultiplayerAPI.showNametags = ShowNametags.Value;

            BossHPmultiplier = Config.Bind("General", "BossHPmultiplier", 1f, "Boss health multiplier (Setting it to 0 means no extra health for each additional player; setting it to 1 means the boss's health doubles for each additional player).");

            SilksongMultiplayerAPI.BossHPmultiplier = BossHPmultiplier.Value;


            EnemyHPmultiplier = Config.Bind("General", "EnemyHPmultiplier", 0f, "Normal enemy health multiplier (Setting it to 0 means no extra health for each additional player; setting it to 1 means the health doubles for each additional player).");
            SilksongMultiplayerAPI.EnemyHPmultiplier = EnemyHPmultiplier.Value;

            SkinName = Config.Bind("Skin", "SkinName", "default", "Skin name, corresponding to the skin folder name.  Try to avoid duplicating names with other skins, and include the skin version number. Name template: (abcd123)");
            SkinLink1 = Config.Bind("Skin", "SkinLink1", "", "Skin image link, corresponding to image knight atlas0");
            SkinLink2 = Config.Bind("Skin", "SkinLink2", "", "Skin image link, corresponding to image knight atlas1");
            SkinLink3 = Config.Bind("Skin", "SkinLink3", "", "Skin image link, corresponding to image knight atlas2");
            SkinLink4 = Config.Bind("Skin", "SkinLink4", "", "Skin image link, corresponding to image knight atlas3");

            SilksongMultiplayerAPI.skinName = SkinName.Value;
            SilksongMultiplayerAPI.skinLink1 = SkinLink1.Value;
            SilksongMultiplayerAPI.skinLink2 = SkinLink2.Value;
            SilksongMultiplayerAPI.skinLink3 = SkinLink3.Value;
            SilksongMultiplayerAPI.skinLink4 = SkinLink4.Value;


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
