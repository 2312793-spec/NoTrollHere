using UnityEngine;
using System.Collections.Generic;

public class SkinManager : MonoBehaviour
{
    public static SkinManager instance;

    // =============================================================
    // ĐỊNH NGHĨA SKIN
    // =============================================================
    public class Skin
    {
        public string id;
        public string ten;
        public string moTa;
        public string dieuKienMo;
        public bool daUnlock;

        public Skin(string id, string ten, string moTa, string dieuKienMo)
        {
            this.id = id;
            this.ten = ten;
            this.moTa = moTa;
            this.dieuKienMo = dieuKienMo;
            this.daUnlock = PlayerPrefs.GetInt("Skin_" + id + "_unlocked", 0) == 1;
        }
    }

    [System.Serializable]
    public class SkinSprite
    {
        public string id;
        public Sprite sprite; // frame đầu tiên — dùng làm preview
    }

    [System.Serializable]
    public class SkinAnimator
    {
        public string id;
        public RuntimeAnimatorController controller;
        // Default dùng AnimatorController gốc
        // Skin khác dùng AnimatorOverrideController
    }

    [Header("=== SPRITES (preview) ===")]
    public List<SkinSprite> cacSkinSprite = new List<SkinSprite>();

    [Header("=== ANIMATORS ===")]
    public List<SkinAnimator> cacSkinAnimator = new List<SkinAnimator>();

    private List<Skin> danhSachSkin = new List<Skin>();
    private string skinDangDung = "Default";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            KhoiTaoDanhSach();
        }
        else Destroy(gameObject);
    }

    void KhoiTaoDanhSach()
    {
        danhSachSkin.Clear();

        // Tất cả skin mở từ đầu — khóa lại sau khi gắn achievement
        ThemSkin("Default", "Default", "Nhân vật chalk nguyên bản", "Mặc định", true);
        ThemSkin("Ghost", "Ghost", "Trong suốt như... ma", "Chết 100 lần", true);
        ThemSkin("Forest", "Forest", "Hòa mình vào rừng xanh", "Hoàn thành Chapter 2", true);
        ThemSkin("Ember", "Ember", "Tôi luyện qua lửa", "Hoàn thành Chapter 3", true);
        ThemSkin("Void", "Void", "Từ vực thẳm trở về", "Hoàn thành Chapter 4", true);
        ThemSkin("Skeleton", "Skeleton", "Chỉ còn lại xương", "Chết 500 lần", true);
        ThemSkin("Golden", "Golden", "Huyền thoại thực sự", "Hoàn thành tất cả", true);
        ThemSkin("IceMan", "Ice", "Băng giá vĩnh cửu", "Hoàn thành Chapter 4", true);
        ThemSkin("Lava", "Lava", "Dung nham sục sôi", "Hoàn thành Chapter 3", true);
        ThemSkin("Robot", "Robot", "Kim loại lạnh lùng", "Qua màn không chết", true);

        skinDangDung = PlayerPrefs.GetString("Skin_DangDung", "Default");
    }

    void ThemSkin(string id, string ten, string moTa,
        string dieuKien, bool moNgay)
    {
        var skin = new Skin(id, ten, moTa, dieuKien);
        if (moNgay)
        {
            skin.daUnlock = true;
            PlayerPrefs.SetInt("Skin_" + id + "_unlocked", 1);
        }
        danhSachSkin.Add(skin);
    }

    // =============================================================
    // LẤY SPRITE PREVIEW
    // =============================================================
    public Sprite LaySpriteSkin(string id)
    {
        SkinSprite ss = cacSkinSprite.Find(s => s.id == id);
        if (ss != null && ss.sprite != null) return ss.sprite;

        // Fallback Default
        SkinSprite def = cacSkinSprite.Find(s => s.id == "Default");
        return def?.sprite;
    }

    // =============================================================
    // CHỌN SKIN
    // =============================================================
    public void ChonSkin(string id)
    {
        Skin skin = danhSachSkin.Find(s => s.id == id);
        if (skin == null || !skin.daUnlock) return;

        skinDangDung = id;
        PlayerPrefs.SetString("Skin_DangDung", id);
        PlayerPrefs.Save();

        ApSkinLenPlayer();
        Debug.Log($"Đang dùng skin: {id}");
    }

    // =============================================================
    // ÁP SKIN LÊN PLAYER
    // =============================================================
    public void ApSkinLenPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Đổi sprite frame đầu
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        Sprite sprite = LaySpriteSkin(skinDangDung);
        if (sr != null && sprite != null)
            sr.sprite = sprite;

        // Đổi Animator Controller
        Animator anim = player.GetComponent<Animator>();
        if (anim == null) return;

        SkinAnimator sa = cacSkinAnimator.Find(s => s.id == skinDangDung);
        if (sa != null && sa.controller != null)
        {
            anim.runtimeAnimatorController = sa.controller;
        }
        else
        {
            // Fallback về Default
            SkinAnimator def = cacSkinAnimator.Find(s => s.id == "Default");
            if (def != null && def.controller != null)
                anim.runtimeAnimatorController = def.controller;
        }
    }

    public string LaySkinDangDung() => skinDangDung;
    public List<Skin> LayDanhSach() => danhSachSkin;

    // =============================================================
    // MỞ SKIN — gọi từ AchievementManager
    // =============================================================
    public void MoSkin(string id)
    {
        Skin skin = danhSachSkin.Find(s => s.id == id);
        if (skin == null || skin.daUnlock) return;

        skin.daUnlock = true;
        PlayerPrefs.SetInt("Skin_" + id + "_unlocked", 1);
        PlayerPrefs.Save();
        Debug.Log($"🎨 Mở skin: {id}");
    }
}