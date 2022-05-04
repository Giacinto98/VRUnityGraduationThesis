using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComandoPlayer : MonoBehaviour
{
    public VideoClipManager.ActionPlayer azionePlayer;
    [HideInInspector]
    public UnityEvent<VideoClipManager.ActionPlayer> evento;


    void Start()
    {
        
    }
    
    void Update()
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
