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
    public Text debug;
    public LayerMask mask;
    public float distance = 5f;
    GameObject cast;
     
    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>(); //prendo le componenti relative allo script OVRGrabbale
        raggio = GetComponentInChildren<LineRenderer>(); //prende le componenti del raggio rosso
    }

    // Update is called once per frame
    void Update()
    { 
   
        if (grabbable) //se grabber non � null
        {
            if(raggio) //se il raggio non � null
            {
                raggio.enabled = grabbable.isGrabbed; //attivo il raggio solo se l'oggetto � preso
                raggio.SetPosition(0, transform.position);
                raggio.SetPosition(1, transform.position + transform.forward * distance);
            }
            if (grabbable.isGrabbed) //se ho in mano il telecomando
            {
                ComandoPlayer comando = null;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, distance, mask)) //se rileva una collisione entro 10 metri, mette le informazione sulla collisione in un Raycast Object
                {
                    cast = hit.collider.gameObject;
                    debug.text = cast.name;
                    comando = hit.collider.GetComponent<ComandoPlayer>();
                    raggio.SetPosition(1, hit.point);
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