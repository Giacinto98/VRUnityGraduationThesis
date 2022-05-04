using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoClipManager : MonoBehaviour
{
    private enum State { Init, Preparing, Play, Pause, Stop };
    private State videoState = State.Init;
    private VideoPlayer videoPlayer;
    public Text debug;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
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


    public void takeInput(Telecomando.InputTelecomando input) 
    {
        if (input == Telecomando.InputTelecomando.NextClip)
        {
            ChangeClip("VideoDue");
        } 
        else if (input == Telecomando.InputTelecomando.PrevClip)
        {
            ChangeClip("VideoTerra");
        }

        switch (videoState)
        {
            case State.Stop:
                if (input == Telecomando.InputTelecomando.PlayPause)
                {
                    debug.text = "Click tasto 'A'";
                    ChangeState(State.Play);
                }
                break;

            case State.Play:
                if (input == Telecomando.InputTelecomando.PlayPause)
                {
                    debug.text = "Click tasto 'A'";
                    ChangeState(State.Pause);
                }
                else if (input == Telecomando.InputTelecomando.Stop)
                {
                    debug.text = "Click tasto 'B'";
                    ChangeState(State.Stop);
                }

                break;

            case State.Pause:
                if (input == Telecomando.InputTelecomando.PlayPause)
                {
                    debug.text = "Click tasto 'A'";
                    ChangeState(State.Play);
                }
                else if (input == Telecomando.InputTelecomando.Stop)
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
