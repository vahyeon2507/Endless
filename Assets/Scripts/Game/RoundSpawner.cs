using UnityEngine;


public class RoundSpawner : MonoBehaviour
{
    [Header("적 Prefabs (도플, 해피, 셔플, 거머리, 방화벽)")]
    public GameObject[] enemyPrefabs;

    [Header("방화벽 전용 스포너")]
    public FirewallSpawner firewallSpawner;    // 인스펙터에서 드래그할 것

    [Header("스폰 주기 (초)")]
    public float spawnInterval = 2f;
    private float spawnTimer;

    [Header("도플갱어 등 일반 몹: 레인 X 좌표 배열")]
    public float[] laneX = { -2f, 0f, 2f };

    [Header("화면 상단에서 스폰할 Y 좌표")]
    public float spawnY = 5f;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
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
        // 해피바이러스 1마리만 유지
        if (FindObjectOfType<HappyVirus>() != null)
            return;

        // 무작위 프리팹 선택
        int idx = Random.Range(0, enemyPrefabs.Length);
        GameObject prefab = enemyPrefabs[idx];

        if (prefab.GetComponent<Firewall>() != null)
        {
            // 방화벽이면 그냥 instatiate(prefab) 대신
            firewallSpawner.SpawnFirewall();
            return;
        }

        // 거머리만 위치 없이 Instantiate
        if (prefab.GetComponent<Leech_LShape>() != null)
        {
            Instantiate(prefab);
            return;
        }

        // 나머지(도플, 셔플, 축구공 등) 일반 레인 스폰
        Vector3 spawnPos;
        if (prefab.GetComponent<ShuffleCard>() != null)
        {
            spawnPos = new Vector3(0f, spawnY, 0f);
        }
        else
        {
            float x = laneX[Random.Range(0, laneX.Length)];
            spawnPos = new Vector3(x, spawnY, 0f);
        }

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
