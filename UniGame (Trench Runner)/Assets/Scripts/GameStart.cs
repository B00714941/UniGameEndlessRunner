using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenuCanvas;


    // This is just to ensure that the MainMenuCanvas activates when the game begins
    void Start()
    {
        MainMenuCanvas.SetActive(true);

    }

}
