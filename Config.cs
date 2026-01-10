using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SilksongMultiplayer
{
    internal static class Configuration
    {
        // Define configuration items
        private static ConfigEntry<bool> _enablePvP;
        private static ConfigEntry<float> _BossHPmultiplier;
        private static ConfigEntry<float> _EnemyHPmultiplier;
        internal static bool enablePvP { get { return _enablePvP.Value; } }
        internal static float BossHPmultiplier { get { return _BossHPmultiplier.Value; } }
        internal static float EnemyHPmultiplier { get { return _EnemyHPmultiplier.Value; } }

        private static ConfigEntry<string> _SkinName;
        private static ConfigEntry<string> _SkinLink1;
        private static ConfigEntry<string> _SkinLink2;
        private static ConfigEntry<string> _SkinLink3;
        private static ConfigEntry<string> _SkinLink4;

        internal static string SkinName { get { return _SkinName.Value; } }
        internal static string[] SkinLinks
        {
            get
            {
                string[] skinLinks = { _SkinLink1.Value, _SkinLink2.Value, _SkinLink3.Value, _SkinLink4.Value };
                return skinLinks;
            }
        }

        private static ConfigEntry<bool> _ShowComments;
        private static ConfigEntry<bool> _DebugText;
        private static ConfigEntry<bool> _ShowNametags;
        private static ConfigEntry<KeyCode> _OpenChatButton;

        internal static bool ShowComments { get { return _ShowComments.Value; } }
        internal static bool DebugText { get { return _DebugText.Value; } }
        internal static bool ShowNametags { get { return _ShowNametags.Value; } }
        internal static KeyCode OpenChatButton { get { return _OpenChatButton.Value; } }
        public static void CreateConfig(ConfigFile configFile)
        {
            _enablePvP = configFile.Bind("General", "enablePvP", false, "Enable PvP?");


            _ShowComments = configFile.Bind("General", "ShowComments", true, "Should comments be enabled?");

            _DebugText = configFile.Bind("DebugText", "DebugText", false, "是否开启信息显示");

            _ShowNametags = configFile.Bind("General", "ShowNames", true, "Should names be shown above players?");

            _BossHPmultiplier = configFile.Bind("General", "BossHPmultiplier", 1f, "Boss health multiplier (Setting it to 0 means no extra health for each additional player; setting it to 1 means the boss's health doubles for each additional player).");


            _EnemyHPmultiplier = configFile.Bind("General", "EnemyHPmultiplier", 0f, "Normal enemy health multiplier (Setting it to 0 means no extra health for each additional player; setting it to 1 means the health doubles for each additional player).");

            _SkinName = configFile.Bind("Skin", "SkinName", "default", "Skin name, corresponding to the skin folder name.  Try to avoid duplicating names with other skins, and include the skin version number. Name template: (abcd123)");
            _SkinLink1 = configFile.Bind("Skin", "SkinLink1", "", "Skin image link, corresponding to image knight atlas0");
            _SkinLink2 = configFile.Bind("Skin", "SkinLink2", "", "Skin image link, corresponding to image knight atlas1");
            _SkinLink3 = configFile.Bind("Skin", "SkinLink3", "", "Skin image link, corresponding to image knight atlas2");
            _SkinLink4 = configFile.Bind("Skin", "SkinLink4", "", "Skin image link, corresponding to image knight atlas3");

            _OpenChatButton = configFile.Bind("General", "OpenChatKey", KeyCode.T, "The keybind to open chat");
        }
    }

}
