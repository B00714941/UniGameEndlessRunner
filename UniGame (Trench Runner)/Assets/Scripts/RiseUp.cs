using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RiseUp : MonoBehaviour
{
    private Vector3 StopAt = new Vector3(-130, 0, 456);
    public float movementSpeed = 60f;
    private Transform target;


    //This introduces a fun little intro note that gives a brief thematic tutorial by rising up from below screen
    void Update()
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, StopAt, movementSpeed * Time.deltaTime);
        transform.position = newPos;
 
    }

    //Click the begin button once you've read the text to start the game
    public void BeginGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

}
