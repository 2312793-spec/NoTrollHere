using System.Collections;
using UnityEngine;
using TMPro;

// Gắn lên bất kỳ GameObject nào trong scene
// Khi player bước vào vùng trigger → hiện text trên đầu nhân vật
public class DialogueTrigger : MonoBehaviour
{
    [Header("=== NỘI DUNG ===")]
    [TextArea(2, 5)]
    public string[] cacDongThoai; // Nhiều dòng thoại, hiện lần lượt
    public float thoiGianMoiDong = 3f; // Mỗi dòng hiện bao lâu
    public float thoiGianFade = 0.5f;  // Thời gian fade in/out

    [Header("=== VỊ TRÍ TEXT ===")]
    public Vector3 offsetText = new Vector3(0f, 1.5f, 0f);
    // Offset so với đầu nhân vật

    [Header("=== STYLE ===")]
    public float coChu = 14f;
    public Color mauChu = new Color(0.91f, 0.91f, 0.82f, 1f); // #E8E8D0
    public Color mauNen = new Color(0.1f, 0.1f, 0.1f, 0.85f);
    public float rongToiDa = 300f;

    [Header("=== CÀI ĐẶT ===")]
    public bool chiHienMotLan = true;
    public bool dungGameKhiHien = false; // false = không pause game

    private bool daHien = false;
    private Transform playerTransform;
    private GameObject textObject;
    private TextMeshPro tmpText;
    private GameObject nenObject;
    private SpriteRenderer nenRenderer;

    void Start()
    {
        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // Tạo text object
        TaoTextObject();
    }

    void TaoTextObject()
    {
        // Tạo parent container
        textObject = new GameObject("DialogueText_" + gameObject.name);

        // Tạo nền (sprite renderer)
        nenObject = new GameObject("Background");
        nenObject.transform.SetParent(textObject.transform, false);
        nenRenderer = nenObject.AddComponent<SpriteRenderer>();
        nenRenderer.sprite = TaoSpriteNen();
        nenRenderer.color = mauNen;
        nenRenderer.sortingOrder = 10;

        // Tạo TMP text
        GameObject tmpObj = new GameObject("Text");
        tmpObj.transform.SetParent(textObject.transform, false);
        tmpText = tmpObj.AddComponent<TextMeshPro>();
        tmpText.fontSize = coChu;
        tmpText.color = mauChu;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.textWrappingMode = TextWrappingModes.Normal;
        tmpText.rectTransform.sizeDelta = new Vector2(rongToiDa / 100f, 10f);
        tmpText.sortingOrder = 11;

        // Ẩn ban đầu
        textObject.SetActive(false);
    }

    Sprite TaoSpriteNen()
    {
        // Tạo texture trắng 1x1 làm nền
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f), 1f);
    }

    void Update()
    {
        // Text luôn theo đầu player
        if (textObject != null && textObject.activeSelf
            && playerTransform != null)
        {
            textObject.transform.position =
                playerTransform.position + offsetText;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (chiHienMotLan && daHien) return;

        daHien = true;
        StartCoroutine(HienThiThoai());
    }

    IEnumerator HienThiThoai()
    {
        if (cacDongThoai == null || cacDongThoai.Length == 0)
            yield break;

        textObject.SetActive(true);

        foreach (string dong in cacDongThoai)
        {
            tmpText.text = dong;

            // Cập nhật kích thước nền theo text
            CapNhatKichThuocNen();

            // Fade in
            yield return StartCoroutine(FadeText(0f, 1f, thoiGianFade));

            // Hiện
            yield return new WaitForSeconds(thoiGianMoiDong);

            // Fade out
            yield return StartCoroutine(FadeText(1f, 0f, thoiGianFade));
        }

        textObject.SetActive(false);
    }

    void CapNhatKichThuocNen()
    {
        // Tính kích thước nền dựa theo text
        float chieuRong = Mathf.Min(
            tmpText.preferredWidth + 0.3f,
            rongToiDa / 100f + 0.3f);
        float chieuCao = tmpText.preferredHeight + 0.2f;

        nenObject.transform.localScale =
            new Vector3(chieuRong, chieuCao, 1f);
        nenObject.transform.localPosition = Vector3.zero;

        // Offset text lên trên nền một chút
        tmpText.transform.localPosition = new Vector3(0, 0, -0.1f);
    }

    IEnumerator FadeText(float tuAlpha, float denAlpha, float thoiGian)
    {
        float t = 0f;
        while (t < thoiGian)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(tuAlpha, denAlpha, t / thoiGian);

            // Fade text
            Color mauText = mauChu;
            mauText.a = alpha;
            tmpText.color = mauText;

            // Fade nền
            Color mauBg = mauNen;
            mauBg.a = mauNen.a * alpha;
            nenRenderer.color = mauBg;

            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        // Vẽ vùng trigger trong Scene view
        Gizmos.color = new Color(0.96f, 0.78f, 0.26f, 0.3f);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
            Gizmos.DrawCube(
                transform.position + (Vector3)box.offset,
                box.size);

#if UNITY_EDITOR
        if (cacDongThoai != null && cacDongThoai.Length > 0)
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 0.5f,
                "💬 " + cacDongThoai[0]);
#endif
    }

    void OnDestroy()
    {
        if (textObject != null)
            Destroy(textObject);
    }
}