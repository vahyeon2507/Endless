using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public Button restartButton;
    public Button mainMenuButton;

    void Awake()
    {
        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainScene");
        });
        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu"); // 메인 메뉴 씬 이름
        });
    }
}
