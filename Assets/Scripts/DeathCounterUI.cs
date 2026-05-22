using UnityEngine;
using TMPro;

public class DeathCounterUI : MonoBehaviour
{
    public TextMeshProUGUI deathText;

    void Update()
    {
        if (GameManager.instance == null) return;
        // Chỉ tính số chết màn hiện tại
        deathText.text = "DEATHS : " + GameManager.instance.LaySoLanChet();
    }
}