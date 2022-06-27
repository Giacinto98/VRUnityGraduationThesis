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
    private ComandoPlayer comando = null;
    
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

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, distance, mask)) //se rileva una collisione entro 10 metri, mette le informazione sulla collisione in un Raycast Object
                {
                    cast = hit.collider.gameObject;
                    debug.text = cast.name;
                    /*
                    Creo una variabile ComandoPlayer a cui assegno l'elemento con cui ha colliso il raggio
                    Controllo quindi che sia diversa da comando, perchè se mi sto spostando sullo stesso comando
                    non deve essere effettuata una nuova vibrazione.  
                    */
                    ComandoPlayer comandoPlayer = hit.collider.GetComponent<ComandoPlayer>(); 
                    /*
                    comando sarà diverso da null solo quando collide con un oggetto che è nel Layer RaycastHit
                    poichè ho impostato questo vincolo per evitare che il raggio collidesse con altri oggetti 
                    inutili o disturbatori
                    */
                    if(comando != comandoPlayer) 
                    {
                        if (hapticFeedback == null)
                        {
                        hapticFeedback = HapticFeedback(grabController);
                        StartCoroutine(hapticFeedback);
                        }
                    }
                    comando = comandoPlayer;
                    raggio.SetPosition(1, hit.point);
                }
                else
                {
                    comando = null; //setto a null il comando in modo da evitare che il joipad vibri all'infinito
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

    /*
    Coroutine che serve a bloccare il feedback aptico altrimetni durerebbe un secondo
    La coroutine è impostata a 1 millisecondo.
    */
    private IEnumerator hapticFeedback = null;
    private IEnumerator HapticFeedback(OVRInput.Controller grabController)
    {
        OVRInput.SetControllerVibration(1, 1, grabController);
        yield return new WaitForSeconds(0.01f);
        OVRInput.SetControllerVibration(0, 0, grabController);
        hapticFeedback = null;
    }
}