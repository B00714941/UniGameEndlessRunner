using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenuCanvas;
    [SerializeField]
    private GameObject SpawnManager;
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject PlayerGhost;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuCanvas.SetActive(true);
        SpawnManager.SetActive(false);
        Player.SetActive(false);
        PlayerGhost.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
