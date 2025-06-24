// Assets/Scripts/Enemies/Firewall.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Firewall : MonoBehaviour
{
    [Header("세팅")]
    public KeyColor doorColor;            // 요구 열쇠 색
    public float dropDelay = 1.0f;    // 열쇠 먹고 문이 내려오기 전
    public float fallSpeed = 3.0f;    // 낙하 속도
    public float stopY = 2.5f;    // 플레이 구역 상단 y (도착지)

    [Header("시각")]
    public SpriteRenderer sr;
    public Color redTint = new(0.90f, 0.15f, 0.15f);
    public Color blueTint = new(0.25f, 0.45f, 1.00f);
    public Color yellowTint = new(1.00f, 0.85f, 0.10f);

    bool isDropping;

    void Awake()
    {
        // 문 색상 표시
        sr ??= GetComponent<SpriteRenderer>();
        sr.color = doorColor switch
        {
            KeyColor.Red => redTint,
            KeyColor.Blue => blueTint,
            KeyColor.Yellow => yellowTint,
            _ => sr.color
        };
    }

    void Start() => StartCoroutine(DropRoutine());

    IEnumerator DropRoutine()
    {
        // 플레이어가 올바른 키를 들고 올 때까지 대기
        while (GameManager.Instance.CurrentKey != doorColor)
            yield return null;

        yield return new WaitForSeconds(dropDelay);
        isDropping = true;
    }

    void Update()
    {
        if (!isDropping) return;

        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        if (transform.position.y <= stopY)
        {
            isDropping = false;
            // 통과 후에는 더 이상 충돌 처리하지 않도록 트리거 켬
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        // 키가 맞으면 통과 & 키 소모 / 틀리면 체력 감소
        if (GameManager.Instance.CurrentKey == doorColor)
        {
            Debug.Log("Firewall Pass!");
            GameManager.Instance.ConsumeKey();
        }
        else
        {
            Debug.Log("Wrong Key ▶ Life -1");
            GameManager.Instance.LoseLife(1);
        }

        Destroy(gameObject);
    }
}
