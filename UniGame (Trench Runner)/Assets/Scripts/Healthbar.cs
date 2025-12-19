using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image healthbar;

    // this allows for the healthbar image on the UI to be filled in a gradual slider

    public void UpdateHealth(float fraction)
    {
        healthbar.fillAmount = fraction;
    }


}
