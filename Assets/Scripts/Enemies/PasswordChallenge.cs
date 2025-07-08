using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordChallenge : MonoBehaviour
{
    [Header("Timing & Slow-Mo")]
    [Tooltip("��ư �������� 1�ʰ� �����ݴϴ� (unscaled).")]
    public float sequenceDisplayTime = 1f;
    [Tooltip("��ư �Է��� ���� �ִ� �ð� (��, unscaled).")]
    public float inputTimeLimit = 3f;
    [Tooltip("�н����� ���� ��ü ���� �ӵ��� ������ �� ����.")]
    [Range(0.1f, 1f)]
    public float slowTimeScale = 0.9f;

    [Header("UI References")]
    [Tooltip("���� ȭ���� ��Ӱ� ���� Image (������ ����).")]
    public Image overlayImage;
    [Tooltip("�н����� UI ��ü�� ���� CanvasGroup.")]
    public CanvasGroup uiGroup;
    [Tooltip("4ĭ¥�� ��ư ���� �̹�����.")]
    public Image[] sequenceImages = new Image[4];

    [Header("Button Sprites")]
    [Tooltip("0:AŰ, 1:DŰ, 2:���콺 ��Ŭ��, 3:���콺 ��Ŭ�� ����")]
    public Sprite[] buttonSprites = new Sprite[4];

    // ���� ����
    bool isActive = false;
    int[] sequence = new int[4];
    PlayerController playerCtrl;

    /// <summary>
    /// �ܺο��� ȣ���ϸ� �н����� ������ ���۵˴ϴ�.
    /// </summary>
    public void StartChallenge()
    {
        if (isActive) return;
        isActive = true;

        // 1) ������ & �Է� ���
        Time.timeScale = slowTimeScale;
        playerCtrl = FindObjectOfType<PlayerController>();
        if (playerCtrl) playerCtrl.enabled = false;

        // 2) ��� ��Ӱ�, UI ���̱�
        overlayImage.gameObject.SetActive(true);
        uiGroup.alpha = 1f;
        uiGroup.interactable = false; // �̺�Ʈ�ý��� �Է� ����

        // 3) �ڷ�ƾ���� ������ -> �Է� �ܰ� ����
        StartCoroutine(RunChallenge());
    }

    IEnumerator RunChallenge()
    {
        // �� STEP 1: ���� ������ ���� (0~3 �� �ߺ� ���� 4��)
        var idxs = new List<int> { 0, 1, 2, 3 };
        for (int i = 0; i < 4; i++)
        {
            int pick = Random.Range(0, idxs.Count);
            sequence[i] = idxs[pick];
            idxs.RemoveAt(pick);
        }

        // �� STEP 2: ������ 1�ʰ� �����ֱ� (unscaled)
        for (int i = 0; i < 4; i++)
        {
            sequenceImages[i].sprite = buttonSprites[sequence[i]];
            sequenceImages[i].color = Color.white;             // ������
            sequenceImages[i].gameObject.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(sequenceDisplayTime);

        // �� STEP 3: ���Ե��� ����Ȱ��ȭ�ȡ� ������ �ʱ�ȭ
        for (int i = 0; i < 4; i++)
        {
            sequenceImages[i].sprite = null;
            sequenceImages[i].color = new Color(1, 1, 1, 0.2f);
        }

        // �� STEP 4: �÷��̾� �Է� �ޱ� (�ִ� inputTimeLimit)
        int inputCount = 0;
        float timer = inputTimeLimit;

        while (inputCount < 4 && timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;

            // AŰ
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
            // DŰ
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
            // ���콺 ��
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
            // ���콺 ��
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

        // ���ѽð� ���� 4�� ��� �������� ����, �ƴϸ� ����
        if (inputCount >= 4)
            OnSuccess();
        else
            OnFail();
    
}



    void OnSuccess()
    {
        // (��� ���� ����)
        EndChallenge();
    }

    void OnFail()
    {
        // ��� �ϳ� �ұ�
        GameManager.Instance.LoseLife(1);
        EndChallenge();
    }

    void EndChallenge()
    {
        // 1) �ð� ����, �Է� ����
        Time.timeScale = 1f;
        if (playerCtrl) playerCtrl.enabled = true;

        // 2) UI/�������� �ݱ�
        overlayImage.gameObject.SetActive(false);
        uiGroup.alpha = 0f;

        isActive = false;
    }
}
