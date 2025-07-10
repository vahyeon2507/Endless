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
        // GameManager �ʱ�ȭ
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();

        // �� �����
        SceneManager.LoadScene("MainScene");
    }

    private void GoToMainMenu()
    {
        // ���� �޴� ���� ������ Reset ó��
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();

        SceneManager.LoadScene("MainMenu");
    }
}
