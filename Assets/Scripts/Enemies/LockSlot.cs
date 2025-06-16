using UnityEngine;

public class LockSlot : MonoBehaviour
{
    [Tooltip("이 구멍의 ID (KeyPickup.keyID 와 비교)")]
    public int slotID;

    private Firewall firewall;

    void Awake()
    {
        firewall = GetComponentInParent<Firewall>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        firewall.TryUnlock(slotID);
    }
}
