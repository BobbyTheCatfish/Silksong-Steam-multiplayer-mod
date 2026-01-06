using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SilksongMultiplayer.MainMenu
{
    public class InviteLobbyButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (SilksongMultiplayerAPI.RoomManager.enterRoom)
            {
                // Open the Steam invite friends interface.
                SilksongMultiplayerAPI.RoomManager.Invite();
            }
        }

        public void Update()
        {
            transform.GetChild(0).GetComponent<Text>().text = "Invite Friends";

            if (GetComponent<EventTrigger>())
                GetComponent<EventTrigger>().enabled = false;
        }
    }
}
