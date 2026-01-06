using BepInEx;
using HarmonyLib;
using SilksongMultiplayer.NetworkData;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMProOld;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Networking.UnityWebRequest;

namespace SilksongMultiplayer.Chat
{
    internal class ChatUI : MonoBehaviour
    {
        public static GameObject chatCanvasGO;
        public static GameObject chatDisplayGO;
        public static InputField chatInput;
        public static ScrollRect scroller;
        //private string typingMessage = "";

        public List<string> ChatHistory = new List<string>();
        private static bool isActive = false;
        private static bool created = false;
        private Vector2 chatSize;

        const float CHAT_INPUT_HEIGHT = 50;
        const int CHAT_MARGIN = 10;
        const int CHAT_HISTORY_HEIGHT = 300;
        const int SCROLL_SENSITIVITY = 10;

        void CreateCanvas(tk2dCamera hud)
        {
            // CREATE CHAT CANVAS
            chatCanvasGO = new GameObject("CHAT CANVAS");

            Canvas chatCanvas = chatCanvasGO.AddComponent<Canvas>();

            chatCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            //chatCanvas.sortingLayerName = "HUD";
            //chatCanvas.sortingLayerID = 629535577;
            chatCanvas.sortingOrder = 55;

            chatCanvasGO.AddComponent<GraphicRaycaster>();
            chatCanvasGO.AddComponent<CanvasScaler>();

            VerticalLayoutGroup chatCanvasLayout = chatCanvasGO.AddComponent<VerticalLayoutGroup>();
            chatCanvasLayout.padding.left = CHAT_MARGIN;
            chatCanvasLayout.padding.bottom = CHAT_MARGIN;
            chatCanvasLayout.childAlignment = TextAnchor.LowerLeft;
            chatCanvasLayout.childControlHeight = true;
            chatCanvasLayout.childControlWidth = false;
            chatCanvasLayout.childForceExpandHeight = false;
            chatCanvasLayout.childForceExpandWidth = false;

            chatCanvasGO.transform.SetParent(hud.gameObject.transform);
        }

        void CreateTextDisplay()
        {
            // CREATE SCROLL PARENT
            GameObject scrollGO = new GameObject("CHAT DISPLAY (SCROLL)");
            scrollGO.transform.SetPositionAndRotation(chatCanvasGO.transform.position, Quaternion.identity);
            scrollGO.transform.SetParent(chatCanvasGO.transform);

            // set up basic scroll settings
            scrollGO.AddComponent<CanvasRenderer>();
            scroller = scrollGO.AddComponent<ScrollRect>();
            scroller.horizontal = false;
            scroller.vertical = true;
            scroller.movementType = ScrollRect.MovementType.Clamped;
            scroller.scrollSensitivity = SCROLL_SENSITIVITY;

            var layout = scrollGO.AddComponent<LayoutElement>();
            layout.minHeight = 100;
            layout.preferredHeight = CHAT_HISTORY_HEIGHT;

            var transform = scrollGO.GetComponent<RectTransform>();
            transform.sizeDelta = new Vector2(400, transform.sizeDelta.y);


            // CREATE SCROLL VIEWPORT
            GameObject viewportGO = new GameObject("VIEWPORT");
            viewportGO.transform.SetParent(scrollGO.transform);
            viewportGO.AddComponent<Image>();
            viewportGO.AddComponent<Mask>().showMaskGraphic = false;

            // stretch to parent
            transform = viewportGO.GetComponent<RectTransform>();
            transform.anchorMin = Vector3.zero;
            transform.anchorMax = Vector3.one;
            transform.offsetMin = Vector3.zero;
            transform.offsetMax = Vector3.zero;

            scroller.viewport = transform;


            // CREATE SCROLL CONTENT
            chatDisplayGO = new GameObject("CHAT DISPLAY (CONTENT)");
            chatDisplayGO.transform.SetParent(viewportGO.transform);
            chatDisplayGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // add layout for chats
            var chatDisplayLayout = chatDisplayGO.AddComponent<VerticalLayoutGroup>();
            chatDisplayLayout.spacing = 5;
            chatDisplayLayout.childForceExpandHeight = false;
            chatDisplayLayout.childForceExpandWidth = false;
            chatDisplayLayout.childControlHeight = true;
            chatDisplayLayout.childControlWidth = true;
            chatDisplayLayout.childAlignment = TextAnchor.LowerLeft;

            // stretch to parent
            transform = chatDisplayGO.GetComponent<RectTransform>();
            transform.anchorMin = Vector3.zero;
            transform.anchorMax = Vector3.one;
            transform.offsetMin = Vector3.zero;
            transform.offsetMax = Vector3.zero;
            transform.pivot = Vector3.zero;

            scroller.content = transform;
        }

        void CreateChatInput()
        {
            // CREATE TEXT INPUT
            GameObject chatInputGO = new GameObject("CHAT INPUT");
            chatInputGO.transform.SetParent(chatCanvasGO.transform);


            // Add UI background texture
            var image = chatInputGO.AddComponent<Image>();
            var tex = Utils.LoadImage("InputFieldBG.png", 32, 32);

            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, 32, 32),
                new Vector2(0.5f, 0.5f),
                100,
                1,
                SpriteMeshType.FullRect,
                new Vector4(10, 10, 10, 10)
            );

