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
        //Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = score.ToString();

    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
