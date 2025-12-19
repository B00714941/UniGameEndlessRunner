using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas;
    [SerializeField]
    private GameObject gameWinCanvas;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    private int score = 0;


    // This StopGame function operates on an old scoring system but would ensure that the Game Over Screen showed up and showed score
    public void StopGame(int score)
    {
        Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = score.ToString();

    }

    // This WinGame function operates on an old scoring system but would ensure that the Game Win Screen showed up and showed score
    public void WinGame(int score)
    {
        Time.timeScale = 0f;
        gameWinCanvas.SetActive(true);
        this.score = score;
        scoreText.text = score.ToString();

    }

    // This ExitGame function allows for the game to be quit
    public void ExitGame()
    {
        #if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // This RestartLevel function allows for the current game scene to be restarted to allow replayability
    public void RestartLevel()
    {
        gameOverCanvas.SetActive(false);
        gameWinCanvas.SetActive(false);
        SceneManager.LoadScene("MainGameScene");

    }

}
