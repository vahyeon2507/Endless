// Assets/Scripts/Game/RoundSpawner.cs
using UnityEngine;

public class RoundSpawner : MonoBehaviour
{
    [Header("적 Prefabs (도플, 해피, 셔플, 거머리, 축구공, 방화벽, 패스워드)")]
    [Tooltip("Inspector에 꼭 FirewallSpawner 와 PasswordUIGroup Prefab도 넣어두세요.")]
    public GameObject[] enemyPrefabs;

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

        // 1) enemyPrefabs 중 하나 랜덤 선택
        int idx = Random.Range(0, enemyPrefabs.Length);
        GameObject prefab = enemyPrefabs[idx];

        // --- 새 분기: HappyVirus는 뷰포트 랜덤 위치에 스폰 ---
        if (prefab.GetComponent<HappyVirus>() != null)
        {
            Camera cam = Camera.main;
            float z = cam.nearClipPlane + 1f;
            Vector3 vp = new Vector3(Random.value, Random.value, z);
            Vector3 worldPos = cam.ViewportToWorldPoint(vp);
            Instantiate(prefab, worldPos, Quaternion.identity);
            return;
        }

        // --- 기존 분기: PasswordChallenge ---
        if (prefab.GetComponent<PasswordChallenge>() != null)
        {
            // 캔버스에 배치된 원본 위치 그대로 Instantiate
            Instantiate(prefab);
            return;
        }

        // 방화벽
        if (prefab.GetComponent<Firewall>() != null)
        {
            if (firewallSpawner != null)
                firewallSpawner.SpawnFirewall();
            else
                Debug.LogWarning("RoundSpawner에 FirewallSpawner가 할당되지 않았습니다!");
            return;
        }

        // 거머리
        if (prefab.GetComponent<Leech_LShape>() != null)
        {
            Instantiate(prefab);
            return;
        }

        // 셔플카드
        Vector3 spawnPos;
        if (prefab.GetComponent<ShuffleCard>() != null)
        {
            spawnPos = new Vector3(0f, spawnY, 0f);
        }
        else
        {
            // 그 외(축구공, 도플갱어 등)는 레인 X 중 랜덤
            float x = laneX[Random.Range(0, laneX.Length)];
            spawnPos = new Vector3(x, spawnY, 0f);
        }

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
