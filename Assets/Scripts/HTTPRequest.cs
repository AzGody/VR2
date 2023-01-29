using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class HTTPRequest : MonoBehaviour
{

    public Color c1 = Color.red;
    public Color c2 = Color.red;

    public GameObject moniteur; // moniteur dans lequel sera affiché la courbe
    // public TMP_Text panneau; // panneau pour afficher du texte
    private LineRenderer lineRenderer;
    private ArrayList datas = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        moniteur = GameObject.Find("Moniteur");
        // initialisation d'un LineRenderer (pour affichage des courbes)
        lineRenderer = moniteur.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.005f;
        lineRenderer.useWorldSpace = true;
        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        // lancement des requêtes
        Debug.Log("Lancement des requêtes");
        string uri = "https://www.labri.fr/perso/pecher/RV/rv_advanced.php";
        StartCoroutine(getRequest(uri));
    }

    void Update()
    {
        int nbPoints =  datas.Count;
        lineRenderer.positionCount = nbPoints;
        var points = new Vector3[nbPoints];
        // affichage des points 
        // calcul de la moyenne, du max, du min des valeurs reçues
        float somme = 0f;
        float max = 0f;
        float min = 200f;
        for (int i=0; i<nbPoints; i++){
            float val = (int) datas[i];
            somme +=  val;
            if (val > max)
                max = val;
            if (val < min)
                min = val;
        }
        float moyenne = somme / nbPoints;
        // calcul des points (recalage dans le moniteur)
        Renderer rend = moniteur.GetComponent<Renderer>(); // boite englobante du moniteur
        float xmin = rend.bounds.min.x; float xmax = rend.bounds.max.x;
        float ymin = rend.bounds.min.y; float ymax = rend.bounds.max.y;
        float zmin = rend.bounds.min.z; float zmax = rend.bounds.max.z;
        for (int i=0; i<nbPoints; i++){
            var val = (int) datas[i];
            var x = xmin + ((xmax-xmin) * i)/nbPoints;
            var y = ymin + (ymax-ymin) * ((val-min)/(max-min));
            var z = zmin + ((zmax-zmin) * i)/nbPoints;
            points[i] = new Vector3(x,y,z);
        }
        // affichage des lignes aves le lineRenderer
        lineRenderer.SetPositions(points);
        // affichage de la moyenne
        string time = System.DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss");
        string affichage = time + " : " + (int) moyenne + " BPM";
        // panneau.text = affichage;
    }

     IEnumerator getRequest(string uri)
    {       
        UnityWebRequest uwr;
            while(true){
                uwr = UnityWebRequest.Get(uri);
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError){
                   Debug.Log("Error While Sending: " + uwr.error);
                }
                else{
                    string contenu = uwr.downloadHandler.text;
                    Debug.Log("Received: " + contenu);
                    datas = new ArrayList();
                    // analyse du contenu
                    char[] delims = {',','[',']',':',' ','\n'};
                    string[] tokens = contenu.Split(delims);
                    
                    for (int i=3; i<tokens.Length; i++){
                        try{
                            float val = float.Parse(tokens[i]);
                            datas.Add( (int) val);
                        } catch (FormatException) {}
                    }
                    
                }
                yield return new WaitForSeconds(5.0f);
            }    
    }

}

// using System.Collections;
// using System.Collections.Generic;
// using System;
// using UnityEngine;
// using UnityEngine.Networking;
// using TMPro;

// public class HTTPRequest : MonoBehaviour
// {private TMP_Text m_TextComponent;
//     // public GameObject moniteur; // moniteur dans lequel sera affiché la courbe
//     // public TMP_Text panneau;
//     void Start()
//     {
//           m_TextComponent = GetComponent<TMP_Text>();
//           m_TextComponent.text = "Some new line of text.";
//     }
// }