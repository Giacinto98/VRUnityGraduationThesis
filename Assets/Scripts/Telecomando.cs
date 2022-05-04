using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Telecomando : MonoBehaviour
{
    public OVRInput.Button inputId;
    private OVRGrabbable grabbable;
    public GameObject RHand;
    public GameObject LHand;
    private LineRenderer raggio;
     void Start()
    {
        grabbable = GetComponent<OVRGrabbable>(); //prendo le componenti relative allo script OVRGrabbale
        raggio = GetComponentInChildren<LineRenderer>(); //prende le componenti del raggio rosso
    }

    // Update is called once per frame
    void Update()
    {   
        if (grabbable) //se grabber non è null
        {
            if(raggio) //se il raggio non è null
            {
                raggio.enabled = grabbable.isGrabbed; //attivo il raggio solo se l'oggetto è preso
            }
            if (grabbable.isGrabbed) //se ho in mano il telecomando
            {
                ComandoPlayer comando = null;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 10f)) //se rileva una collisione entro 10 metri, mette le informazione sulla collisione in un Raycast Object
                {
                   comando  = hit.collider.GetComponent<ComandoPlayer>();
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

                if(OVRInput.GetDown(inputId, grabController))
                {
                    if (comando)
                    {
                        comando.Execute();
                    }
                }
            }
        }
    }
}