using UnityEngine;
using UnityEngine.UI;

// Gắn lên GameObject rỗng trong Canvas — thay thế Background tĩnh
// Tạo hiệu ứng nền động theo theme từng chương
public class ChapterBackground : MonoBehaviour
{
    public static ChapterBackground instance;

    [Header("=== CÀI ĐẶT ===")]
    public int soLuongPhanTu = 20;

    private RectTransform rectTransform;
    private GameObject[] cacPhanTu;
    private Image[] cacImage;
    private Vector2[] cacVanToc;
    private float[] cacPha;
    private int chuongHienTai = 1;

    // =====================================================
    // CẤU HÌNH TỪNG CHƯƠNG
    // =====================================================
    private struct CauHinhChuong
    {
        public Color mauNen;
        public Color[] mauPhanTu;
        public float tocDoMin, tocDoMax;
        public float kichThuocMin, kichThuocMax;
        public float doMoMin, doMoMax;
        public Vector2 huongDiChuyen; // hướng chuyển động
    }

    private readonly CauHinhChuong[] cauHinhCacChuong = new CauHinhChuong[]
    {
        // Special (idx 0) — tím, hạt bay ngẫu nhiên hỗn loạn
        new CauHinhChuong {
            mauNen = new Color(0.071f, 0.031f, 0.102f),
            mauPhanTu = new Color[] {
                new Color(0.75f, 0.52f, 0.99f, 1f),
                new Color(0.91f, 0.49f, 0.98f, 1f),
                new Color(0.55f, 0.32f, 0.79f, 1f),
            },
            tocDoMin = 15f, tocDoMax = 60f,
            kichThuocMin = 2f, kichThuocMax = 8f,
            doMoMin = 0.03f, doMoMax = 0.15f,
            huongDiChuyen = new Vector2(0.3f, 1f)
        },
        // Chapter 1 (idx 1) — vàng chalk, hạt trôi lên chậm
        new CauHinhChuong {
            mauNen = new Color(0.102f, 0.102f, 0.102f),
            mauPhanTu = new Color[] {
                new Color(0.91f, 0.91f, 0.82f, 1f),
                new Color(0.96f, 0.78f, 0.26f, 1f),
                new Color(0.70f, 0.70f, 0.65f, 1f),
            },
            tocDoMin = 8f, tocDoMax = 25f,
            kichThuocMin = 2f, kichThuocMax = 6f,
            doMoMin = 0.03f, doMoMax = 0.12f,
            huongDiChuyen = new Vector2(0.1f, 1f)
        },
        // Chapter 2 (idx 2) — xanh rừng, hạt lá bay
        new CauHinhChuong {
            mauNen = new Color(0.051f, 0.102f, 0.071f),
            mauPhanTu = new Color[] {
                new Color(0.78f, 0.91f, 0.82f, 1f),
                new Color(0.29f, 0.87f, 0.50f, 1f),
                new Color(0.16f, 0.55f, 0.28f, 1f),
            },
            tocDoMin = 10f, tocDoMax = 30f,
            kichThuocMin = 3f, kichThuocMax = 9f,  // lá to hơn
            doMoMin = 0.04f, doMoMax = 0.14f,
            huongDiChuyen = new Vector2(0.5f, 1f)  // bay chéo như lá rơi
        },
        // Chapter 3 (idx 3) — cam núi lửa, hạt tàn lửa bay lên
        new CauHinhChuong {
            mauNen = new Color(0.102f, 0.051f, 0.039f),
            mauPhanTu = new Color[] {
                new Color(0.98f, 0.57f, 0.24f, 1f),
                new Color(0.94f, 0.27f, 0.27f, 1f),
                new Color(0.91f, 0.82f, 0.78f, 1f),
            },
            tocDoMin = 20f, tocDoMax = 70f,  // nhanh như tàn lửa
            kichThuocMin = 1f, kichThuocMax = 5f,
            doMoMin = 0.05f, doMoMax = 0.20f,
            huongDiChuyen = new Vector2(0.15f, 1f)
        },
        // Chapter 4 (idx 4) — xanh vũ trụ, hạt sao trôi chậm
        new CauHinhChuong {
            mauNen = new Color(0.031f, 0.063f, 0.102f),
            mauPhanTu = new Color[] {
                new Color(0.78f, 0.85f, 0.91f, 1f),
                new Color(0.38f, 0.65f, 0.98f, 1f),
                new Color(0.51f, 0.44f, 0.98f, 1f),
            },
            tocDoMin = 3f, tocDoMax = 12f,  // chậm như trôi trong vũ trụ
            kichThuocMin = 1f, kichThuocMax = 4f,
            doMoMin = 0.02f, doMoMax = 0.10f,
            huongDiChuyen = new Vector2(0.05f, 1f)
        },
    };

