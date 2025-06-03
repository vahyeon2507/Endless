using UnityEngine;

public class RoundSpawner : MonoBehaviour
{
    [Header("적 Prefabs (도플, 해피, 셔플, 거머리 등)")]
    public GameObject[] enemyPrefabs;

    [Header("스폰 주기 (초)")]
    public float spawnInterval = 2f;
    private float spawnTimer;

    [Header("도플갱어 등 일반 몹: 레인 X 좌표 배열")]
    public float[] laneX = { -2f, 0f, 2f };

    [Header("화면 상단에서 스폰할 Y 좌표 (예: 5f)")]
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
        // *** 1) “이미 해피 바이러스가 씬에 존재하는지” 여부 선체크 ***
        if (FindObjectOfType<HappyVirus>() != null)
        {
            // 이미 한 마리가 존재하므로 이번 사이클엔 스폰 없음
            return;
        }

        // *** 2) 랜덤으로 적 Prefab 하나 선택 ***
        int idx = Random.Range(0, enemyPrefabs.Length);
        GameObject prefab = enemyPrefabs[idx];

        // *** 3) “거머리”라면, 위치 지정 없이 그냥 Instantiate(prefab) ***
        if (prefab.GetComponent<Leech_LShape>() != null)
        {
            Instantiate(prefab);
            return;
        }

        // *** 4) “나머지(도플·셔플·축구공·해피 등)”은 기존처럼 레인 X + spawnY ***
        //     단, “셔플카드”는 사실상 아래 스크립트가 내부에서 positions[] 배열을 참조하므로
        //     여기서 X값을 넣어줄 필요는 없지만, 충돌 판정 등을 위해 일정한 X축 위치로 스폰해 둡니다.
        //     (원칙적으로는 X=0만으로도 되지만, 그렇게 되면 스폰 시점부터 화면 위에 겹쳐 보이므로
        //     시각적으로 헷갈릴 수 있습니다. 그냥 중앙 레인처럼 보이도록 하려면 X=0 써도 OK)

        Vector3 spawnPos;
        // “셔플카드 Prefab”일 때
        if (prefab.GetComponent<ShuffleCard>() != null)
        {
            // 카드가 나중에 positions[]를 참조하니 여기서는 X=0, Y=spawnY만 넣어 줘도 됩니다.
            spawnPos = new Vector3(0f, spawnY, 0f);
        }
        else
        {
            // 도플갱어나 다른 유기체는 레인 X 랜덤
            float x = laneX[Random.Range(0, laneX.Length)];
            spawnPos = new Vector3(x, spawnY, 0f);
        }

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
