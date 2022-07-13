using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;

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
    bool isStopped = false;
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
        //La coroutine serve a eseguire l'operazione su piu fotogrammi in modo tale da non bloccare l'0esecuzione di altre operazioni
        StartCoroutine(GetText()); 
        //Nel caso di una richiesta HTTP conviene avviare una coroutine 
        //debug.text = "";
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
            //debug.text = "Click tasto Advance";
            SkipVideo(+15.0f);
        }

        if (input == ActionPlayer.NextClip)
        {
            //debug.text = "Click tasto NextClip";
            ChangeClip(1);
        }

        else if (input == ActionPlayer.PrevClip)
        {
            //debug.text = "Click tasto PrevClip";
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
                //debug.text = "Video in esecuzione";
                videoPlayer.Play();
                isStopped = false;
                break;
            case State.Pause:
                //debug.text = "Video in pausa";
                videoPlayer.Pause();
                isStopped = false;
                break;
            case State.Stop:
                if(isStopped)
                {
                    //debug.text = "Il video è già stoppato";
                }
                else
                {
                    //debug.text = "Video Stoppato";
                    isStopped = true;
                }
                videoPlayer.Stop();
                break;
            case State.Preparing:
                //debug.text = "Video in preparazione, attendi qualche secondo...";
                isStopped = false;
                videoPlayer.Prepare();
                break;
        }
        videoState = stato;
    }

    /*
    Quando si itera attraverso una collezione o si accede a un file di grandi dimensioni, 
    l'attesa dell'intera azione bloccherebbe tutte le altre, IEnumerator consente di 
    interrompere il processo in un momento specifico, di restituire quella parte di oggetto 
    (o nulla) e di ritornare a quel punto ogni volta che se ne ha bisogno.
    */
    private IEnumerator GetText()
    {
        //richiesta HTTP con metodo get
        UnityWebRequest request = UnityWebRequest.Get(textURL); 
        //L'esecuzione viene riavviata a partire da quella posizione la volta successiva che viene chiamata la funzione iteratore.
        yield return request.SendWebRequest(); 
        //se si e verificato un errore nella richiesta HTTP 
        if (request.isHttpError || request.isNetworkError) 
        {
            //debug.text = "Richiesta di un nuovo video effettuata";
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
                videoPlayer.url = vods.video[0].urlCDN;
                //debug.text = "Collegamento al video pronto";
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