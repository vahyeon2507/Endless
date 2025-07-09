// Assets/Scripts/Entities/Leech_LShape.cs
using UnityEngine;
using System.Collections;

public class Leech_LShape : MonoBehaviour
{
    [Header("◾︎ 속도 및 드레인 설정")]
    [Tooltip("LifeZone으로 대각선 이동할 때 속도")]
    public float diagonalSpeed = 3f;
    [Tooltip("LifeZone에 붙어 있으면 이 시간(초) 후 목숨 1 감소")]
    public float drainTime = 10f;

    [Header("◾︎ 카메라 및 경계 계산용")]
    public Camera mainCamera;           // 비워두면 Start()에서 Camera.main 자동 할당
    private float camLeftX, camRightX, camBottomY;

    [Header("◾︎ LifeZone 설정")]
    public Transform lifeZone;          // Tag="LifeZone"인 트리거 영역

    // 내부 상태
    private bool isAttached = false;    // LifeZone에 도달했는지
    private float drainTimer = 0f;

    void Awake()
    {
        // Rigidbody2D를 Kinematic으로 설정 (스크립트 이동 제어)
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.simulated = true;
    }

    void Start()
    {
        // 1) 카메라 참조 & 경계 계산
        if (mainCamera == null)
            mainCamera = Camera.main;
        CalculateCameraBounds();

        // 2) LifeZone 참조 (Inspector에 할당 안 됐으면 씬에서 찾기)
        if (lifeZone == null)
        {
            var zl = GameObject.FindGameObjectWithTag("LifeZone");
            if (zl != null) lifeZone = zl.transform;
            else Debug.LogWarning("Leech_LShape: Tag=\"LifeZone\"인 오브젝트를 찾지 못했습니다.");
        }

        // 3) 화면 하단 외각 바로 아래, 왼쪽/오른쪽 랜덤 외각 X에서 스폰
        float spawnY = camBottomY - 1f;
        float spawnX = (Random.value < 0.5f)
            ? camLeftX - 0.5f
            : camRightX + 0.5f;
        transform.position = new Vector3(spawnX, spawnY, 0f);
    }

    void Update()
    {
        // -- LifeZone에 붙어 있으면 드레인 로직만 돌리고 이동 중단 --
        if (isAttached)
        {
            drainTimer += Time.deltaTime;
            if (drainTimer >= drainTime)
            {
                // 설정된 시간이 지나면 목숨 감소 후 파괴
                GameManager.Instance?.LoseLife(1);
                Destroy(gameObject);
            }
            return;
        }

        // -- 아직 붙지 않았다면: LifeZone 위치로 대각선 이동 --
        if (lifeZone != null)
        {
            Vector3 pos = transform.position;
            Vector3 target = lifeZone.position;
            Vector3 dir = (target - pos).normalized;

            pos += dir * diagonalSpeed * Time.deltaTime;
            transform.position = pos;

            // 목표 지점에 거의 닿으면 “붙은 상태”로 전환
            if (Vector3.Distance(pos, target) < 0.1f)
                AttachToLifeZone();
        }
    }

    /// <summary>
    /// LifeZone에 도달했음을 표시하고 드레인 타이머 초기화
    /// </summary>
    private void AttachToLifeZone()
    {
        isAttached = true;
        drainTimer = 0f;
    }

    /// <summary>
    /// 마우스 클릭 시 즉시 제거 (플레이어가 드래그해서 제거)
    /// </summary>
    void OnMouseDown()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 카메라 뷰포트 좌하단/우상단을 월드좌표로 변환해 경계값 계산
    /// </summary>
    private void CalculateCameraBounds()
    {
        float z = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 bl = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, z));
        Vector3 tr = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, z));

        camLeftX = bl.x;
        camRightX = tr.x;
        camBottomY = bl.y;
    }

    /// <summary>
    /// LifeZone 트리거에 진입하면 붙은 상태로 전환
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttached && other.CompareTag("LifeZone"))
            AttachToLifeZone();
    }
}
