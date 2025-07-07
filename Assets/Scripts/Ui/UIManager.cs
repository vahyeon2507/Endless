using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Life HUD")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Gauge & Purge Button Prompt")]
    public Image gaugeFill;         // 배터리/게이지 UI
    public GameObject purgePrompt;  // "Press SPACE to Purge!" 같은 안내 텍스트나 아이콘

    void Start()
    {
        // 시작할 때 프롬프트는 숨겨두기
        if (purgePrompt != null)
            purgePrompt.SetActive(false);
    }

    /// <summary>
    /// 목숨(하트) UI 갱신
    /// </summary>
    public void UpdateLife(int life)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < life) ? fullHeart : emptyHeart;
        }
    }



    /// <summary>
    /// 게이지 UI 업데이트 및
    /// 가득 차면 Purge 안내 프롬프트 활성화
    /// </summary>
    public void UpdateGauge(float percent)
    {
        float p = Mathf.Clamp01(percent);
        gaugeFill.fillAmount = p;

        if (purgePrompt != null)
            purgePrompt.SetActive(p >= 1f);
    }
}
