using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SilksongMultiplayer
{
    internal static class NametagManager
    {
        private static GameObject _AddNameTag(string name, Transform transform, ref Canvas canva, bool isSelf = false)
        {

            GameObject nameCanva = new GameObject("nameCanva");
            nameCanva.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            nameCanva.transform.SetParent(transform);
                
            canva = nameCanva.AddComponent<Canvas>();
            canva.renderMode = RenderMode.WorldSpace;
            canva.sortingLayerName = "HUD";
            canva.sortingLayerID = 629535577;
            canva.sortingOrder = 50;

            if (isSelf) canva.renderMode = RenderMode.ScreenSpaceCamera;

            RectTransform rect = nameCanva.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(2560, 1440);

            if (!Configuration.ShowNametags) return null;


            var nameText = new GameObject("nameText");
            nameText.transform.SetParent(canva.gameObject.transform);

            // 必须有 CanvasRenderer
            nameText.AddComponent<CanvasRenderer>();
            nameText.transform.localScale = Vector3.one * 0.01f;

            Text text = nameText.AddComponent<Text>();
            text.text = name;
            text.font = SilksongMultiplayerAPI.savedFont;
            text.fontSize = 50;
            text.alignment = TextAnchor.MiddleCenter;

            ulong XvXSteamId64 = 76561198929282998UL;
            ulong truthSteamId64 = 76561199835946204UL;

            if (SteamUser.GetSteamID().m_SteamID == XvXSteamId64 || SteamUser.GetSteamID().m_SteamID == truthSteamId64)
            {
                text.color = Color.yellow;
            }

            return nameText;
        }

        public static GameObject AddNametag(Transform transform, ref Canvas canvas)
        {
            string name = SteamFriends.GetPersonaName();
            return _AddNameTag(name, transform, ref canvas, true);
        }
        public static GameObject AddNametag(Transform transform, ref Canvas canvas, CSteamID steamID)
        {
            string name = SteamFriends.GetFriendPersonaName(steamID);
            return _AddNameTag(name, transform, ref canvas);
        }
    }
}
