using UnityEngine;
using System.Collections;

public class DeathEffect : MonoBehaviour
{
    [Header("=== SCREEN SHAKE ===")]
    public float doDungCamera = 0.15f;
    public float thoiGianRung = 0.2f;

    [Header("=== FLASH ĐỎ ===")]
    public float thoiGianFlash = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color mauGoc;
    private PlayerController playerController;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mauGoc = spriteRenderer.color;
        playerController = GetComponent<PlayerController>();
    }

    public void KichHoatHieuUng()
    {
        StartCoroutine(ChayHieuUng());
    }

    IEnumerator ChayHieuUng()
    {
        // Pause nhân vật ngay lập tức
        if (playerController != null)
            playerController.enabled = false;

        // SFX chết
        if (SoundManager.instance != null)
            SoundManager.instance.PlayChet();

        // Flash đỏ
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(thoiGianFlash);
        spriteRenderer.color = mauGoc;

        // Screen shake
        StartCoroutine(RungCamera());
        yield return new WaitForSeconds(thoiGianRung);

        // PlayerController sẽ tự bật lại khi scene reload
        GameManager.instance.PlayerChet();
    }

    IEnumerator RungCamera()
    {
        Camera cam = Camera.main;
        Vector3 viTriGoc = cam.transform.position;
        float thoiGianDaRung = 0f;

        while (thoiGianDaRung < thoiGianRung)
        {
            float x = viTriGoc.x + Random.Range(-doDungCamera, doDungCamera);
            float y = viTriGoc.y + Random.Range(-doDungCamera, doDungCamera);
            cam.transform.position = new Vector3(x, y, viTriGoc.z);
            thoiGianDaRung += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = viTriGoc;
    }
}