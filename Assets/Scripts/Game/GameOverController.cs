// Assets/Scripts/Game/GameOverController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public Button restartButton;
    public Button mainMenuButton;

    void Awake()
    {
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void RestartGame()
    {
        // GameManager 초기화
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();

        // 씬 재시작
        SceneManager.LoadScene("MainScene");
    }

    private void GoToMainMenu()
    {
        // 메인 메뉴 가기 전에도 Reset 처리
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();

        SceneManager.LoadScene("MainMenu");
    }
}
