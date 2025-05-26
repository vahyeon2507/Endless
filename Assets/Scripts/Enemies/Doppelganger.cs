using UnityEngine;

public class Doppelganger : MonoBehaviour
{
    public float speed = 3f;

    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("도플갱어에게 맞음!");
            // GameManager.Instance.LoseLife();  ← 나중에 연동
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Killzone"))
        {
            // 화면 밖 처리용
            Destroy(gameObject);
        }
    }
}
