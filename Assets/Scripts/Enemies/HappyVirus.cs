// Assets/Scripts/Entities/HappyVirus.cs
using System.Collections;
using UnityEngine;

public class HappyVirus : MonoBehaviour
{
    [Header("Growth Settings")]
    public float growthInterval = 3f;      // 증식 주기
    public GameObject virusPrefab;         // 복제용 Prefab
    public int maxSpawnCount = 33;         // 최대 개수

    [Header("Disappear Animation (Mecanim)")]
    [Tooltip("Animator 컴포넌트")]
    public Animator animator;
    [Tooltip("Animator Controller에 설정한 Trigger 파라미터 이름")]
    public string disappearTrigger = "PlayDisappear";
    [Tooltip("사라지는 애니메이션 클립(happyclick)을 드래그하세요")]
    public AnimationClip disappearClip;

    // 더블클릭 판정용
    private float clickTime;
    private int clickCount;
    private bool isDying;

    void Start()
    {
        // 일정 주기로 복제 호출
        InvokeRepeating(nameof(Duplicate), growthInterval, growthInterval);
    }

    void OnMouseDown()
    {
        if (isDying) return;

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
                // 더블클릭 감지
                isDying = true;
                CancelInvoke(nameof(Duplicate));

                // Animator에 트리거 날리기
                if (animator != null)
                    animator.SetTrigger(disappearTrigger);

                // 애니 길이만큼 기다렸다가 파괴
                StartCoroutine(WaitAndDestroy());
            }
            clickCount = 0;
        }
    }

    void Duplicate()
    {
        if (isDying) return;

        var cam = Camera.main;
        var vp = new Vector3(Random.value, Random.value, cam.nearClipPlane + 1f);
        Instantiate(virusPrefab, cam.ViewportToWorldPoint(vp), Quaternion.identity);

        if (FindObjectsOfType<HappyVirus>().Length >= maxSpawnCount)
            GameManager.Instance.LoseLife(GameManager.Instance.maxLife);
    }

    IEnumerator WaitAndDestroy()
    {
        if (disappearClip != null)
        {
            // 클립 길이만큼 대기
            yield return new WaitForSeconds(disappearClip.length);
        }
        else
        {
            // 클립이 할당 안 돼 있으면 기본 0.5초
            yield return new WaitForSeconds(0.5f);
            Debug.LogWarning("HappyVirus: disappearClip이 할당되지 않았습니다!");
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(Duplicate));
    }
}
