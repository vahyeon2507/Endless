using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject doppelPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnDoppelganger();
        }
    }

    void SpawnDoppelganger()
    {
        float x = Random.Range(-2f, 2f); // 가로 위치 랜덤
        Vector3 spawnPos = new Vector3(x, 5f, 0f); // 상단 위치
        Instantiate(doppelPrefab, spawnPos, Quaternion.identity);
    }
}
