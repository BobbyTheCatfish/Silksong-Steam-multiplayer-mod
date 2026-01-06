using System.IO;
using SilksongMultiplayer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerComment : MonoBehaviour
{
    public GameObject nameText;
    public Canvas canva;
    SpriteRenderer spriteRenderer;
    public float minDistance = 1;
    public float randomBias = 0;
    GameObject bg;

    public void Init(string comment, Vector2 position)
    {
        GameObject nameCanva = new GameObject("nameCanva");
        nameCanva.transform.SetPositionAndRotation(this.transform.position, Quaternion.identity);
        nameCanva.transform.SetParent(this.transform);

        canva = nameCanva.AddComponent<Canvas>();
        canva.renderMode = RenderMode.ScreenSpaceCamera;
        canva.sortingLayerName = "HUD";
        canva.sortingLayerID = 629535577;
        canva.sortingOrder = 50;

        RectTransform rect = nameCanva.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(2560, 1440);

        nameText = new GameObject("nameText");
        nameText.transform.SetParent(nameCanva.transform);

        // A CanvasRenderer is required.
        nameText.AddComponent<CanvasRenderer>();
        nameText.transform.localScale = Vector3.one * 0.01f;
        //nameText.GetComponent<RectTransform>().sizeDelta = new Vector2(256000, 144000);
        Text text = nameText.AddComponent<Text>();
        text.text = comment; // or SteamFriends.GetPersonaName()
        text.font = SilksongMultiplayerAPI.savedFont;
        text.fontSize = 30;
        text.alignment = TextAnchor.MiddleCenter;

        // Give the Text's RectTransform reasonable anchor points/position (this won't affect your existing logic, it simply completes the layout).
        RectTransform textRect = nameText.GetComponent<RectTransform>();
        textRect.anchorMin = textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;

        // Allow the Text element to resize itself based on its "preferred content size."
        var fitter = nameText.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Create a background (placed below the text as a base layer)
        bg = new GameObject("nameText_BG");
        bg.transform.SetParent(nameCanva.transform, false);
        bg.transform.SetAsFirstSibling(); // Ensure it's at the very bottom layer (the background is behind the text).

        var bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = textRect.anchoredPosition;

        var img = bg.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.45f); // Semi-transparent black (transparency can be adjusted)

        Vector2 padding = new Vector2(20f, 10f);
        float w = text.preferredWidth + padding.x;
        float h = 1;

        bgRect.sizeDelta = new Vector2(w, h);
        bgRect.transform.localScale = new Vector2(0.01f, 0.5f);

        randomBias = Random.Range(-0.3f,0.3f);



        spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

        Rect spriteSize = new Rect(0, 0, 60, 60);
        Vector2Int textureSize = new Vector2Int(1, 1);

        Texture2D tex = Utils.LoadImage("dot.png", textureSize.x, textureSize.y);

        Sprite sprite = Sprite.Create(
            tex,
            spriteSize,
            new Vector2(0.5f, 0.5f)
        );


        spriteRenderer.sprite = sprite;
    }

    public void Update()
    {
        if(Vector2.Distance(this.transform.position,SilksongMultiplayerAPI.Hero_Hornet.transform.position) < 4)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);
            nameText.SetActive(true);
            bg.SetActive(true);
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            nameText.SetActive(false);
            bg.SetActive(false);
        }

        if (canva != null)
        {
            canva.transform.localPosition = Vector3.zero + new Vector3(0, 0, 0);
            nameText.transform.localPosition = Vector3.zero;
            nameText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
        }

        if (canva != null)
        {
            canva.renderMode = RenderMode.WorldSpace;
            canva.transform.localPosition = Vector3.zero;
            nameText.transform.localPosition = Vector3.zero;
        }
    }
}
