using UnityEngine;

public class GoalFall : MonoBehaviour
{
    public float fallSpeed = 4f;     // 골대가 떨어지는 속도
    public float destroyY = -7f;     // 골대가 이 Y 밑으로 내려가면 파괴

    void Update()
    {
        // 골대를 아래로 떨어뜨림
        float newY = transform.position.y - fallSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, newY, 0f);

        // 일정 지점(플레이어 머리 아래 정도)을 지나면 파괴
        if (newY <= destroyY)
        {
            Destroy(gameObject);
        }
    }

    // (선택) 플레이어와 닿아도 목숨 감점은 없음.  
    // 예) 플레이어 충돌 시 특별한 처리가 필요하다면 이 안에 코드를 추가할 수 있음.
}