            image.sprite = sprite;
            image.color = Color.white;
            image.raycastTarget = true;
            image.maskable = true;
            image.type = Image.Type.Tiled;
            image.fillCenter = true;
            image.pixelsPerUnitMultiplier = 1;

            // Create input field
            chatInput = chatInputGO.AddComponent<InputField>();
            chatInput.interactable = true;
            chatInput.transition = Selectable.Transition.ColorTint;
            chatInput.shouldActivateOnSelect = true;

            // Set input colors and other values
            ColorBlock colors = new ColorBlock();
            colors.normalColor = new Color(1, 1, 1, 0.2f);
            colors.highlightedColor = colors.normalColor;
            colors.pressedColor = chatInput.colors.pressedColor;
            colors.selectedColor = Color.white;
            colors.disabledColor = chatInput.colors.disabledColor;
            colors.colorMultiplier = 1;
            colors.fadeDuration = 0.1f;

            chatInput.colors = colors;


            // Set self layout and transform
            var chatInputSelfLayout = chatInputGO.AddComponent<LayoutElement>();
            chatInputSelfLayout.preferredHeight = 40;

            var _a = chatInputGO.GetComponent<RectTransform>();
            _a.sizeDelta = new Vector2(400, _a.sizeDelta.y);

            chatInput.onSubmit.AddListener(OnSendMessage);
            chatInput.DeactivateInputField();

            Debug.Log("Text input created");


            // CREATE PLACEHOLDER TEXT OBJECT
            GameObject inputPlaceholderGO = new GameObject("CHAT INPUT PLACEHOLDER");
            inputPlaceholderGO.transform.SetParent(chatInputGO.transform);

            Text inputPlaceholder = inputPlaceholderGO.AddComponent<Text>();
            SetTextboxPosition(inputPlaceholderGO);
            SetTextSettings(inputPlaceholder);
            inputPlaceholder.text = "Enter a message";
            chatInput.placeholder = inputPlaceholder;

            Debug.Log("Input placeholder created");


            // CREATE TEXT INPUT OBJECT
            GameObject inputTextGO = new GameObject("CHAT INPUT TEXT");
            inputTextGO.transform.SetParent(chatInputGO.transform);

            Text inputText = inputTextGO.AddComponent<Text>();
            SetTextboxPosition(inputTextGO);
            SetTextSettings(inputText);
            inputText.supportRichText = false;
            chatInput.textComponent = inputText;
        }

        public void CreateChatUI(tk2dCamera hud)
        {
            if (chatInput != null) return;

            Debug.Log(hud.gameObject.name + " found, creating chat");
            CreateCanvas(hud);

            // CREATE CHAT DISPLAY
            CreateTextDisplay();
            Debug.Log("Chat display created");

            CreateChatInput();
            Debug.Log("Input text created");
        }

        void SetTextSettings(Text text)
        {
            text.fontSize = 14;
            text.font = SilksongMultiplayerAPI.savedFont;

            text.color = Color.black;
            text.alignment = TextAnchor.MiddleLeft;
            text.verticalOverflow = VerticalWrapMode.Truncate;
        }
        public void SetTextboxPosition(GameObject texboxGO)
        {
            var _a = texboxGO.GetComponent<RectTransform>();
            _a.sizeDelta = new Vector2(393, 25);
            _a.anchoredPosition = new Vector3(7, 0, 0);
        }

        void Update()
        {
            if (!created)
            {
                tk2dCamera hud = FindFirstObjectByType<tk2dCamera>();
                if (hud != null && SilksongMultiplayerAPI.savedFont)
                {
                    created = true;
                    CreateChatUI(hud);
                }
            }


            if (Input.GetKeyDown(KeyCode.T) && !isActive)
            {
                isActive = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape) && isActive)
            {
                isActive = false;
                chatInput.text = "";
            }

            if (chatInput != null)
            {
                if (isActive) chatInput.ActivateInputField();
                else chatInput.DeactivateInputField();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                tk2dCamera hud = FindFirstObjectByType<tk2dCamera>();
                CreateChatUI(hud);
            }

            //if (chatInput != null && chatInput.isFocused && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
            //{

            //}
        }

        void OnSendMessage(string msg)
        {
            isActive = false;
            chatInput.text = "";

            if (!string.IsNullOrEmpty(msg))
            {
                NetworkDataSender.SendChatMessage(msg);
                string name = SteamFriends.GetPersonaName();

                DisplayMessage(msg, name);
                scroller.normalizedPosition = new Vector2(0, 0);
            }
        }

        void DisplayMessage(string msg, string name)
        {
            string newMsg = $"[{name}] {msg}";
            GameObject chatMessage = new GameObject("ChatMessage");
            chatMessage.transform.SetParent(chatDisplayGO.gameObject.transform);

            Text text = chatMessage.AddComponent<Text>();
            text.text = newMsg;
            text.font = SilksongMultiplayerAPI.savedFont;
            text.fontSize = 14;
        }

        public void OnReceiveMessage(string msg, CSteamID steamID)
        {
            string name = SteamFriends.GetFriendPersonaName(steamID);
            DisplayMessage(msg, name);
        }

        [HarmonyPatch(typeof(HeroController), "IsInputBlocked")]
        [HarmonyPostfix]
        public static void IsInputBlocked(ref bool __result)
        {
            if (chatInput != null && isActive)
            {
                __result = true;
                //Debug.Log("BLOCK INPUT");
            }
        }
    }
}
