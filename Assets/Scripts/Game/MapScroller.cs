using UnityEngine;

[DisallowMultipleComponent]
public class MapScroller : MonoBehaviour
{
    [Header("��ũ�� �ӵ� (����/�� Ȥ�� �ȼ�/��)")]
    public float scrollSpeed = 2f;

    [Header("���� �Ӱ� Y ��ǥ")]
    public float resetThresholdY = -10f;

    // ��Ÿ�ӿ� ������ ���� Y ��ġ
    private float _startY;

    // RectTransform ��� UI ���� ����
    private RectTransform _rect;
    // �Ϲ� Transform
    private Transform _tf;

    private void Awake()
    {
        // UI(Canvas) ���� �÷����ٸ� RectTransform �� �پ����� �ſ���
        _rect = GetComponent<RectTransform>();
        if (_rect != null)
        {
            // UI ���: anchoredPosition.y �� ��������
            _startY = _rect.anchoredPosition.y;
        }
        else
        {
            // �Ϲ� ��������Ʈ ���: world position �� ��������
            _tf = transform;
            _startY = _tf.position.y;
        }
    }

    private void Update()
    {
        float delta = scrollSpeed * Time.deltaTime;

        if (_rect != null)
            ScrollUI(delta);
        else
            ScrollWorld(delta);
    }

    private void ScrollUI(float delta)
    {
        var pos = _rect.anchoredPosition;
        pos.y -= delta;

        if (pos.y <= resetThresholdY)
            pos.y = _startY;

        _rect.anchoredPosition = pos;
    }

    private void ScrollWorld(float delta)
    {
        var pos = _tf.position;
        pos.y -= delta;

        if (pos.y <= resetThresholdY)
            pos.y = _startY;

        _tf.position = pos;
    }
}
