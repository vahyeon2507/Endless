using System.Collections;
using UnityEngine;

public class ShuffleCard : MonoBehaviour
{
    public SpriteRenderer[] cards;     // 카드 3장의 SpriteRenderer
    public Transform[] positions;      // 3개 레인 위치
    public float shuffleDuration = 2f; // 셔플 총 시간
    public int shuffleCount = 10;      // 셔플 횟수

    private Collider2D[] colliders;    // 카드마다 붙은 Collider2D 모아두기
    private int correctIndex;

    void Awake()
    {
        // 모든 자식 카드의 Collider2D 컴포넌트 수집
        colliders = GetComponentsInChildren<Collider2D>();
    }

    void Start()
    {
        // 시작 전에는 충돌 감지 끔
        EnableColliders(false);

        // 카드 초기 위치 세팅
        for (int i = 0; i < cards.Length; i++)
            cards[i].transform.position = positions[i].position;

        // 셔플 코루틴 시작
        StartCoroutine(DoShuffle());
    }

    IEnumerator DoShuffle()
    {
        float interval = shuffleDuration / shuffleCount;

        for (int i = 0; i < shuffleCount; i++)
        {
            int a = Random.Range(0, cards.Length);
            int b;
            do { b = Random.Range(0, cards.Length); }
            while (b == a);

            // 위치 스왑
            Vector3 tmp = cards[a].transform.position;
            cards[a].transform.position = cards[b].transform.position;
            cards[b].transform.position = tmp;

            yield return new WaitForSeconds(interval);
        }

        // EXIT 카드가 최종적으로 어느 인덱스에 있는지 계산
        for (int i = 0; i < cards.Length; i++)
            if (cards[i].sprite.name.ToLower().Contains("exit"))
                correctIndex = i;

        // 셔플 끝! 이제 충돌 감지 켜기
        EnableColliders(true);
    }

    void EnableColliders(bool on)
    {
        foreach (var col in colliders)
            col.enabled = on;
    }

    // 플레이어가 카드 충돌 영역에 들어올 때 실행
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 잘못된 레인에 있으면 목숨 감소
        float dx = Mathf.Abs(other.transform.position.x - positions[correctIndex].position.x);
        if (dx > 0.1f)
        {
            Debug.Log("셔플 실패! 목숨 감소");
            GameManager.Instance.LoseLife();
        }

        Destroy(gameObject);
    }
}
