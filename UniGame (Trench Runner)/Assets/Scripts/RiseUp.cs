using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RiseUp : MonoBehaviour
{
    private Vector3 StopAt = new Vector3(-197, 0, 456);
    public float movementSpeed = 60f;
    private Transform target;

    // Start is called before the first frame update
    void Update()
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, StopAt, movementSpeed * Time.deltaTime);
        transform.position = newPos;
        //StartCoroutine(IntroCoroutine());
    }

    public void BeginGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    // Update is called once per frame
    //IEnumerator IntroCoroutine()
    //{


    // yield return new WaitForSeconds(20);
    // SceneManager.LoadScene("MainGameScene");


    //}
}
