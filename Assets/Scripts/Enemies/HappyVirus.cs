using UnityEngine;

public class HappyVirus : MonoBehaviour
{
    public float growthInterval = 3f; // 증식 주기
    public GameObject virusPrefab; // 증식용 Prefab
    public int maxSpawnCount = 33;  // 화면 가득칠 때 기준

    private float clickTime = 0f;
    private int clickCount = 0;

    void Start()
    {
        InvokeRepeating("Duplicate", growthInterval, growthInterval);
    }
    void OnMouseDown()
    {
        float now = Time.time;
        clickCount++;

        if (clickCount == 1)
        {
            clickTime = now;
        }
        else if (clickCount == 2)
        {
            if (now - clickTime < 0.3f)
            {
                // 더블 클릭 성공
                Destroy(gameObject);
            }
            // 클릭 횟수는 무조건 초기화
            clickCount = 0;
        }
    }


    void Duplicate()
    {

        // 메인 카메라 가져오기
        Camera cam = Camera.main;

        // 뷰포트 내 랜덤 위치 (x, y 모두 0~1)
        Vector3 viewportPos = new Vector3(Random.value, Random.value, cam.nearClipPlane + 1f);

        // 월드 좌표로 변환
        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);

        // 생성
        Instantiate(virusPrefab, worldPos, Quaternion.identity);

        // 화면에 남은 해피바이러스 개수 체크
        int count = FindObjectsOfType<HappyVirus>().Length;
        if (count >= maxSpawnCount)
        {
            // 최대 목숨만큼 차감 → 즉시 게임오버
            GameManager.Instance.LoseLife(GameManager.Instance.maxLife);
        }
    }
    void OnDestroy()
    {
        CancelInvoke(nameof(Duplicate));
    }
}