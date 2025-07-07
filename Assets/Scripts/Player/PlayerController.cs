using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 leftLane = new Vector3(-2f, -3.5f, 0f);   // 왼쪽 위치
    private Vector3 rightLane = new Vector3(2f, -3.5f, 0f);   // 오른쪽 위치
    private bool isLeft = true;

    // GameManager 싱글톤 캐시
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        // ← A 키로 왼쪽 레인으로
        if (Input.GetKey(KeyCode.A))
        {
            isLeft = true;
            transform.position = new Vector3(leftLane.x, transform.position.y, transform.position.z);
        }
        // A 키 떼면 중앙 복귀
        if (Input.GetKeyUp(KeyCode.A))
        {
            transform.position = new Vector3(0f, -3.5f, 0f);
        }

        // → D 키로 오른쪽 레인으로
        if (Input.GetKey(KeyCode.D))
        {
            isLeft = false;
            transform.position = new Vector3(rightLane.x, transform.position.y, transform.position.z);
        }
        // D 키 떼면 중앙 복귀
        if (Input.GetKeyUp(KeyCode.D))
        {
            transform.position = new Vector3(0f, -3.5f, 0f);
        }

        // 스페이스바 누르면 “숨고르기” 발동
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("숨고르기 발동!");

            // GameManager에 숨고르기(Purge) 요청
            // → PurgeAllEnemies(): 화면에 있는 모든 유기체 제거 + 게이지 리셋 등 처리
            if (gameManager != null)
                gameManager.PurgeAllEnemies();
            else
                Debug.LogWarning("GameManager 인스턴스가 없습니다!");
        }
    }
}