    // Background image
    private Image bgImage;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Tạo background image
        GameObject bgGO = new GameObject("BG_Color");
        bgGO.transform.SetParent(transform, false);
        RectTransform bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgImage = bgGO.AddComponent<Image>();
        bgGO.transform.SetSiblingIndex(0);

        // Tạo các phần tử
        cacPhanTu = new GameObject[soLuongPhanTu];
        cacImage = new Image[soLuongPhanTu];
        cacVanToc = new Vector2[soLuongPhanTu];
        cacPha = new float[soLuongPhanTu];

        for (int i = 0; i < soLuongPhanTu; i++)
        {
            GameObject go = new GameObject("Phan_Tu_" + i);
            go.transform.SetParent(transform, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            cacImage[i] = go.AddComponent<Image>();
            cacPha[i] = Random.Range(0f, Mathf.PI * 2f);

            rt.anchoredPosition = new Vector2(
                Random.Range(-960f, 960f),
                Random.Range(-540f, 540f));
            cacPhanTu[i] = go;
        }

        // Áp theme mặc định Ch1
        ApTheme(1, true);
    }

    void Update()
    {
        if (cacPhanTu == null) return;

        var ch = cauHinhCacChuong[chuongHienTai];
        float time = Time.time;

        for (int i = 0; i < soLuongPhanTu; i++)
        {
            RectTransform rt = cacPhanTu[i].GetComponent<RectTransform>();
            Vector2 pos = rt.anchoredPosition;

            // Di chuyển theo hướng + lắc nhẹ
            pos += cacVanToc[i] * Time.deltaTime;
            pos.x += Mathf.Sin(time * 0.4f + cacPha[i]) * 0.4f;

            // Reset khi ra ngoài màn
            if (pos.y > 580f)
            {
                pos.y = -580f;
                pos.x = Random.Range(-960f, 960f);
            }
            if (pos.x > 1000f) pos.x = -1000f;
            if (pos.x < -1000f) pos.x = 1000f;

            rt.anchoredPosition = pos;

            // Twinkle
            float doMo = Mathf.Lerp(ch.doMoMin, ch.doMoMax,
                (Mathf.Sin(time * 1.2f + cacPha[i]) + 1f) * 0.5f);
            Color c = cacImage[i].color;
            c.a = doMo;
            cacImage[i].color = c;
        }
    }

    // =============================================================
    // ĐỔI THEME CHƯƠNG
    // =============================================================
    public void DoiChuong(int idx)
    {
        if (idx < 0 || idx >= cauHinhCacChuong.Length) return;
        chuongHienTai = idx;
        ApTheme(idx, false);
    }

    void ApTheme(int idx, bool tucThi)
    {
        var ch = cauHinhCacChuong[idx];

        // Đổi màu nền
        if (bgImage != null)
            bgImage.color = ch.mauNen;

        // Reset các phần tử với cấu hình mới
        for (int i = 0; i < soLuongPhanTu; i++)
        {
            RectTransform rt = cacPhanTu[i].GetComponent<RectTransform>();

            // Kích thước mới
            float size = Random.Range(ch.kichThuocMin, ch.kichThuocMax);
            rt.sizeDelta = new Vector2(size, size);

            // Màu ngẫu nhiên từ palette chương
            Color mauChon = ch.mauPhanTu[Random.Range(0, ch.mauPhanTu.Length)];
            float doMo = Random.Range(ch.doMoMin, ch.doMoMax);
            cacImage[i].color = new Color(
                mauChon.r, mauChon.g, mauChon.b, doMo);

            // Vận tốc theo hướng chương
            float tocDo = Random.Range(ch.tocDoMin, ch.tocDoMax);
            Vector2 huong = ch.huongDiChuyen.normalized;
            cacVanToc[i] = huong * tocDo;

            // Reset vị trí nếu đổi ngay lập tức
            if (tucThi)
            {
                rt.anchoredPosition = new Vector2(
                    Random.Range(-960f, 960f),
                    Random.Range(-540f, 540f));
            }
        }
    }
}