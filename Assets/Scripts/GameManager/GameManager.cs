using UnityEngine;

public class GameManager : MonoBehaviour
{
	public UIManager uiManager;  // 🔥 UIManager 참조 연결할 변수

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
			uiManager.UpdateLife(2);

		if (Input.GetKeyDown(KeyCode.Alpha2))
			uiManager.UpdateLife(1);

		if (Input.GetKeyDown(KeyCode.Alpha3))
			uiManager.UpdateGauge(0.35f);
	}
}
