using UnityEngine;
using System.Collections;

public class FallingBrick : MonoBehaviour
{
    public enum CheDoSauKhiRoi
    {
        HoiSinh,
        BienMat
    }

    [Header("=== THỜI GIAN ===")]
    public float thoiGianChoTruocKhiRoi = 0.3f;

    [Header("=== SAU KHI RƠI ===")]
    public CheDoSauKhiRoi cheDoSauKhiRoi = CheDoSauKhiRoi.HoiSinh;
    public float thoiGianHoiSinh = 3f;

    [Header("=== ROI XUYÊN MAP ===")]
    public float tocDoRoi = 20f;
    public float khoangRoiTruocKhiBienMat = 15f;

    [HideInInspector] public bool dangRoi = false;

    private Rigidbody2D rb;
    private Vector3 viTriGoc;
    private BoxCollider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        viTriGoc = transform.position;
    }

    public void KichHoatRoi()
    {
        if (!dangRoi)
        {
            dangRoi = true;
            StartCoroutine(DemGioRoi());
        }
    }

    IEnumerator DemGioRoi()
    {
        if (thoiGianChoTruocKhiRoi > 0)
        {
            float daRung = 0f;
            while (daRung < thoiGianChoTruocKhiRoi)
            {
                float offsetX = Random.Range(-0.05f, 0.05f);
                transform.position = viTriGoc + new Vector3(offsetX, 0, 0);
                daRung += Time.deltaTime;
                yield return null;
            }
            transform.position = viTriGoc;
        }

        col.enabled = false;

        float diaDiem = viTriGoc.y - khoangRoiTruocKhiBienMat;
        while (transform.position.y > diaDiem)
        {
            transform.position += Vector3.down * tocDoRoi * Time.deltaTime;
            yield return null;
        }

        sr.enabled = false;

        if (cheDoSauKhiRoi == CheDoSauKhiRoi.BienMat)
        {
            Destroy(gameObject);
            yield break;
        }

        yield return new WaitForSeconds(thoiGianHoiSinh);

        transform.position = viTriGoc;
        col.enabled = true;
        sr.enabled = true;
        dangRoi = false;
    }
}