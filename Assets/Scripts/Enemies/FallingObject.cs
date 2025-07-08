using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [Tooltip("초당 얼마나 빠르게 아래로 떨어질지")]
    public float fallSpeed = 3f;

    [Tooltip("이 y값에 도달하면 멈춥니다")]
    public float stopY = -3.5f;  // 플레이어가 서 있는 y

    void Update()
    {
        // 1) 아래로 이동
        float newY = transform.position.y - fallSpeed * Time.deltaTime;

        // 2) 목표 y 밑으로 내려가지 않도록
        if (newY <= stopY)
        {
            newY = stopY;
            // 더 이상 떨어질 필요가 없으면 스크립트를 비활성화
            enabled = false;
        }

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }
}
    