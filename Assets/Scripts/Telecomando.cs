using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Telecomando : MonoBehaviour
{
    private OVRGrabbable grabbable;
    public GameObject RHand;
    public GameObject LHand;
    private LineRenderer raggio;
    public enum InputTelecomando {None, PlayPause, Stop, NextClip, PrevClip, VolumeUp, VolumeDown, VolumeMute};
    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>(); //prendo le componenti relative allo script OVRGrabbale
        raggio = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame s
    void Update()
    {
        VideoClipManager managerInput = null;
       
        if (grabbable) //se grabber ha delle componenti
        {
            if(raggio)
            {
                raggio.enabled = grabbable.isGrabbed; //attivo il raggio solo se l'oggetto è preso
            }
            if (grabbable.isGrabbed) //se ho in mano il telecomando
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 5f)) //ho avuto una collisione
                {
                    managerInput = hit.collider.GetComponent<VideoClipManager>();
                }
                OVRGrabber grabber = grabbable.grabbedBy; //componente che controlla le mani
                OVRInput.Controller grabController = OVRInput.Controller.None;
               
                if (grabber.gameObject.Equals(RHand)) //se sto prendendo con la mano destra
                {
                    grabController = OVRInput.Controller.RTouch; //seleziono gli input su mano destra
                }
                else if (grabber.gameObject.Equals(LHand)) //se sto prendendo con mano sinistra
                {
                    grabController = OVRInput.Controller.LTouch; //seleziono gli input su mano sinistra
                }
                bool play = OVRInput.GetDown(OVRInput.Button.One, grabController);
                bool stop = OVRInput.GetDown(OVRInput.Button.Two, grabController);
                bool nextClip = OVRInput.GetDown(OVRInput.Button.DpadRight, grabController);
                bool prevClip = OVRInput.GetDown(OVRInput.Button.DpadLeft, grabController);
                InputTelecomando inputCorrente = InputTelecomando.None;

                if(play)
                {
                    inputCorrente = InputTelecomando.PlayPause;
                } 
                else if (stop)
                {
                    inputCorrente = InputTelecomando.Stop;
                }
                else if (nextClip)
                {
                    inputCorrente = InputTelecomando.NextClip;
                }
                else if (prevClip)
                {
                    inputCorrente = InputTelecomando.PrevClip;
                }

                if (managerInput)
                {
                    managerInput.takeInput(inputCorrente);
                }
            }
        }
    }
}