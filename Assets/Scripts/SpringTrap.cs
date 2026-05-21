using UnityEngine;

public class SpringTrap : MonoBehaviour
{
    [Header("=== LỰC BẮN ===")]
    public float lucBan = 25f;

    [Header("=== ANIMATION ===")]
    public float thoiGianNen = 0.05f;
    public float thoiGianGian = 0.15f;

    [Header("=== SFX ===")]
    public bool phatSFX = true;

    private Animator animator;
    private bool dangNhay = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (dangNhay) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        if (rb.linearVelocity.y > 2f) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null) pc.NayLoXo(lucBan);

        if (phatSFX && SoundManager.instance != null)
            SoundManager.instance.PlayNhay();

        dangNhay = true;

        // Chuyển sang frame nén
        if (animator != null)
            animator.SetBool("IsPressed", true);

        StartCoroutine(AnimationLoXo());
    }

    System.Collections.IEnumerator AnimationLoXo()
    {
        // Giữ frame nén
        yield return new WaitForSeconds(thoiGianNen);

        // Trở về frame bình thường
        if (animator != null)
            animator.SetBool("IsPressed", false);

        yield return new WaitForSeconds(thoiGianGian + 0.3f);
        dangNhay = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,
            transform.position + Vector3.up * (lucBan * 0.1f));
    }
}