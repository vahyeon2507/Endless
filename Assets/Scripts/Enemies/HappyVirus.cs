using UnityEngine;

public class HappyVirus : MonoBehaviour
{
    public float growthInterval = 3f; // 증식 주기
    public GameObject virusPrefab; // 증식용 Prefab

    private float clickTime = 0f;
    private int clickCount = 0;

    void Start()
    {
        InvokeRepeating("Duplicate", growthInterval, growthInterval);
    }

    void OnMouseDown()
    {
        clickCount++;
        if (clickCount == 1)
            clickTime = Time.time;
        else if (clickCount == 2 && Time.time - clickTime < 0.3f) // 더블클릭 판정
        {
            Debug.Log("HappyVirus 더블클릭 삭제!");
            Destroy(gameObject);
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
    }
}
