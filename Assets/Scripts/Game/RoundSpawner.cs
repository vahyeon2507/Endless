using UnityEngine;

public class RoundSpawner : MonoBehaviour
{
    [Header("적 Prefabs (도플, 해피, 셔플, 거머리, 축구공, 방화벽)")]
    public GameObject[] enemyPrefabs;     // Inspector에 꼭 Firewall Prefab도 넣어두세요.

    [Header("스폰 주기 (초)")]
    public float spawnInterval = 2f;
    private float spawnTimer;

    [Header("도플갱어 등 일반 몹: 레인 X 좌표 배열")]
    public float[] laneX = { -2f, 0f, 2f };

    [Header("화면 상단에서 스폰할 Y 좌표")]
    public float spawnY = 5f;

    [Header("방화벽 전용 스포너")]
    public FirewallSpawner firewallSpawner;   // Inspector에서 연결

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        // 라운드가 활성 상태가 아니면 스폰 중지
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
        // HappyVirus는 씬에 1마리만 존재
        if (FindObjectOfType<HappyVirus>() != null)
            return;

        // 1) 6종 enemyPrefabs 중 하나 랜덤 선택
        int idx = Random.Range(0, enemyPrefabs.Length);
        GameObject prefab = enemyPrefabs[idx];

        // 2) 만약 방화벽 Prefab이라면 → FirewallSpawner로 위임
        if (prefab.GetComponent<Firewall>() != null)
        {
            if (firewallSpawner != null)
                firewallSpawner.SpawnFirewall();
            else
                Debug.LogWarning("RoundSpawner에 FirewallSpawner가 할당되지 않았습니다!");
            return;
        }

        // 3) 거머리(Leech)만 특별 처리: 위치 없이 그냥 Instantiate
        if (prefab.GetComponent<Leech_LShape>() != null)
        {
            Instantiate(prefab);
            return;
        }

        // 4) 셔플카드만 X=0으로 스폰 (내부에서 positions[] 사용)
        Vector3 spawnPos;
        if (prefab.GetComponent<ShuffleCard>() != null)
        {
            spawnPos = new Vector3(0f, spawnY, 0f);
        }
        else
        {
            // 나머지(도플, 축구공 등)는 레인 X 중 랜덤
            float x = laneX[Random.Range(0, laneX.Length)];
            spawnPos = new Vector3(x, spawnY, 0f);
        }

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
