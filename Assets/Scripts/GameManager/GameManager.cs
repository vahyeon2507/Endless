// Assets/Scripts/GameManager/GameManager.cs
using UnityEngine;
using static Firewall;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Life / Gauge")]
    public int maxLife = 3;
    public int life = 3;

    private UIManager uiManager;


    [Header("현재 들고 있는 키 색상")]
    public KeyColor CurrentKey { get; private set; } = KeyColor.None;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        uiManager = FindObjectOfType<UIManager>();
    }

    #region Life / Gauge
    public void LoseLife(int amount = 1)
    {
        life -= amount;
        Debug.Log($"Life –{amount} ▶ 남은 목숨: {life}");
        if (life <= 0) Debug.Log("GAME OVER");
        // TODO: UIManager.UpdateLife(life);

        if (uiManager != null)
            uiManager.UpdateLife(life);
        else
            Debug.LogWarning("UIManager 참조가 없습니다!");
    }
    #endregion

    #region Key 로직
    public void PickKey(KeyColor color)
    {
        CurrentKey = color;
        Debug.Log($"🔑 Key Picked: {color}");
        // HUD에 열쇠 표시가 있다면 여기서 업데이트
        // RoundManager 같은 애가 문들에게 브로드캐스트할 수도 있음
    }

    public void ConsumeKey() => CurrentKey = KeyColor.None;
    #endregion
}
