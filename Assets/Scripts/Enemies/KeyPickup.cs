using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Tooltip("0=빨강, 1=파랑, 2=노랑 등 키 색 인덱스")]
    public int keyID;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"키 획득: {keyID}");
        GameManager.Instance.PickupKey(keyID);
        Destroy(gameObject);
    }
}
