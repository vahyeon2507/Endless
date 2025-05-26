using UnityEngine;
using UnityEngine.UI; // HUD가 Text라면, TextMeshProUGUI 쓴다면 using TMPro;

public class RoundManager : MonoBehaviour
{
    // 1) 싱글톤 인스턴스
    public static RoundManager Instance { get; private set; }

    [Header("라운드 설정")]
    public int startingRound = 1;
    public float roundDuration = 180f;     // 한 라운드 지속 시간(초)
    public float roundBreak = 2f;          // 라운드 간 휴식 시간(초)

    [Header("HUD")]
    public Text roundHud;                  // Text 대신 TMP 쓰면 TextMeshProUGUI

    // 프로퍼티
    public int CurrentRound { get; private set; }
    public bool IsRoundActive { get; private set; }

    private float timer;

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CurrentRound = startingRound;
        BeginRound();
    }

    void Update()
    {
        if (!IsRoundActive)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
            EndRound();
    }

    void BeginRound()
    {
        IsRoundActive = true;
        timer = roundDuration;
        UpdateHUD();
    }

    void EndRound()
    {
        IsRoundActive = false;
        Debug.Log($"Round {CurrentRound} 종료");

        if (CurrentRound < 3)
        {
            CurrentRound++;
            // 잠깐 휴식 후 다음 라운드 시작
            Invoke(nameof(BeginRound), roundBreak);
        }
        else
        {
            Debug.Log("게임 클리어!");
            // TODO: 클리어 화면 또는 다음 씬 로드
        }
    }

    void UpdateHUD()
    {
        if (roundHud != null)
            roundHud.text = $"Round {CurrentRound}";
    }
}
