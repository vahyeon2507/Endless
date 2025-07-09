using UnityEngine;

[DisallowMultipleComponent]
public class MapScroller : MonoBehaviour
{
    [Header("스크롤 속도 (유닛/초 혹은 픽셀/초)")]
    public float scrollSpeed = 2f;

    [Header("리셋 임계 Y 좌표")]
    public float resetThresholdY = -10f;

    // 런타임에 저장할 원래 Y 위치
    private float _startY;

    // RectTransform 기반 UI 인지 여부
    private RectTransform _rect;
    // 일반 Transform
    private Transform _tf;

    private void Awake()
    {
        // UI(Canvas) 위에 올려졌다면 RectTransform 이 붙어있을 거예요
        _rect = GetComponent<RectTransform>();
        if (_rect != null)
        {
            // UI 모드: anchoredPosition.y 를 기준으로
            _startY = _rect.anchoredPosition.y;
        }
        else
        {
            // 일반 스프라이트 모드: world position 을 기준으로
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
