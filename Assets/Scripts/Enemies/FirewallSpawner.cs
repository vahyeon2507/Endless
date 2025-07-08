using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;    // Enums.cs 에 정의된 KeyColor 를 가져옵니다

public class FirewallSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject keyPrefab;       // KeyPickup 스크립트가 붙어 있어야 함
    public GameObject firewallPrefab;  // Firewall 스크립트가 붙어 있어야 함

    [Header("Sprite Assets")]
    [Tooltip("Red, Blue, Yellow 순서대로 넣어주세요")]
    public Sprite[] keySprites = new Sprite[3];
    [Tooltip("Red, Blue, Yellow 순서대로 넣어주세요")]
    public Sprite[] doorSprites = new Sprite[3];

    [Header("Spawn Settings")]
    public float[] laneX = { -2f, 0f, 2f };
    public float spawnY = 5f;
    public float doorSpawnDelay = 1f;

    /// <summary>
    /// 외부에서 이 메서드를 호출하면
    /// 키 1개 → (딜레이) → 문 3개 사이클이 한 번 돌게 됩니다.
    /// </summary>
    public void SpawnFirewall()
    {
        SpawnKeyAndDoors();
    }

    private void SpawnKeyAndDoors()
    {
        // 1) 랜덤 키 색 뽑기
        KeyColor chosenColor = (KeyColor)Random.Range(1, 4);

        // 2) 랜덤 레인에 키 스폰
        float keyX = laneX[Random.Range(0, laneX.Length)];
        var keyGO = Instantiate(
            keyPrefab,
            new Vector3(keyX, spawnY, 0f),
            Quaternion.identity
        );

        // **여기부터 추가된 부분**
        // KeyPickup 컴포넌트에 색 정보 넘기기
        var keyPickup = keyGO.GetComponent<KeyPickup>();
        if (keyPickup != null)
            keyPickup.keyColor = chosenColor;
        else
            Debug.LogWarning("KeyPickup 컴포넌트가 없습니다!");

        // 키 스프라이트 교체
        var keySr = keyGO.GetComponent<SpriteRenderer>();
        keySr.sprite = keySprites[(int)chosenColor - 1];
        // **여기까지**

        // 3) 딜레이 후 문 3개 스폰
        StartCoroutine(SpawnDoorsAfterDelay(chosenColor));
    }

    private IEnumerator SpawnDoorsAfterDelay(KeyColor doorColor)
    {
        yield return new WaitForSeconds(doorSpawnDelay);

        // 3개 레인 인덱스와 3개 색깔 리스트를 랜덤 섞기
        var laneIndices = new List<int> { 0, 1, 2 };
        var doorColors = new List<KeyColor> { KeyColor.Red, KeyColor.Blue, KeyColor.Yellow };

        for (int i = 0; i < 3; i++)
        {
            // 랜덤 레인 선택
            int pickLane = Random.Range(0, laneIndices.Count);
            int laneIndex = laneIndices[pickLane];
            laneIndices.RemoveAt(pickLane);

            // 랜덤 색 선택
            int pickColor = Random.Range(0, doorColors.Count);
            KeyColor color = doorColors[pickColor];
            doorColors.RemoveAt(pickColor);

            // 문 생성
            var doorGO = Instantiate(
                firewallPrefab,
                new Vector3(laneX[laneIndex], spawnY, 0f),
                Quaternion.identity
            );

            // Firewall 스크립트에 색 정보 세팅
            var fw = doorGO.GetComponent<Firewall>();
            if (fw != null)
                fw.doorColor = color;

            // 문 스프라이트 교체
            var doorSr = doorGO.GetComponent<SpriteRenderer>();
            doorSr.sprite = doorSprites[(int)color - 1];
        }
    }
}
