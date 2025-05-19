using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Image gaugeFill;  // 배터리 UI 이미지 연결 예정

    public void UpdateLife(int life)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < life) ? fullHeart : emptyHeart;
        }
    }

    public void UpdateGauge(float percent)
    {
        gaugeFill.fillAmount = Mathf.Clamp01(percent);
    }
}
