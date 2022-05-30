using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;
using Newtonsoft.Json;
using UnityEngine.Networking;
using TMPro;

public class VideoClipManager : MonoBehaviour
{
    ComandoPlayer[] array;
    private enum State { Init, Preparing, Play, Pause, Stop };
    private State videoState = State.Init;
    private VideoPlayer videoPlayer;
    private int indiceArrayVideo = 0;
    public enum ActionPlayer { None, PlayPause, Stop, NextClip, PrevClip, VolumeUp, VolumeDown, VolumeMute, Behind, Advance};
    public Image progressBar;
    private Vods vods;
    //stringa che conterra l'url di richiesta
    [SerializeField] 
    private string textURL;
    //classe che conterra i dati della stringa JSON 
    [System.Serializable]
    public partial class Vods
    {
        [JsonProperty("vods")]
        public List<Vod> video;

    }

    [System.Serializable]
    public partial class Vod
    {
        public string name;
        public string urlS3;
        public string urlCDN;
        public int year;
        public string category;
    }

    void Start()
    {
        array = transform.parent.GetComponentsInChildren<ComandoPlayer>(); //mette in un array tutti i comandi ricevuti dal telecomando
        foreach (ComandoPlayer a in array)
        {
            a.evento.AddListener(takeInput); //collego al comando l'evento che lancia metodo takeInput
        }
        videoPlayer = GetComponent<VideoPlayer>(); //prendo le componenti del videoPlayer
        StartCoroutine(GetText());
    }

    void Update()
    {
        switch (videoState)
        {
            case State.Init:
                break;

            case State.Preparing:
                if (videoPlayer.isPrepared)
                {
                    ChangeState(State.Play);
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space)) //alla pressione del tasto spazio
        {
            takeInput(ActionPlayer.Advance);
        }
        
        if(progressBar != null)
        {
            if(videoPlayer.frameCount > 0)
            {
                progressBar.fillAmount = (float)videoPlayer.frame / (float) videoPlayer.frameCount;
            }
        }   
    }


    public void takeInput(ActionPlayer input) 
    {
        if (input == ActionPlayer.Behind)
        {
            SkipVideo(-15.0f);
        }

        if (input == ActionPlayer.Advance)
        {
            SkipVideo(+15.0f);
        }

        if (input == ActionPlayer.NextClip)
        {
            ChangeClip(1);
        }

        else if (input == ActionPlayer.PrevClip)
        {
            ChangeClip(-1);
        }

        switch (videoState)
        {
            case State.Stop:
                if (input == ActionPlayer.PlayPause)
                {
                    ChangeState(State.Play);
                }
                break;

            case State.Play:
                if (input == ActionPlayer.PlayPause)
                {
                    ChangeState(State.Pause);
                }
                else if (input == ActionPlayer.Stop)
                {
                    ChangeState(State.Stop);
                }

                break;

            case State.Pause:
                if (input == ActionPlayer.PlayPause)
                {
                    ChangeState(State.Play);
                }
                else if (input == ActionPlayer.Stop)
                {
                    ChangeState(State.Stop);
                }
                break;
        }

    }

    void ChangeClip(int direction)
    {
        indiceArrayVideo = (indiceArrayVideo + direction) % vods.video.Count;

        

        if(indiceArrayVideo < 0)
        {
            indiceArrayVideo = vods.video.Count - 1;
        }
        videoPlayer.url = vods.video[indiceArrayVideo].urlCDN;
        ChangeState(State.Preparing);
    }

    void ChangeState(State stato)
    {
        switch (stato)
        {
            case State.Play:
                videoPlayer.Play();
                break;
            case State.Pause:
                videoPlayer.Pause();
                break;
            case State.Stop:
                videoPlayer.Stop();
                break;
            case State.Preparing:
                videoPlayer.Prepare();
                break;
        }
        videoState = stato;
    }

    private IEnumerator GetText()
    {
        //richiesta HTTP con metodo get
        UnityWebRequest request = UnityWebRequest.Get(textURL); 
        //L'esecuzione viene riavviata a partire da quella posizione la volta successiva che viene chiamata la funzione iteratore.
        yield return request.SendWebRequest(); 
        //se si e verificato un errore nella richiesta HTTP 
        if (request.isHttpError || request.isNetworkError) 
        {
            //Lo stampiamo in cosnole
            Debug.LogError(request.error); 
        }
        else
        {
            //downloadHandler gestisce la response del seerver
            var text = request.downloadHandler.text;
            vods = JsonConvert.DeserializeObject<Vods>(text); //converte il codice JSON in una classe FACT che abbiamo definito sopra
            //stampa di prova da eliminiare
            if(vods != null) 
            {
                //Debug.Log(vods.video[0].name);
                videoPlayer.url = vods.video[0].urlCDN;
                ChangeState(State.Preparing);
            }
        }
    }

    void SkipVideo(float value)
    {
        double newTime = videoPlayer.time + value;
        if(newTime < 0)
            newTime = 0f;
        else if (newTime >= videoPlayer.length)
        {
            newTime = videoPlayer.length;
        }        
        videoPlayer.time = newTime;
    }
}
