using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    private int score = 0;




    public void StopGame(int score)
    {
        Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = score.ToString();

    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void RestartLevel()
    {
        gameOverCanvas.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene("TrenchRunner");
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("TrenchRunner"));
    }

}
