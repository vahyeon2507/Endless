// Assets/Scripts/GameManager/GameManager.cs
using UnityEngine;
using Common;            // ← KeyColor enum
// using static Firewall; // ← 삭제!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Life / Gauge")]
    public int maxLife = 3;
    public int life = 3;

    [Range(0f, 1f)]
    public float currentGauge = 0f;
    public float maxGauge = 1f;

    private UIManager uiManager;

    [Header("현재 들고 있는 키 색상")]
    public KeyColor CurrentKey { get; private set; } = KeyColor.None;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        uiManager = FindObjectOfType<UIManager>();
    }

    #region Life
    public void LoseLife(int amount = 1)
    {
        life -= amount;
        Debug.Log($"Life –{amount} ▶ 남은 목숨: {life}");
        if (life <= 0) Debug.Log("GAME OVER");

        uiManager?.UpdateLife(life);
    }
    #endregion

    #region Gauge (숨고르기)
    public void AddGauge(float delta)
    {
        currentGauge = Mathf.Clamp01(currentGauge + delta);
        uiManager?.UpdateGauge(currentGauge);
    }

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
        // UI 업데이트 로직이 있다면 여기서 호출
    }

    public void ConsumeKey()
    {
        CurrentKey = KeyColor.None;
        // UI 업데이트가 필요하면 여기서 호출
    }
    #endregion
}
