using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;

public class VideoClipManager : MonoBehaviour
{
    ComandoPlayer[] array;
    private enum State { Init, Preparing, Play, Pause, Stop };
    private State videoState = State.Init;
    private VideoPlayer videoPlayer;
    public Text debug;
    public enum ActionPlayer { None, PlayPause, Stop, NextClip, PrevClip, VolumeUp, VolumeDown, VolumeMute };


    void Start()
    {
        array = GetComponentsInChildren<ComandoPlayer>(); //mette in un array tutti i comandi ricevuti dal telecomando
        foreach (ComandoPlayer a in array)
        {
            a.evento.AddListener(takeInput); //collego al comando l'evento che lancia metodo takeInput
        }
        videoPlayer = GetComponent<VideoPlayer>(); //prendo le componenti del videoPlayer
    }

    void Update()
    {
        switch (videoState)
        {
            case State.Init:
                if (videoPlayer)
                {
                    if (videoPlayer.clip)
                    {
                        ChangeState(State.Play);
                    }
                }
                break;

            case State.Preparing:
                if (videoPlayer.isPrepared)
                {
                    ChangeState(State.Play);
                }
                else
                {
                    debug.text = "In preparazione";
                }
                break;
        }
    }


    public void takeInput(ActionPlayer input) 
    {
        if (input == ActionPlayer.NextClip)
        {
            ChangeClip("VideoDue");
        } 
        else if (input == ActionPlayer.PrevClip)
        {
            ChangeClip("VideoTerra");
        }

        switch (videoState)
        {
            case State.Stop:
                if (input == ActionPlayer.PlayPause)
                {
                    debug.text = "Click tasto 'A'";
                    ChangeState(State.Play);
                }
                break;

            case State.Play:
                if (input == ActionPlayer.PlayPause)
                {
                    debug.text = "Click tasto 'A'";
                    ChangeState(State.Pause);
                }
                else if (input == ActionPlayer.Stop)
                {
                    debug.text = "Click tasto 'B'";
                    ChangeState(State.Stop);
                }

                break;

            case State.Pause:
                if (input == ActionPlayer.PlayPause)
                {
                    debug.text = "Click tasto 'A'";
                    ChangeState(State.Play);
                }
                else if (input == ActionPlayer.Stop)
                {
                    debug.text = "Click tasto 'B'";
                    ChangeState(State.Stop);
                }
                break;
        }

    }

    void ChangeClip(string fileName)
    {
        videoPlayer.clip = (VideoClip)Resources.Load("Video/" + fileName);
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
}
