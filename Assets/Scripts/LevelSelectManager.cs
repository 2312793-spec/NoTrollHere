using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("=== UI THAM CHIẾU ===")]
    public TextMeshProUGUI txtTenChuong;
    public TextMeshProUGUI txtMoTaChuong;
    public GameObject[] cacNutLevel;
    public Button nutTrai;
    public Button nutPhai;

    [Header("=== CÀI ĐẶT ===")]
    public string tenSceneMainMenu = "MainMenu";

    // =============================================================
    // THỨ TỰ CHƯƠNG: Special(0 trái) ← Chapter1(1 giữa) → Chapter2(2) → Chapter3(3)...
    // Index 0 = Special, Index 1 = Ch1, Index 2 = Ch2, ...
    // =============================================================
    private readonly string[] tenCacChuong =
    {
        "SPECIAL",      // 0 — trái nhất
        "CHAPTER 1",    // 1 — mặc định khi mở
        "CHAPTER 2",    // 2
        "CHAPTER 3",    // 3
        "CHAPTER 4",    // 4
    };

    private readonly string[] moTaCacChuong =
    {
        "something else entirely",
        "choose your suffering",
        "into the forest",
        "the volcano awaits",
        "into the abyss",
    };

    private readonly string[] prefixCacChuong =
    {
        "Level_S_",
        "Level_1_",
        "Level_2_",
        "Level_3_",
        "Level_4_",
    };

    private readonly int[] soManCacChuong =
    {
        1,   // Special: 1 màn tutorial
        10,  // Chapter 1
        10,  // Chapter 2
        10,  // Chapter 3
        10,  // Chapter 4
    };

    private readonly Color[] mauAccentChuong =
    {
        new Color(0.75f, 0.52f, 0.99f, 1f), // Special: tím #C084FC
        new Color(0.96f, 0.78f, 0.26f, 1f), // Ch1: vàng #F5C842
        new Color(0.29f, 0.87f, 0.50f, 1f), // Ch2: xanh lá #4ADE80
        new Color(0.98f, 0.57f, 0.24f, 1f), // Ch3: cam #FB923C
        new Color(0.38f, 0.65f, 0.98f, 1f), // Ch4: xanh dương #60A5FA
    };

    private readonly Color[] mauNenDangChoiChuong =
    {
        new Color(0.10f, 0.05f, 0.15f, 1f), // Special: tím tối
        new Color(0.16f, 0.13f, 0.00f, 1f), // Ch1: vàng tối
        new Color(0.05f, 0.13f, 0.07f, 1f), // Ch2: xanh tối
        new Color(0.15f, 0.06f, 0.02f, 1f), // Ch3: cam tối
        new Color(0.03f, 0.08f, 0.15f, 1f), // Ch4: xanh dương tối
    };

    // Màu chung
    private readonly Color mauNenDaMo = new Color(0.18f, 0.18f, 0.18f, 1f);
    private readonly Color mauNenLock = new Color(0.11f, 0.11f, 0.11f, 1f);
    private readonly Color mauVienDaMo = new Color(0.23f, 0.23f, 0.23f, 1f);
    private readonly Color mauVienLock = new Color(0.14f, 0.14f, 0.14f, 1f);
    private readonly Color mauChuDaMo = new Color(0.91f, 0.91f, 0.82f, 1f);
    private readonly Color mauChuLock = new Color(0.20f, 0.20f, 0.20f, 1f);

    // Index 1 = Chapter 1 = mặc định khi mở
    private int chuongHienTai = 1;
    private string levelMoiNhat = "Level_1_1";

    void Start()
    {
        if (GameManager.instance != null)
            levelMoiNhat = GameManager.instance.LayLevelMoiNhat();
        else
            levelMoiNhat = PlayerPrefs.GetString("LatestLevel", "Level_1_1");

        // Luôn mở ở Chapter 1 (index 1) khi vào Level Select
        chuongHienTai = 1;
        HienThiChuong(chuongHienTai);
    }

    // =============================================================
    // HIỂN THỊ CHƯƠNG
    // =============================================================
    void HienThiChuong(int idx)
    {
        if (ChapterBackground.instance != null)
            ChapterBackground.instance.DoiChuong(idx);

        bool duocXem = KiemTraDuocXem(idx);

        txtTenChuong.text = "— " + tenCacChuong[idx] + " —";
        txtMoTaChuong.text = duocXem ? moTaCacChuong[idx] : "???";
        txtTenChuong.color = duocXem ? mauAccentChuong[idx] : mauChuLock;

        CapNhatMuiTen(idx);

        string prefix = prefixCacChuong[idx];
        int soMan = soManCacChuong[idx];

        for (int i = 0; i < cacNutLevel.Length; i++)
        {
            bool hienThi = i < soMan;
            cacNutLevel[i].SetActive(hienThi);
            if (!hienThi) continue;

            int soThuTu = i + 1;
            string tenLevel = prefix + soThuTu;

            bool daUnlock = duocXem && KiemTraUnlock(tenLevel);
            bool daPassed = PlayerPrefs.GetInt(tenLevel + "_passed", 0) == 1;
            bool laMoiNhat = tenLevel == levelMoiNhat;

            Image img = cacNutLevel[i].GetComponent<Image>();
            Outline outline = cacNutLevel[i].GetComponent<Outline>();
            Button btn = cacNutLevel[i].GetComponent<Button>();
            TextMeshProUGUI txtSo = cacNutLevel[i]
                .transform.Find("Txt_So")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI txtIcon = cacNutLevel[i]
                .transform.Find("Txt_Icon")?.GetComponent<TextMeshProUGUI>();

            Color accent = mauAccentChuong[idx];
            Color nenActive = mauNenDangChoiChuong[idx];

            if (!daUnlock)
            {
                // LOCKED — không có icon
                SetNut(img, outline, btn, txtSo, txtIcon,
                    mauNenLock, mauVienLock, mauChuLock,
                    soThuTu.ToString(), "", false);
            }
            else if (daPassed)
            {
                // ĐÃ PASS — có dấu *
                SetNut(img, outline, btn, txtSo, txtIcon,
                    mauNenDaMo, mauVienDaMo, mauChuDaMo,
                    soThuTu.ToString(), "*", true);
                GanOnClick(btn, tenLevel);
            }
            else if (laMoiNhat)
            {
                // ĐANG CHƠI — không có icon
                SetNut(img, outline, btn, txtSo, txtIcon,
                    nenActive, accent, accent,
                    soThuTu.ToString(), "", true);
                GanOnClick(btn, tenLevel);
            }
            else
            {
                // UNLOCK CHƯA CHƠI — không có icon
                SetNut(img, outline, btn, txtSo, txtIcon,
                    mauNenDaMo, mauVienDaMo, mauChuDaMo,
                    soThuTu.ToString(), "", true);
                GanOnClick(btn, tenLevel);
            }
        }
    }

    // =============================================================
    // HELPERS
    // =============================================================
    void SetNut(Image img, Outline outline, Button btn,
        TextMeshProUGUI txtSo, TextMeshProUGUI txtIcon,
        Color mauNen, Color mauVien, Color mauChu,
        string so, string icon, bool tuongTac)
    {
        if (img != null) img.color = mauNen;
        if (outline != null) outline.effectColor = mauVien;
        btn.interactable = tuongTac;
        if (txtSo != null) { txtSo.text = so; txtSo.color = mauChu; }
        if (txtIcon != null) { txtIcon.text = icon; txtIcon.color = mauChu; }
    }

    void GanOnClick(Button btn, string tenLevel)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => SceneManager.LoadScene(tenLevel));
    }

    void CapNhatMuiTen(int idx)
    {
        // Trái — về Special (idx 0) hoặc các chương trước
        if (nutTrai != null)
        {
            bool coTrai = idx > 0;
            nutTrai.interactable = coTrai;
            CanvasGroup cg = nutTrai.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = coTrai ? 1f : 0.25f;
        }

        // Phải — sang chương sau (chỉ nếu đã unlock)
        if (nutPhai != null)
        {
            bool coTiep = idx < tenCacChuong.Length - 1;
            bool tiepDaUnlock = coTiep && KiemTraDuocXem(idx + 1);
            nutPhai.interactable = coTiep;
            CanvasGroup cg = nutPhai.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = tiepDaUnlock ? 1f : 0.25f;
        }
    }

    // =============================================================
    // KIỂM TRA UNLOCK
    // =============================================================
    bool KiemTraDuocXem(int idx)
    {
        // Chapter 1 luôn xem được
        if (idx == 1) return true;

        // Special (idx 0) — unlock sau khi pass Ch1 màn 1-9
        if (idx == 0)
        {
            if (GameManager.instance != null)
                return GameManager.instance.LaChapterSpecialDaMo();
            return PlayerPrefs.GetInt("Chapter_Special_unlocked", 0) == 1;
        }

        // Chapter 2, 3, 4... — unlock sau khi pass màn 10 chương trước
        // idx=2 → Ch2 → cần pass Level_1_10
        // idx=3 → Ch3 → cần pass Level_2_10
        // idx=4 → Ch4 → cần pass Level_3_10
        int chuongTruoc = idx - 1; // chapter number (1-based)
        string keyMan10 = $"Level_{chuongTruoc}_10_passed";
        return PlayerPrefs.GetInt(keyMan10, 0) == 1;
    }

    bool KiemTraUnlock(string tenLevel)
    {
        if (GameManager.instance != null)
            return GameManager.instance.LaLevelDaUnlock(tenLevel);
        return PlayerPrefs.GetInt(tenLevel + "_unlocked", 0) == 1;
    }

    // =============================================================
    // NÚT MŨI TÊN
    // =============================================================
    public void NutTrai()
    {
        if (chuongHienTai > 0)
        {
            chuongHienTai--;
            HienThiChuong(chuongHienTai);
        }
    }

    public void NutPhai()
    {
        if (chuongHienTai < tenCacChuong.Length - 1)
        {
            chuongHienTai++;
            HienThiChuong(chuongHienTai);
        }
    }

    public void NutBack()
    {
        SceneManager.LoadScene(tenSceneMainMenu);
    }
}