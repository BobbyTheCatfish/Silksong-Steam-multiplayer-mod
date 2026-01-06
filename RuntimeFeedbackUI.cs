using SilksongMultiplayer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RuntimeFeedbackUI : MonoBehaviour
{
    // You want to disable the "game control scripts" (player movement, camera, attacks, etc.)
    [Header("Disable these while UI is open")]
    public MonoBehaviour[] disableWhileOpen = new MonoBehaviour[0];

    private GameObject canvasGO;
    private GameObject panel;
    private InputField inputField;

    private bool isOpen;

    void Update()
    {
        // Open/close the UI (you can change the keybind).
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (canvasGO == null) CreateUI();
            if (!isOpen) Open();
            else Close();
        }

        // When the UI is open: Press Enter to submit, press Esc to cancel.
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Submit();
            }
        }
    }

    void CreateUI()
    {
        // ===== Canvas =====
        canvasGO = new GameObject("FeedbackCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem (required)
        if (FindObjectOfType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // ===== Panel =====
        panel = CreateUIObject("Panel", canvasGO.transform);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.6f);
        var panelRT = panel.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(560, 220);
        panelRT.anchoredPosition = Vector2.zero;

        // ===== InputField =====
        var inputGO = CreateUIObject("InputField", panel.transform);
        var inputImg = inputGO.AddComponent<Image>();
        inputImg.color = Color.white;

        inputField = inputGO.AddComponent<InputField>();
        var inputRT = inputGO.GetComponent<RectTransform>();
        inputRT.sizeDelta = new Vector2(520, 80);
        inputRT.anchoredPosition = new Vector2(0, 10);

        // Text
        Text text = CreateText("Text", inputGO.transform, "");
        text.alignment = TextAnchor.UpperLeft;
        inputField.textComponent = text;

        // Placeholder
        Text placeholder = CreateText("Placeholder", inputGO.transform, "Leave feedback... (Enter to submit / Esc to cancel)");
        placeholder.color = Color.gray;
        inputField.placeholder = placeholder;

        // Hidden by default.
        panel.SetActive(false);
        isOpen = false;
    }

    void Open()
    {
        isOpen = true;
        panel.SetActive(true);

        // Clear and focus the input field.
        inputField.text = "";
        inputField.ActivateInputField();

        // Unlock the mouse (if your game locks the mouse).
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable game operation scripts.
        foreach (var c in disableWhileOpen)
        {
            if (c != null) c.enabled = false;
        }
    }

    void Close()
    {
        isOpen = false;
        panel.SetActive(false);

        // Restore the mouse cursor (decide whether to lock it back based on your game requirements).
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Restore game operation script
        foreach (var c in disableWhileOpen)
        {
            if (c != null) c.enabled = true;
        }
    }

    void Submit()
    {
        string msg = inputField.text.Trim();
        if (msg.Length < 2) return; // Your backend requires a version greater than or equal to 2.

        Debug.Log("Feedback: " + msg);

        // TODO: Connect to your existing transmitter (example)
        Vector2 playerPos = SilksongMultiplayerAPI.Hero_Hornet.transform.position;

        SilksongMultiplayerAPI.RoomManager.feedbackSender.SendFeedback(playerPos, msg);

        GameObject comment = GameObject.Instantiate(new GameObject(), SilksongMultiplayerAPI.Hero_Hornet.transform.position + new Vector3(0,0,0),Quaternion.identity);
        PlayerComment playerComment = comment.AddComponent<PlayerComment>();
        playerComment.Init(msg, playerPos);
        Close();
    }

    // ===== Tools and methods =====
    GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    Text CreateText(string name, Transform parent, string content)
    {
        GameObject go = CreateUIObject(name, parent);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.text = content;
        t.color = Color.black;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(12, 10);
        rt.offsetMax = new Vector2(-12, -10);

        return t;
    }
}
