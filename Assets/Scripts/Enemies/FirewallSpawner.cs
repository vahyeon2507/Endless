using System.Collections;
using UnityEngine;
using Common;    // KeyColor enum

public class FirewallSpawner : MonoBehaviour
{
    public void SpawnFirewall()
    {
        SpawnKeyAndDoors();
    }
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
    public float keySpawnDelay = 1f;
    public float doorSpawnDelay = 1f;

    private float keyTimer;

    void Start()
    {
        keyTimer = keySpawnDelay;
    }

    void Update()
    {
        keyTimer -= Time.deltaTime;
        if (keyTimer <= 0f)
        {
            SpawnKeyAndDoors();
            keyTimer = keySpawnDelay;
        }
    }

    void SpawnKeyAndDoors()
    {
        // 1) 랜덤 키 컬러 뽑기 (1=Red, 2=Blue, 3=Yellow)
        KeyColor chosenColor = (KeyColor)Random.Range(1, 4);

        // 2) 키 스폰
        float keyX = laneX[Random.Range(0, laneX.Length)];
        var keyGO = Instantiate(keyPrefab, new Vector3(keyX, spawnY, 0), Quaternion.identity);
        // SpriteRenderer 에 스프라이트 할당
        var keySr = keyGO.GetComponent<SpriteRenderer>();
        keySr.sprite = keySprites[(int)chosenColor - 1];

        // 3) 잠시 기다렸다가 3개의 문 스폰
        StartCoroutine(SpawnDoorsAfterDelay(chosenColor));
    }

    IEnumerator SpawnDoorsAfterDelay(KeyColor doorColor)
    {
        yield return new WaitForSeconds(doorSpawnDelay);

        // X 자리 섞기
        var indices = new List<int> { 0, 1, 2 };
        for (int i = 0; i < 3; i++)
        {
            // 랜덤 하나 꺼내서
            int pick = Random.Range(0, indices.Count);
            int laneIndex = indices[pick];
            indices.RemoveAt(pick);

            // 문 스폰
            var doorGO = Instantiate(
                firewallPrefab,
                new Vector3(laneX[laneIndex], spawnY, 0),
                Quaternion.identity
            );

            // Firewall 스크립트에도 색 정보 넣어주기
            var fw = doorGO.GetComponent<Firewall>();
            fw.doorColor = doorColor;  // 플레이어와 비교할 때 쓸 값

            // SpriteRenderer 에 스프라이트 할당
            var doorSr = doorGO.GetComponent<SpriteRenderer>();
            doorSr.sprite = doorSprites[(int)doorColor - 1];
        }
    }
}
