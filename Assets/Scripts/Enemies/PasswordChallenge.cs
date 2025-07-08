using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordChallenge : MonoBehaviour
{
    [Header("Timing & Slow-Mo")]
    [Tooltip("버튼 시퀀스를 1초간 보여줍니다 (unscaled).")]
    public float sequenceDisplayTime = 1f;
    [Tooltip("버튼 입력을 받을 최대 시간 (초, unscaled).")]
    public float inputTimeLimit = 3f;
    [Tooltip("패스워드 도중 전체 게임 속도를 느리게 할 배율.")]
    [Range(0.1f, 1f)]
    public float slowTimeScale = 0.9f;

    [Header("UI References")]
    [Tooltip("게임 화면을 어둡게 덮는 Image (반투명 검정).")]
    public Image overlayImage;
    [Tooltip("패스워드 UI 전체를 묶는 CanvasGroup.")]
    public CanvasGroup uiGroup;
    [Tooltip("4칸짜리 버튼 슬롯 이미지들.")]
    public Image[] sequenceImages = new Image[4];

    [Header("Button Sprites")]
    [Tooltip("0:A키, 1:D키, 2:마우스 좌클릭, 3:마우스 우클릭 순서")]
    public Sprite[] buttonSprites = new Sprite[4];

    // 내부 상태
    bool isActive = false;
    int[] sequence = new int[4];
    PlayerController playerCtrl;

    /// <summary>
    /// 외부에서 호출하면 패스워드 도전이 시작됩니다.
    /// </summary>
    public void StartChallenge()
    {
        if (isActive) return;
        isActive = true;

        // 1) 느려짐 & 입력 잠금
        Time.timeScale = slowTimeScale;
        playerCtrl = FindObjectOfType<PlayerController>();
        if (playerCtrl) playerCtrl.enabled = false;

        // 2) 배경 어둡게, UI 보이기
        overlayImage.gameObject.SetActive(true);
        uiGroup.alpha = 1f;
        uiGroup.interactable = false; // 이벤트시스템 입력 차단

        // 3) 코루틴으로 시퀀스 -> 입력 단계 진행
        StartCoroutine(RunChallenge());
    }

    IEnumerator RunChallenge()
    {
        // ▶ STEP 1: 랜덤 시퀀스 생성 (0~3 중 중복 없이 4개)
        var idxs = new List<int> { 0, 1, 2, 3 };
        for (int i = 0; i < 4; i++)
        {
            int pick = Random.Range(0, idxs.Count);
            sequence[i] = idxs[pick];
            idxs.RemoveAt(pick);
        }

        // ▶ STEP 2: 시퀀스 1초간 보여주기 (unscaled)
        for (int i = 0; i < 4; i++)
        {
            sequenceImages[i].sprite = buttonSprites[sequence[i]];
            sequenceImages[i].color = Color.white;             // 선명히
            sequenceImages[i].gameObject.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(sequenceDisplayTime);

        // ▶ STEP 3: 슬롯들을 “비활성화된” 색으로 초기화
        for (int i = 0; i < 4; i++)
        {
            sequenceImages[i].sprite = null;
            sequenceImages[i].color = new Color(1, 1, 1, 0.2f);
        }

        // ▶ STEP 4: 플레이어 입력 받기 (최대 inputTimeLimit)
        int inputCount = 0;
        float timer = inputTimeLimit;

        while (inputCount < 4 && timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;

            // A키
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (sequence[inputCount] == 0)
                {
                    sequenceImages[inputCount].sprite = buttonSprites[0];
                    sequenceImages[inputCount].color = Color.white;
                    inputCount++;
                }
                else
                {
                    OnFail();
                    yield break;
                }
            }
            // D키
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (sequence[inputCount] == 1)
                {
                    sequenceImages[inputCount].sprite = buttonSprites[1];
                    sequenceImages[inputCount].color = Color.white;
                    inputCount++;
                }
                else
                {
                    OnFail();
                    yield break;
                }
            }
            // 마우스 좌
            else if (Input.GetMouseButtonDown(0))
            {
                if (sequence[inputCount] == 2)
                {
                    sequenceImages[inputCount].sprite = buttonSprites[2];
                    sequenceImages[inputCount].color = Color.white;
                    inputCount++;
                }
                else
                {
                    OnFail();
                    yield break;
                }
            }
            // 마우스 우
            else if (Input.GetMouseButtonDown(1))
            {
                if (sequence[inputCount] == 3)
                {
                    sequenceImages[inputCount].sprite = buttonSprites[3];
                    sequenceImages[inputCount].color = Color.white;
                    inputCount++;
                }
                else
                {
                    OnFail();
                    yield break;
                }
            }

            yield return null;
        }

        // 제한시간 내에 4번 모두 맞췄으면 성공, 아니면 실패
        if (inputCount >= 4)
            OnSuccess();
        else
            OnFail();
    
}



    void OnSuccess()
    {
        // (목숨 차감 없음)
        EndChallenge();
    }

    void OnFail()
    {
        // 목숨 하나 잃기
        GameManager.Instance.LoseLife(1);
        EndChallenge();
    }

    void EndChallenge()
    {
        // 1) 시간 복원, 입력 복원
        Time.timeScale = 1f;
        if (playerCtrl) playerCtrl.enabled = true;

        // 2) UI/오버레이 닫기
        overlayImage.gameObject.SetActive(false);
        uiGroup.alpha = 0f;

        isActive = false;
    }
}
