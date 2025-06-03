using UnityEngine;

// LifePanel 쪽에 붙여서, 거머리가 닿았을 때 알려주는 역할
public class LeechTargetArea : MonoBehaviour
{
    // 빈 스크립트. Leech 쪽에서 OnTriggerEnter2D(other.CompareTag("LeechTargetArea"))를 쓸 수도 있고,
    // 아니면 LifePanel 자식 Collider가 “LifeCollider” 태그를 가지고 있다면
    // Leech.cs에서 CompareTag("LifeCollider")로만 검사해도 된다.
}
