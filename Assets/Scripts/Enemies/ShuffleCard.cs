using System.Collections;
using UnityEngine;

public class ShuffleCard : MonoBehaviour
{
    [Header("▶︎ 카드 SpriteRenderer 목록 (왼쪽, 중앙, 오른쪽 순서)")]
    public SpriteRenderer[] cards;     // 총 3장의 카드

    [Header("▶︎ 3개 레인 위치 (왼쪽, 중앙, 오른쪽 순서)")]
    public Transform[] positions;      // 각 레인의 고정 X 좌표를 가지는 Transform

    [Header("▶︎ 셔플 설정")]
    public float shuffleDuration = 2f; // 셔플이 총 몇 초 동안 진행될지
    public int shuffleCount = 10;      // 셔플 횟수 (Swap 횟수)

    [Header("▶︎ 낙하(Free-fall) 설정")]
    public float fallSpeed = 5f;       // 카드가 낙하할 때 속도
    public float fallTargetY = -3f;    // 카드가 떨어질 최종 Y 좌표 (플레이어 위치 바로 밑으로)

    // 내부 용도
    private Collider2D[] cardColliders; // 자식 카드 3장 각각에 붙은 Collider2D
    private int correctIndex;           // “EXIT 카드가 최종적으로 배치된 positions 인덱스”
    private bool isDropping = false;    // 카드들이 낙하 중인지
    private bool hasDropped = false;    // 한 번이라도 낙하가 끝났는지(킬존에 빠졌는지) 체크

    void Awake()
    {
        // 자식으로 붙어 있는 3개 카드에서 Collider2D 컴포넌트를 모두 수집
        cardColliders = GetComponentsInChildren<Collider2D>();
    }

    void Start()
    {
        // 0) 초기 상태: 충돌 검사(Trigger)는 꺼 놓는다
        EnableColliders(false);

        // 1) 카드 3장의 초기 위치: 씬에 배치해 둔 “positions[0..2]” (좌, 중, 우) 그대로
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].transform.position = positions[i].position;
        }

        // 2) 셔플 + 낙하 수행 코루틴 시작
        StartCoroutine(DoShuffleAndDrop());
    }

    /// <summary>
    /// 카드 셔플 → 위치 보정 → 충돌 활성화 → Free-fall(낙하) 루틴
    /// </summary>
    IEnumerator DoShuffleAndDrop()
    {
        float interval = shuffleDuration / shuffleCount;

        // === (1) 카드 내부 위치를 섞는다 (랜덤 Swap) ===
        for (int i = 0; i < shuffleCount; i++)
        {
            int a = Random.Range(0, cards.Length);
            int b;
            do { b = Random.Range(0, cards.Length); }
            while (b == a);

            // Swap a와 b 위치
            Vector3 tmp = cards[a].transform.position;
            cards[a].transform.position = cards[b].transform.position;
            cards[b].transform.position = tmp;

            yield return new WaitForSeconds(interval);
        }

        // === (2) 셔플 완료 후: 반드시 EXIT 카드가 한 레인, STOP 카드 두 개가 나머지 두 레인 → 정확히 3개 레인 중 1:1로 나누어 차지하도록 재배치 ===

        // (2-1) 우선 EXIT 카드(녹색 사람 모양)가 어느 cards[i] 인덱스인지 찾기
        int exitCardLocalIndex = -1;
        for (int i = 0; i < cards.Length; i++)
        {
            string lowerName = cards[i].sprite.name.ToLower();
            // 스프라이트 이름에 “exit” 단어가 포함되어 있다면
            if (lowerName.Contains("exit"))
            {
                exitCardLocalIndex = i;
                break;
            }
        }

        // (2-2) EXIT 카드와 STOP 카드 두 장을 따로 분리
        SpriteRenderer exitCard = cards[exitCardLocalIndex];
        SpriteRenderer stopCard1 = null;
        SpriteRenderer stopCard2 = null;
        int stopIdx = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (i == exitCardLocalIndex) continue;
            if (stopIdx == 0) { stopCard1 = cards[i]; stopIdx++; }
            else { stopCard2 = cards[i]; }
        }

        // (2-3) EXIT 카드가 최종 배치될 레인(positions 인덱스)를 랜덤으로 뽑아서 correctIndex에 저장
        correctIndex = Random.Range(0, positions.Length);
        exitCard.transform.position = positions[correctIndex].position;

        // (2-4) 나머지 두 레인에는 stopCard1, stopCard2를 하나씩 채운다
        int fillIdx = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == correctIndex) continue;

            if (fillIdx == 0)
            {
                stopCard1.transform.position = positions[i].position;
                fillIdx++;
            }
            else
            {
                stopCard2.transform.position = positions[i].position;
            }
        }

        // === (3) 셔플 완료 → 충돌(Trigger) 활성화 ===
        EnableColliders(true);

        // === (4) 낙하(Free-fall) 코루틴 시작 ===
        isDropping = true;
        StartCoroutine(DropAllCards());
    }

    /// <summary>
    /// 카드들이 fallTargetY 아래로 모두 내려갈 때까지 Free-fall
    /// </summary>
    IEnumerator DropAllCards()
    {
        // fallTargetY보다 위에 있는 카드가 하나라도 있는 동안 반복
        while (true)
        {
            bool anyAbove = false;
            for (int i = 0; i < cards.Length; i++)
            {
                Vector3 p = cards[i].transform.position;
                if (p.y > fallTargetY)
                {
                    p.y -= fallSpeed * Time.deltaTime;
                    if (p.y < fallTargetY) p.y = fallTargetY;
                    cards[i].transform.position = p;
                    anyAbove = true;
                }
            }

            if (!anyAbove) break;
            yield return null;
        }

        // 낙하가 끝났다는 표시
        isDropping = false;
        hasDropped = true;

        // 낙하를 마친 뒤에는 일정 시간 대기 없이 바로 삭제
        Destroy(gameObject);
    }

    /// <summary>
    /// 카드들의 Collider2D(Trigger)를 On/Off
    /// </summary>
    void EnableColliders(bool on)
    {
        foreach (var col in cardColliders)
            col.enabled = on;
    }

    /// <summary>
    /// 플레이어가 카드의 충돌 영역(Trigger)에 닿았을 때
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // 단 한 번만 처리하도록: 이미 낙하가 끝나면 무시
        if (hasDropped) return;

        float playerX = other.transform.position.x;
        float exitX = positions[correctIndex].position.x;
        float dx = Mathf.Abs(playerX - exitX);

        // “EXIT이 아닌 레인에 들어왔으면” 목숨 감소
        if (dx > 0.5f)
        {
            Debug.Log("셔플 실패! 목숨 감소");
            GameManager.Instance.LoseLife(1);
        }
        else
        {
            Debug.Log("EXIT 성공! 목숨 유지");
        }

        // 충돌이 일어나면 즉시 카드 전체 삭제
        Destroy(gameObject);
    }
}
