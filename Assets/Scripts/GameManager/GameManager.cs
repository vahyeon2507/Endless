using UnityEngine;

public class GameManager : MonoBehaviour
{
	// 1) 전역 인스턴스
	public static GameManager Instance { get; private set; }

	public UIManager uiManager; // Inspector에서 드래그 연결
	public int maxLife = 3;
	private int currentLife;

	void Awake()
	{
		// 싱글톤 초기화
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		currentLife = maxLife;
		uiManager.UpdateLife(currentLife);
	}

	public void LoseLife(int amount = 1)
	{
		currentLife = Mathf.Max(0, currentLife - amount);
		uiManager.UpdateLife(currentLife);
		if (currentLife <= 0) GameOver();
	}

	void GameOver()
	{
		Debug.Log("게임 오버!");
		// TODO: 리스타트, 메뉴 전환 등
		enabled = false;
	}
}
