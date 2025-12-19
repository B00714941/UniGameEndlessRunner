using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RiseUp : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    //This introduces a fun little intro note that gives a brief thematic tutorial by rising up from below screen
    //MATHS CONTENT - VECTORS 2 - LERP
    void Update()
    {

        float t = 0.005f;

        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
 
    }

    //Click the begin button once you've read the text to start the game
    public void BeginGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

}
