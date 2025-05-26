using System.Collections;
using UnityEngine;

public class ShuffleCard : MonoBehaviour
{
    public SpriteRenderer[] cards;        // [0]=EXIT, [1]=STOP, [2]=STOP
    public Transform[] positions;         // 3개의 레인 월드 좌표
    public float shuffleDuration = 2f;    // 셔플 애니메이션 총 시간
    public int shuffleCount = 10;         // 셔플 횟수

    private int correctIndex;             // EXIT 카드가 최종적으로 위치할 인덱스

    void Start()
    {
        // 최초 로테이션: cards[i].transform.position = positions[i].position
        for (int i = 0; i < 3; i++)
            cards[i].transform.position = positions[i].position;

        // EXIT 카드는 0, STOP 카드는 1,2 로 세팅
        StartCoroutine(DoShuffle());
    }

    IEnumerator DoShuffle()
    {
        for (int i = 0; i < shuffleCount; i++)
        {
            // 두 카드 인덱스 랜덤 교환
            int a = Random.Range(0, 3);
            int b;
            do { b = Random.Range(0, 3); } while (b == a);

            // 위치 스왑
            Vector3 posA = cards[a].transform.position;
            cards[a].transform.position = cards[b].transform.position;
            cards[b].transform.position = posA;

            yield return new WaitForSeconds(shuffleDuration / shuffleCount);
        }

        // 최종 위치 파악
        for (int i = 0; i < 3; i++)
            if (cards[i].sprite.name.Contains("exit"))  // EXIT 스프라이트 이름 기준
                correctIndex = i;

        // 이제 카드들을 콜라이더만 남긴 채 고정
        EnableColliders(true);
    }

    void EnableColliders(bool on)
    {
        foreach (var r in cards)
            r.GetComponent<Collider2D>().enabled = on;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 플레이어가 멈출 레인(positions[correctIndex])에 있지 않으면 피해
        if (Mathf.Abs(other.transform.position.x - positions[correctIndex].position.x) > 0.1f)
        {
            Debug.Log("셔플 실패! 목숨 하나 감소");
            // GameManager.Instance.LoseLife();
        }
        Destroy(gameObject);
    }
}
