using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public enum BallState
    {
        FALLING,    // 공이 아래로 떨어지는 중
        KICKED_UP,  // 공이 플레이어에게 차여 위로 튕겨올리는 중
        GOAL        // 5회 드리블 완료 후 골대 생성 상태 (사실상 공 파괴 직후 상태)
    }

    [Header("드리블 및 낙하 설정")]
    public int hitsRequired = 5;        // 플레이어가 공을 몇 번 차야 하는지
    public float fallSpeed = 5f;        // 공이 아래로 떨어지는 속도
    public float kickUpSpeed = 7f;      // 공이 위로 튀어올리는 속도

    [Header("좌표 설정")]
    public float spawnHighY = 7f;       // 공이 스폰될 때의 Y 좌표 (화면 상단 바깥)
    public float missY = -7f;           // 공이 이 Y보다 낮아지면 "놓친 것"으로 간주

    [Header("골대(Goal) 설정")]
    public GameObject goalPrefab;       // 골대 프리팹 (Tag="Goal" 붙여두기)
    public float goalSpawnY = 7f;       // 공 대신 골대를 스폰할 Y 좌표 (플레이어 머리 위)
    public float goalFallSpeed = 4f;    // 골대가 떨어지는 속도
    public float goalDestroyY = -7f;    // 골대가 이 Y보다 낮아지면 파괴

    private int hitCount = 0;           // 지금까지 차낸 횟수
    private BallState state;            // 현재 공 상태
    private Rigidbody2D rb;             // 공의 Rigidbody2D 컴포넌트
    private Collider2D ballCollider;    // 공의 Collider2D (IsTrigger = true)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<Collider2D>();

        // Rigidbody2D: Kinematic으로 설정하여 직접 transform 제어
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        // 스폰 직후 바로 아래로 떨어지도록 초기 상태 설정
        state = BallState.FALLING;
    }

    void Update()
    {
        switch (state)
        {
            case BallState.FALLING:
                HandleFalling();
                break;

            case BallState.KICKED_UP:
                HandleKickUp();
                break;

            case BallState.GOAL:
                // 공은 이미 제거됐고 골대 로직이 대신 움직이므로 여기서는 아무것도 안 함
                break;
        }
    }

    // 1) 공이 아래로 떨어지는 로직
    void HandleFalling()
    {
        // Y 좌표를 missY 방향(아래)으로 MoveTowards
        float newY = Mathf.MoveTowards(transform.position.y, missY, fallSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, 0f);

        // Y가 missY 에 도달(작거나 같아짐)하면 '놓친 것' 처리
        if (newY <= missY)
        {
            // 플레이어가 차내지 못했으므로 목숨 감소
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife(1);
            }
            Destroy(gameObject);
        }
    }

    // 2) 플레이어와 부딪쳐 바로 위로 튕기는 순간
    void OnTriggerEnter2D(Collider2D other)
    {
        if (state == BallState.FALLING && other.CompareTag("Player"))
        {
            // 차내기 (드리블)
            hitCount++;
            Debug.Log($"축구공 히트: {hitCount} / {hitsRequired}");

            // 공 위로 순간 이동(충돌 후 바로 튕기기를 자연스럽게 보이기 위해)
            state = BallState.KICKED_UP;

            // Collider를 잠시 끊어 추가 충돌 방지
            if (ballCollider != null)
                ballCollider.enabled = false;
        }
    }

    // 3) 공이 위로 튕겨올라가는 로직
    void HandleKickUp()
    {
        // 현재 Y에서 spawnHighY까지 켜져있는 가상 MoveTowards
        float newY = Mathf.MoveTowards(transform.position.y, spawnHighY, kickUpSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, 0f);

        // 다시 최상단(spawnHighY)에 닿으면
        if (Mathf.Approximately(newY, spawnHighY))
        {
            // 드리블 횟수가 아직 부족하면 다시 아래로 떨어지도록 전환
            if (hitCount < hitsRequired)
            {
                state = BallState.FALLING;
                ballCollider.enabled = true; // 다시 충돌 허용
            }
            else
            {
                // 5번 드리블 완료: 골대 생성 및 공 제거
                SpawnGoalAndRemoveBall();
            }
        }
    }

    // 4) 드리블 5회 완료 시 골대를 스폰하고 공을 제거
    void SpawnGoalAndRemoveBall()
    {
        state = BallState.GOAL;

        // 공 Collider 비활성화 (안전차원)
        if (ballCollider != null)
            ballCollider.enabled = false;

        // 공 파괴
        Destroy(gameObject);

        // 플레이어가 있는 레인(x좌표) 그대로, goalSpawnY 높이에 골대를 생성
        Vector3 goalPos = new Vector3(transform.position.x, goalSpawnY, 0f);
        GameObject goalObj = Instantiate(goalPrefab, goalPos, Quaternion.identity);

        // 골대에 "GoalFall" 스크립트가 붙어 있어야 낙하 및 파괴가 진행됨
        // GoalFall는 아래에 다시 설명
    }
}
