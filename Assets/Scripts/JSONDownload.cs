using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class JSONDownload : MonoBehaviour
{
    //stringa che conterra l'url di richiesta
    [SerializeField] private string textURL; 
    
    //classe che conterra i dati della stringa JSON 
    [System.Serializable]
    public class Vods
    {
        public List<Vod> video;
    }

    [System.Serializable]
    public class Vod
    {
        public string name;
        public string urlS3;
        public string urlCDN;
        public int year;
        public string category;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //alla pressione del tasto spazio
        {
            //La coroutine serve a eseguire l'operazione su piu fotogrammi in modo tale da non bloccare l'0esecuzione di altre operazioni
            StartCoroutine(GetText()); 
            //Nel caso di una richiesta HTTP conviene avviare una coroutine
        }
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
                //Lo stampiamo in cosnole
                Debug.LogError(request.error); 
            }
            else
            {
                //downloadHandler gestisce la response del seerver
                var text = request.downloadHandler.text; 
                Vods vods = JsonUtility.FromJson<Vods>(text); //converte il codice JSON in una classe FACT che abbiamo definito sopra
                //stampa di prova da eliminiare
                Debug.Log(vods.video[0].name);
                //Capire come farlo visualizzare come testo
            }
    }
}