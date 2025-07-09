// Assets/Scripts/GameManager/GameManager.cs
using Common;                           // KeyColor enum
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Life / Gauge")]
    [Tooltip("최대 목숨 개수")]
    public int maxLife = 3;
    [Tooltip("현재 목숨 개수")]
    public int life = 3;

    [Tooltip("1분(60초) 동안 0→1.0 으로 채우려면 60")]
    public float gaugeFillDuration = 60f;

    [Range(0f, 1f)]
    [Tooltip("현재 게이지(0~1)")]
    public float currentGauge = 0f;
    [Tooltip("숨고르기 발동에 필요한 게이지(1.0)")]
    public float maxGauge = 1f;

    [Header("현재 들고 있는 키 색상")]
    public KeyColor CurrentKey { get; private set; } = KeyColor.None;

    private UIManager uiManager;

    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        // 게이지 자동 충전
        if (currentGauge < maxGauge)
            AddGauge(Time.deltaTime / gaugeFillDuration);
    }

    #region Life

    /// <summary>
    /// 목숨을 줄입니다. life가 0 이하일 때만 게임오버 씬으로 전환합니다.
    /// </summary>
    public void LoseLife(int amount = 1)
    {
        life -= amount;
        Debug.Log($"Life –{amount} ▶ 남은 목숨: {life}");

        // UI 업데이트
        uiManager?.UpdateLife(life);

        // 목숨이 다 떨어졌을 때만 게임오버
        if (life <= 0)
        {
            Debug.Log("GAME OVER");
            StartCoroutine(DoGameOver());
        }
    }

    private System.Collections.IEnumerator DoGameOver()
    {
        // 한 프레임 대기해서 UI(0 라이프)가 반영된 후 씬 전환
        yield return null;
        SceneManager.LoadScene("GameOver");
    }

    #endregion

    #region Gauge (숨고르기)

    /// <summary>
    /// 게이지를 delta만큼 채우고 UI 갱신
    /// </summary>
    public void AddGauge(float delta)
    {
        currentGauge = Mathf.Clamp01(currentGauge + delta);
        uiManager?.UpdateGauge(currentGauge);
    }

    /// <summary>
    /// 게이지가 maxGauge에 도달하면 모든 적 제거 + 게이지 리셋
    /// </summary>
    public void PurgeAllEnemies()
    {
        if (currentGauge < maxGauge)
        {
            Debug.Log("게이지가 가득 차지 않았습니다!");
            return;
        }

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
            Destroy(e);

        Debug.Log($"숨고르기 발동! 적 {enemies.Length}마리 제거.");
        currentGauge = 0f;
        uiManager?.UpdateGauge(0f);
    }

    #endregion

    #region Key 로직

    public void PickKey(KeyColor color)
    {
        CurrentKey = color;
        Debug.Log($"🔑 Key Picked: {color}");
        // (필요시 UIManager 업데이트)
    }

    public void ConsumeKey()
    {
        CurrentKey = KeyColor.None;
        // (필요시 UIManager 업데이트)
    }

    #endregion
}
