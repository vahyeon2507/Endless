using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 leftLane = new Vector3(-2f, 0, 0);   // 왼쪽 위치
    private Vector3 rightLane = new Vector3(2f, 0, 0);   // 오른쪽 위치
    private bool isLeft = true;

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("왼쪽");
            isLeft = true;
            transform.position = new Vector3(leftLane.x, transform.position.y, transform.position.z);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            transform.position = new Vector3(0, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("오른쪽");
            isLeft = false;
            transform.position = new Vector3(rightLane.x, transform.position.y, transform.position.z);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            transform.position = new Vector3(0, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("숨고르기 발동!");
            // 나중에 게이지 체크 + 유기체 전멸 추가
        }
    }
}
