// Assets/Scripts/Entities/KeyPickup.cs
using UnityEngine;
using static Firewall;

[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    public KeyColor keyColor;                  // 이 열쇠의 색
    public float bobAmplitude = 0.1f;          // 둥실둥실 효과
    public float bobSpeed = 3f;

    Vector3 startPos;

    void Start() => startPos = transform.position;

    void Update()
    {
        // 살짝 위아래 둥실둥실
        float yOff = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.position = startPos + Vector3.up * yOff;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        GameManager.Instance.PickKey(keyColor);
        Destroy(gameObject);
    }
}
