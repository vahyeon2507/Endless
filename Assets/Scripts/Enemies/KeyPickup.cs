// Assets/Scripts/Entities/KeyPickup.cs
using UnityEngine;
using Common;  // Enums.cs 에 정의된 KeyColor 를 가져옵니다

[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    [Tooltip("스포너에서 할당되는 이 열쇠의 색깔")]
    public KeyColor keyColor;

    /// <summary>
    /// 플레이어가 닿으면 GameManager 에 색상을 전달하고 스스로 파괴
    /// </summary>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
            return;

        GameManager.Instance.PickKey(keyColor);
        Destroy(gameObject);
    }
}
