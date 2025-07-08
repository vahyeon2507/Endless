using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [Tooltip("�ʴ� �󸶳� ������ �Ʒ��� ��������")]
    public float fallSpeed = 3f;

    [Tooltip("�� y���� �����ϸ� ����ϴ�")]
    public float stopY = -3.5f;  // �÷��̾ �� �ִ� y

    void Update()
    {
        // 1) �Ʒ��� �̵�
        float newY = transform.position.y - fallSpeed * Time.deltaTime;

        // 2) ��ǥ y ������ �������� �ʵ���
        if (newY <= stopY)
        {
            newY = stopY;
            // �� �̻� ������ �ʿ䰡 ������ ��ũ��Ʈ�� ��Ȱ��ȭ
            enabled = false;
        }

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }
}
    