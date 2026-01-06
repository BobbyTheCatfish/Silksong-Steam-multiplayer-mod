using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using TMProOld;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InControl.InControlInputModule;

namespace SilksongMultiplayer.MainMenu
{
    internal class CreateLobbyButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            CreateLobby();
        }

        public void CreateLobby()
        {
            SilksongMultiplayerAPI.RoomManager.CreateRoom();

            GameObject button2 = Instantiate(GameObject.Find("OptionsButton"), GameObject.Find("OptionsButton").transform.parent);
            button2.transform.GetChild(0).GetComponent<Text>().text = "Invite Friends";
            button2.AddComponent<InviteLobbyButton>();
            SilksongMultiplayerAPI.inviteLobbyButton = button2;

            Destroy(gameObject);
        }

        public void Update()
        {
            transform.GetChild(0).GetComponent<Text>().text = "Create Lobby";
            SilksongMultiplayerAPI.savedFont = GameObject.Find("OptionsButton").transform.GetChild(0).GetComponent<Text>().font;

            if (GetComponent<EventTrigger>())
                GetComponent<EventTrigger>().enabled = false;

            CreateLobby();
        }
    }
}
