using UnityEngine;

public class RoundSpawner : MonoBehaviour
{
    [Header("적 Prefabs (도플, 해피, 셔플)")]
    public GameObject[] enemyPrefabs;

    [Header("스폰 주기 (초)")]
    public float spawnInterval = 2f;
    private float spawnTimer;

    [Header("도플갱어 레인 X 좌표")]
    public float[] laneX = { -2f, 0f, 2f };
    [Header("적 스폰 Y 좌표")]
    public float spawnY = 5f;

    void Start()
    {
        // 타이머 초기화
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        // 라운드가 진행 중일 때만 스폰
        if (!RoundManager.Instance.IsRoundActive)
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnRandomEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnRandomEnemy()
    {
        // 0~enemyPrefabs.Length-1 중 랜덤 선택
        int idx = Random.Range(0, enemyPrefabs.Length);
        GameObject prefab = enemyPrefabs[idx];

        // 도플갱어도, 다른 애들도 전부 레인에 스폰
        float x = laneX[Random.Range(0, laneX.Length)];
        Vector3 spawnPos = new Vector3(x, spawnY, 0f);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
