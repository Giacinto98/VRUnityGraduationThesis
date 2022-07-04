using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComandoPlayer : MonoBehaviour
{
    public VideoClipManager.ActionPlayer azionePlayer;
    public UnityEvent<VideoClipManager.ActionPlayer> evento;


    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void CambiaIcona(Image nuovaImmagine)
    {
            
    } 

    public void Execute()
    {
        if(evento != null)
        {
            evento.Invoke(azionePlayer);
        }
    }

    private void OnDestroy()
    {
        evento.RemoveAllListeners();
    }
}
