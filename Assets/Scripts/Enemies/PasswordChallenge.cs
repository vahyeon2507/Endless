// Assets/Scripts/Entities/PasswordChallenge.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Common;  // KeyColor enum

public class PasswordChallenge : MonoBehaviour
{
    [Header("Timing & Slow-Mo")]
    [Tooltip("전체 입력 및 힌트 노출 시간 (초)")]
    public float inputTimeLimit = 4f;
    [Tooltip("입력 중 게임 속도 배율")]
    [Range(0.01f, 1f)]
    public float slowTimeScale = 0.1f;

    [Header("UI References")]
    public Image overlayImage;          // 어두워지는 반투명 오버레이
    public CanvasGroup uiGroup;         // 버튼 힌트들을 블록하기 위해

    [Header("Sequence Images")]
    [Tooltip("힌트로 표시될 4칸 Image")]
    public Image[] sequenceImages = new Image[4];

    [Header("Button Sprites")]
    [Tooltip("A, D, 좌클릭, 우클릭 순서대로")]
    public Sprite[] buttonSprites = new Sprite[4];

    // 내부
    private KeyColor[] sequence;        // 정답 시퀀스 (permutation)
    private int inputCount;
    private PlayerController playerController;
    private bool challengeEnded = false;

    void Start()
    {
        // 0) 플레이어 입력 잠금
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerController.enabled = false;

        // 1) Round 멈추고 느리게
        Time.timeScale = slowTimeScale;
        RoundManager.Instance.IsRoundActive = false;

        // 2) UI 켜기
        overlayImage.gameObject.SetActive(true);
        uiGroup.blocksRaycasts = true;

        // 3) 랜덤 시퀀스 생성 (0~3 각 키를 한 번씩만)
        sequence = new KeyColor[4];
        int[] idx = new int[] { 0, 1, 2, 3 };
        // Fisher–Yates shuffle
        for (int i = 0; i < idx.Length; i++)
        {
            int j = Random.Range(i, idx.Length);
            int tmp = idx[i];
            idx[i] = idx[j];
            idx[j] = tmp;

            sequence[i] = (KeyColor)idx[i];
            sequenceImages[i].sprite = buttonSprites[idx[i]];
            sequenceImages[i].color = Color.white;
        }

        inputCount = 0;
        // 4) 입력 루프 시작
        StartCoroutine(HandleInput());
    }

    IEnumerator HandleInput()
    {
        // 4.1) 초기 1초 대기: 어떤 입력도 받지 않음
        yield return new WaitForSecondsRealtime(1f);

        // 4.2) 나머지 inputTimeLimit 동안에만 입력 허용
        float timer = inputTimeLimit;
        while (timer > 0f && inputCount < sequence.Length)
        {
            timer -= Time.unscaledDeltaTime;

            // 4가지 키 입력 체크 (Unscaled so 슬로우모드 영향 없음)
            if (Input.GetKeyDown(KeyCode.A)) yield return CheckKey(0);
            else if (Input.GetKeyDown(KeyCode.D)) yield return CheckKey(1);
            else if (Input.GetMouseButtonDown(0)) yield return CheckKey(2);
            else if (Input.GetMouseButtonDown(1)) yield return CheckKey(3);

            yield return null;
        }

        // 성공 여부 결정
        bool success = (inputCount >= sequence.Length);
        EndChallenge(success);
    }

    IEnumerator CheckKey(int keyIndex)
    {
        if (challengeEnded) yield break;

        // 맞았으면 다음칸 불투명하게 표시
        if (sequence[inputCount] == (KeyColor)keyIndex)
        {
            sequenceImages[inputCount].color = Color.gray;
            inputCount++;
        }
        else
        {
            // 틀리면 즉시 실패
            EndChallenge(false);
            yield break;
        }

        yield return null;
    }

    private void EndChallenge(bool success)
    {
        if (challengeEnded) return;
        challengeEnded = true;

        // 1) 원상 복구
        Time.timeScale = 1f;
        RoundManager.Instance.IsRoundActive = true;

        // 플레이어 입력 복구
        if (playerController != null)
            playerController.enabled = true;

        overlayImage.gameObject.SetActive(false);
        uiGroup.blocksRaycasts = false;

        // 2) 결과 처리
        if (success)
            GameManager.Instance.ConsumeKey();
        else
            GameManager.Instance.LoseLife(1);

        // 3) 자신 파괴
        Destroy(gameObject);
    }
}
