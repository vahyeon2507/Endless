using UnityEngine;
using System.Collections;

public class Leech_LShape : MonoBehaviour
{
    [Header("◾︎ 속도 및 드레인 설정")]
    public float verticalSpeed = 2f;    // 수직으로 올라갈 때 속도
    public float horizontalSpeed = 3f;  // 상단에서 가로로 이동할 때 속도
    public float drainTime = 10f;       // LifeZone에 붙어 있으면 10초 후 목숨 1 감소

    [Header("◾︎ 카메라 및 경계 계산용")]
    public Camera mainCamera;           // 메인 카메라 (비워두면 Start()에서 Camera.main 자동 할당)
    private float camLeftX;             // 카메라 뷰 왼쪽 경계 X
    private float camRightX;            // 카메라 뷰 오른쪽 경계 X
    private float camBottomY;           // 카메라 뷰 하단 경계 Y
    private float camTopY;              // 카메라 뷰 상단 경계 Y

    [Header("◾︎ LifeZone 설정")]
    public Transform lifeZone;          // 씬에 Tag="LifeZone" 으로 둔 트리거 영역

    private bool isGoingUp;             // 위로 올라가는 중
    private bool isGoingRight;          // 상단에서 오른쪽으로 가는 중
    private bool isGoingLeft;           // 상단에서 왼쪽으로 가는 중
    private bool isAttached;            // LifeZone에 붙은 상태

    private float drainTimer;           // LifeZone에 붙어 있던 시간 누적
    private bool spawnOnLeft;           // 왼쪽 외각에서 스폰했는지 여부

    private Rigidbody2D rb;
    private Collider2D coll2d;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll2d = GetComponent<Collider2D>();

        // Rigidbody2D는 Kinematic으로, 스크립트에서 transform으로 이동 처리
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.simulated = true;
    }

    void Start()
    {
        // 1) 카메라 참조
        if (mainCamera == null)
            mainCamera = Camera.main;

        CalculateCameraBounds();

        // 2) LifeZone Transform 자동 연결 (만약 Inspector에서 할당 안 했을 때)
        if (lifeZone == null)
        {
            GameObject zl = GameObject.FindGameObjectWithTag("LifeZone");
            if (zl != null) lifeZone = zl.transform;
            else Debug.LogWarning("Leech_LShape: LifeZone 태그가 붙은 오브젝트를 찾지 못했습니다.");
        }

        // 3) 화면 외각(아래) 바로 아래, 그리고 왼쪽/오른쪽 외각 X 좌표에서 스폰
        float spawnY = camBottomY - 1f; // 카메라 뷰포트 바로 아래
        float spawnX;

        // 반반 확률로 왼쪽 or 오른쪽 선택
        spawnOnLeft = (Random.value < 0.5f);
        if (spawnOnLeft)
        {
            // 왼쪽 바깥쪽(카메라 왼쪽 경계보다 더 왼쪽)
            spawnX = camLeftX - 0.5f;
        }
        else
        {
            // 오른쪽 바깥쪽(카메라 오른쪽 경계보다 더 오른쪽)
            spawnX = camRightX + 0.5f;
        }

        transform.position = new Vector3(spawnX, spawnY, 0f);

        // 4) 초기 상태: 항상 “위로 올라가기” (수직 이동)
        isGoingUp = true;
        isGoingRight = false;
        isGoingLeft = false;
        isAttached = false;
        drainTimer = 0f;
    }

    void Update()
    {
        // 0) LifeZone에 붙어 있으면 드레인 타이머만 누적하고 이동하지 않음
        if (isAttached)
        {
            drainTimer += Time.deltaTime;
            if (drainTimer >= drainTime)
            {
                // 10초 지나면 목숨 깎고 파괴
                if (GameManager.Instance != null)
                    GameManager.Instance.LoseLife(1);
                Destroy(gameObject);
            }
            return;
        }

        // 1) L자 경로로 이동
        if (isGoingUp)
        {
            // 수직으로 위로 이동
            Vector3 pos = transform.position;
            pos.y += verticalSpeed * Time.deltaTime;

            // 목표 Y: LifeZone.Y (UI 바로 아래) 또는 camTopY-0.5
            float targetY = (lifeZone != null) ? lifeZone.position.y : (camTopY - 0.5f);

            if (pos.y >= targetY)
            {
                pos.y = targetY;
                isGoingUp = false;

                // 왼쪽 외각에서 올라 왔으면 → 상단에서 오른쪽으로
                if (spawnOnLeft)
                    isGoingRight = true;
                else
                    isGoingLeft = true;
            }
            transform.position = pos;
        }
        else if (isGoingRight)
        {
            // 상단에서 오른쪽으로 이동 → 목표 X: LifeZone.X
            Vector3 pos = transform.position;
            pos.x += horizontalSpeed * Time.deltaTime;

            float targetX = (lifeZone != null) ? lifeZone.position.x : ((camLeftX + camRightX) * 0.5f);
            if (pos.x >= targetX)
            {
                pos.x = targetX;
                isGoingRight = false;
                AttachToLifeZone();
            }
            transform.position = pos;
        }
        else if (isGoingLeft)
        {
            // 상단에서 왼쪽으로 이동 → 목표 X: LifeZone.X
            Vector3 pos = transform.position;
            pos.x -= horizontalSpeed * Time.deltaTime;

            float targetX = (lifeZone != null) ? lifeZone.position.x : ((camLeftX + camRightX) * 0.5f);
            if (pos.x <= targetX)
            {
                pos.x = targetX;
                isGoingLeft = false;
                AttachToLifeZone();
            }
            transform.position = pos;
        }
    }

    // LifeZone에 도달했을 때 호출
    private void AttachToLifeZone()
    {
        isAttached = true;
        drainTimer = 0f;
    }

    // ------------------------
    // “거머리 제거(사라짐)” + 드래그 기능
    // ------------------------

    void OnMouseDown()
    {
        // 드래그 시작함과 동시에 즉시 사라지도록 Destroy 호출
        Destroy(gameObject);
    }

    // OnMouseDrag / OnMouseUp 은 더 이상 필요 없으므로 제거

    // =====================
    // 카메라 경계 계산
    // =====================
    private void CalculateCameraBounds()
    {
        if (mainCamera == null) return;

        float camZ = Mathf.Abs(mainCamera.transform.position.z);
        // 뷰포트 좌하단
        Vector3 bl = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, camZ));
        // 뷰포트 우상단
        Vector3 tr = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, camZ));

        camLeftX = bl.x;
        camRightX = tr.x;
        camBottomY = bl.y;
        camTopY = tr.y;
    }

    // =====================
    // LifeZone 트리거 충돌
    // =====================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttached && other.CompareTag("LifeZone"))
        {
            AttachToLifeZone();
        }
    }
}
