using UnityEngine;

public class Firewall : MonoBehaviour
{
    [Tooltip("방화벽이 스폰될 때까지 기다리는 시간")]
    public float appearDelay = 1f;
    [Tooltip("방화벽이 생성될 위치")]
    public Vector3 spawnPosition;

    private bool isActive = false;
    private LockSlot[] slots;

    int playerKey = GameManager.Instance.CurrentKey;

    void Start()
    {
        transform.position = spawnPosition;
        // 처음엔 눈에 안 보이게
        foreach (var rend in GetComponentsInChildren<SpriteRenderer>())
            rend.enabled = false;

        slots = GetComponentsInChildren<LockSlot>();
        Invoke(nameof(ActivateFirewall), appearDelay);
    }

    void ActivateFirewall()
    {
        // 화면에 보이게
        foreach (var rend in GetComponentsInChildren<SpriteRenderer>())
            rend.enabled = true;

        isActive = true;
    }

    /// <summary>
    /// 플레이어가 slotID 구멍에 들어갔을 때 호출
    /// </summary>
    public void TryUnlock(int slotID)
    {
        if (!isActive) return;

        int playerKey = GameManager.Instance.CurrentKey;
        if (playerKey == slotID)
        {
            Debug.Log("방화벽 해제 성공!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("방화벽 충돌! 목숨 감소");
            GameManager.Instance.LoseLife(1);
            Destroy(gameObject);
        }
        // 사용한 키 초기화
        GameManager.Instance.CurrentKey = -1;
    }
}
