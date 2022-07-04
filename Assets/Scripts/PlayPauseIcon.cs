using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPauseIcon : MonoBehaviour
{
    public GameObject playIcon;
    public GameObject pauseIcon;

    public void ShowPlay()
    {
        if(playIcon)
            playIcon.SetActive(true);
        if(pauseIcon)
            pauseIcon.SetActive(false);       
    }

    public void SwitchIcons ()
    {
        if(playIcon)
            playIcon.SetActive(!playIcon.activeInHierarchy);
        if(pauseIcon)
            pauseIcon.SetActive(!pauseIcon.activeInHierarchy);
    }
}