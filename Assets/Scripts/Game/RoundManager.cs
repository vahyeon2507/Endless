using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public int currentRound = 1;        // 현재 라운드 (1~3)
    public float roundDuration = 180f;  // 한 라운드 = 3분
    private float timer;

    public Text hudText;               // HUD 텍스트 (라운드 정보만 표시)

    private void Start()
    {
        timer = roundDuration;
        UpdateHUD();  // HUD 초기 출력
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Debug.Log("라운드 종료");

            if (currentRound >= 3)
            {
                Debug.Log("모든 라운드 클리어! 🎉");
                // 게임 종료 처리 등 나중에 추가
                enabled = false;
                return;
            }

            currentRound++;
            timer = roundDuration;
            UpdateHUD();
        }
    }

    void UpdateHUD()
    {
        hudText.text = $"Round {currentRound} - ??? 유기체";
        // 이후 여기서 라운드별 등장 유기체 이름 출력 가능
    }
}
