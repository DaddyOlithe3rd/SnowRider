using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGeneration : MonoBehaviour
{
    //Déclaration des variables pour les obstacles
    public GameObject tree;
    public GameObject rock;

    //Points est le nombre de valeurs d'interpolations à calculer pour tracer les courbes
    public int points;

    //Target correspond à la caméra qui suit le personnage.  Elle est utilisée dans la génération et la suppression du terrain
    public Transform target;

    EdgeCollider2D edgeCollider;

    //Valeurs utilisées pour le générateur de nombres pseudo-aléatoires
    float a = 1664525;
    float c = 1013904223;
    float m = 2 ^ 32;
    float point = 34920;
    float position;

    //Variables utilisées pour l'interpolation entre les points générés
    float x1;
    float x2;
    float x3;
    float y1;
    float y2;
    float y3;
    float a1;
    float a2;
    float b1;
    float b2;
    float q1;
    float q2;
    float t;

    bool start;

    //Timer pour la génération d'obstacles et de sauts
    float jumptime = 0;
    float obstacletime = 0;

    //List qui reçoit les courbes créées
    List<KeyValuePair<GameObject, float>> list_of_object = new List<KeyValuePair<GameObject, float>>();

    // Start is called before the first frame update
    void Start()
    {
        start = true;

        //La première courbe commence en (0,0)
        x1 = 0;
        y1 = 0;

        //La distance entre chaque x est aléatoire, alors que les y sont générés pseudo-aléatoirement
        x2 = Random.Range(1f, 6f);
        point = NouveauPoint(point);
        y2 = MapPoint(point, 1, -4);

        x3 = x2 + Random.Range(1f, 6f);
        y3 = MapPoint(NouveauPoint(point), 1, -4);

        //Interpolation entre les points générés
        Interpolation(x1, x2, x3, y1, y2, y3, start);

        start = false;
    }

    // Update is called once per frame
    void Update()
    {
        //La création de nouveaux segements (courbes) de terrain s'effectue selon la position de la caméra.
        if (x3 < (target.position.x + 20))
        {
            //Si le timer pour le saut arrive plus haut que 10, un saut va être créé en ajoutant un distance à x2.
            //En ajoutant un distance à x2, on créé un intervalle pour lequel il n'y aura aucun terrain de généré
            if (jumptime > 10)
            {
                x1 = x2;
                y1 = y2;

                x2 = x3 + 4;
                y2 = y3 - 5;

                x3 += Random.Range(8f, 12f);
                y3 = y2 - 3;

                Interpolation(x1, x2, x3, y1, y2, y3, start);
                jumptime = 0;
            }
            else if (obstacletime == 6)
            {
                //Si le timer obstacle est égal à 6, on lance génère entre 2 et 6 obstacles
                Debug.Log("obstacle");
                ObstacleGeneration(Random.Range(2, 6));
                obstacletime = 0; //Important de remettre le timer à 0 une fois la génération complété
            }
            else
            {
                //S'il n'y a pas de saut ni d'obstacle, on génère les points normalement
                x1 = x2;
                x2 = x3;
                y1 = y2;
                y2 = y3;
                x3 += Random.Range(2f, 6f);

                //Permet de faire un effet de saut juste avant l'intervalle de non génération de terrain
                //Sinon le nouveau point est généré normalement
                if (jumptime == 10)
                {
                    y3 = y2 + 1;
                }
                else
                {
                    
                    y3 = MapPoint(NouveauPoint(y3), (y2 - Random.Range(1.0f, 2.0f)), (y2 - Random.Range(-1.0f, 2.0f))); //Le nouveau point est ajusté entre un minimum et un maximum déterminé aléatoirement, mais en tenant compte du point précédant
                }
                
                //On effectue l'interpolation entre les points déterminés
                Interpolation(x1, x2, x3, y1, y2, y3, start);

                //On ajoute 1 au timer du saut et des obstacles
                jumptime++;
                obstacletime++;
            }
        }

        //Selon la position de la caméra, les game objects seront supprimés s'ils sont en dehors du champ.
        for (int i = 0; i < list_of_object.Count; i++)
        {
            if (list_of_object[i].Value < (target.transform.position.x - 50))
            {
                Destroy(list_of_object[i].Key);
                list_of_object.RemoveAt(i);
            }
        }
    }

    //Générateur de nombres pesudo-aléatoires prend un nombre en argument et en retourne un nouveaux.
    public float NouveauPoint(float point)
    {
        position = (a * point + c) % m;
        position /= m;
        return position;
    }

    //La fonction Map donne une nouvelle valeur du point entre un min et un max prédifini.
    //Elle sert entre autre à s'assurer qu'il n'y ait pas de grosses pentes ascendantes que le skieur ne peut franchir.
    public float MapPoint(float point, float min, float desiredMax)
    {
        point *= (desiredMax - min);
        point += min;
        return point;
    }

    //La fonction Interpolation sert à effectuer les calculs permettant de détermnier les équations des courbes.
    public void Interpolation(float val1, float val2, float val3, float val4, float val5, float val6, bool start)
    {
        x1 = val1;
        x2 = val2;
        x3 = val3;

        y1 = val4;
        y2 = val5;
        y3 = val6;

        //Déclaration des matrices nécessaires aux calculs
        float[,] matrice = new float[3, 3];
        float[] resultat = new float[3];
        float[] matriceb = new float[3];
        float[,] inverse = new float[3, 3];

        //Détermination des valeurs des éléments de la matrice
        matrice[0, 0] = 2 / (x2 - x1);
        matrice[0, 1] = 1 / (x2 - x1);
        matrice[1, 0] = 1 / (x2 - x1);
        matrice[1, 1] = 2 * (1 / (x2 - x1) + 1 / (x3 - x2));
        matrice[1, 2] = 1 / (x3 - x2);
        matrice[2, 1] = 1 / (x3 - x2);
        matrice[2, 2] = 2 / (x3 - x2);

        //Détermination des valeurs des éléments de la matrice b
        matriceb[0] = 3 * (y2 - y1) / (Mathf.Pow((x2 - x1), 2));
        matriceb[1] = matriceb[0] + (3 * (y3 - y2) / (Mathf.Pow((x3 - x2), 2)));
        matriceb[2] = 3 * (y3 - y2) / (Mathf.Pow((x3 - x2), 2));

        //Calcul du déterminant
        float det = 0;
        det = matrice[0, 0] * (matrice[1, 1] * matrice[2, 2] - matrice[2, 1] * matrice[1, 2]) - matrice[0, 1] * (matrice[1, 0] * matrice[2, 2]);

        //Calcul de l'adjointe de la matrice
        inverse[0, 0] = matrice[1, 1] * matrice[2, 2] - matrice[2, 1] * matrice[1, 2];
        inverse[1, 0] = -1 * (matrice[1, 0] * matrice[2, 2]);
        inverse[2, 0] = matrice[0, 1] * matrice[2, 1];

        inverse[0, 1] = -1 * (matrice[0, 1] * matrice[2, 2]);
        inverse[1, 1] = matrice[0, 0] * matrice[2, 2];
        inverse[2, 1] = -1 * (matrice[0, 0] * matrice[2, 1]);

        inverse[0, 2] = matrice[0, 1] * matrice[1, 2];
        inverse[1, 2] = -1 * (matrice[0, 0] * matrice[1, 2]);
        inverse[2, 2] = matrice[0, 0] * matrice[1, 1] - matrice[1, 0] * matrice[0, 1];

        //Calcul de la matrice résultat (correspond à 1/det * matrice adjointe * matrice b)
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                resultat[i] += (1 / det) * inverse[i, j] * matriceb[j];
            }
        }

        //Détermination des paramètres utilisés dans les équations des courbes
        a1 = resultat[0] * (x2 - x1) - (y2 - y1);
        a2 = resultat[1] * (x3 - x2) - (y3 - y2);
        b1 = -resultat[0] * (x2 - x1) + (y2 - y1);
        b2 = -resultat[1] * (x3 - x2) + (y3 - y2);

        //Appel de la fonction tracer pour construire les courbes selon les équations
        Tracer(start);
    }

    //Équation de la courbe entre le point 1 et 2
    public float Equation1(float x)
    {
        t = (x - x1) / (x2 - x1);
        q1 = (1 - t) * y1 + t * y2 + t * (1 - t) * ((1 - t) * a1 + t * b1);
        return q1;
    }

    //Équation de la courbe entre le point 2 et 3
    public float Equation2(float x)
    {
        t = (x - x2) / (x3 - x2);
        q2 = (1 - t) * y2 + t * y3 + t * (1 - t) * ((1 - t) * a2 + t * b2);
        return q2;
    }

    public void Tracer(bool start)
    {
        //Position du début et de la fin de la courbe
        float xdebut;
        float xfin;

        //Créer un nouveau gameObject qui agira comme la ligne d'une courbe
        GameObject line = (new GameObject("line"));
        line.AddComponent<LineRenderer>(); //Ajout du composant LineRenderer au gameObject
        line.GetComponent<LineRenderer>().startWidth = 0.2f; //Épaisseur de la ligne
        line.GetComponent<LineRenderer>().endWidth = 0.2f;
        line.GetComponent<LineRenderer>().positionCount = points;
        line.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        line.GetComponent<LineRenderer>().startColor = Color.white; // Couleur de la courbe
        line.GetComponent<LineRenderer>().endColor = Color.white;
        line.layer = 10; //La courbe se situe dans le layer 10 "Ground" pour gérer les collisions avec les skieurs

        //La première section diffère des autres parce qu'elle fait intervenir l'équation 1
        if (start == true)
        {
            //Créer un game
            GameObject line2 = (new GameObject("line"));
            line2.AddComponent<LineRenderer>(); //Ajout du composant LineRenderer au gameObject
            line2.GetComponent<LineRenderer>().startWidth = 0.2f; //Épaisseur de la courbe
            line2.GetComponent<LineRenderer>().endWidth = 0.2f;
            line2.GetComponent<LineRenderer>().positionCount = points;
            line2.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            line2.GetComponent<LineRenderer>().startColor = Color.white; // Couleur de la courbe
            line2.GetComponent<LineRenderer>().endColor = Color.white;
            line2.layer = 10; //La courbe se situe dans le layer 10 "Ground" pour gérer les collisions avec les skieurs
            xdebut = x1;
            xfin = x2;

            //Tracer la courbe selon l'équation 1
            //L'équation 1 est utilisée seulement pour la première courbe
            for (int position = 0; position < points; position++)
            {
                float progress = (float)position / (points - 1);
                float x = xdebut + (xfin - xdebut) * progress;
                float y = Equation1(x);
                line2.GetComponent<LineRenderer>().SetPosition(position, new Vector3(x, y, 0));
            }

            //Ajout des colliders à la courbe
            line2.AddComponent<EdgeCollider2D>();
            edgeCollider = line2.GetComponent<EdgeCollider2D>();
            SetCollider(line2);

            //Ajout de la ligne dans la list pour qu'elle soit supprimée lorsqu'elle sera out of range
            list_of_object.Add(new KeyValuePair<GameObject, float>(line2, xdebut));  
        }

        xdebut = x2;
        xfin = x3;

        //Tracer la courbe selon l'équation 2
        for (int position = 0; position < points; position++)
        {
            float progress = (float)position / (points - 1);
            float x = xdebut + (xfin - xdebut) * progress;
            float y = Equation2(x);
            line.GetComponent<LineRenderer>().SetPosition(position, new Vector3(x, y, 0));
        }

        //Ajout des colliders à la ligne
        line.gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider = line.GetComponent<EdgeCollider2D>();
        SetCollider(line);

        //Ajout de la ligne dans la list pour qu'elle soit supprimée lorsqu'elle sera out of range
        list_of_object.Add(new KeyValuePair<GameObject, float>(line.gameObject, xdebut));
    }

    //La fonction SetCollider permet d'ajouter un collider aux courbes afin que le joueur puisse skier dessus et non passer au travers
    public void SetCollider(GameObject ligne)
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < ligne.gameObject.GetComponent<LineRenderer>().positionCount; point++)
        {
            //Ajoute un vecteur à la liste pour chaque point qui compose les courbes
            Vector3 lineRendererPoint = ligne.gameObject.GetComponent<LineRenderer>().GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        //Ajoute un collider selon chaque vecteur contenu dans la liste
        edgeCollider.SetPoints(edges);
    }

    //La fonction ObstacleGeneration permet de générer différents obstacles à différentes positions sur la courbe de Perlin.
    public void ObstacleGeneration(int nbObstacles)
    {
        //La boucle while sert à répéter la génération d'obstacles pour en obtenir un certain nombre prédéterminé
        while (nbObstacles > 0)
        {
            x1 = x2;
            x2 = x3;
            x3 += Random.Range(20.0f, 30.0f);

            y1 = y3;
            y2 = y3;
            y3 -= Random.Range(1.0f, 3.0f);

            //L'interpolation selon les points calculés ci-haut permet d'obtenir une courbe dont la pente n'est pas extrême pour que le jeu soit "jouable"
            Interpolation(x1, x2, x3, y1, y2, y3, start);

            //Génération d'un arbre ou d'une roche selon le hasard
            if (Random.Range(0, 2) == 0)
            {
                Instantiate(tree, new Vector3(x3, y3 + 2.0f, 0), transform.rotation); //Fait apparaitre un arbre sur la courbe selon l'objet préfabriqué
                //list_of_object.Add(new KeyValuePair<GameObject, float>(arbre, x2));
            }
            else
            {
                Instantiate(rock, new Vector3(x3, y3 + 0.5f, 0), transform.rotation); //Fait apparaitre une roche sur la courbe selon l'object préfabriqué
                //Debug.Log("Roche");
            }
            nbObstacles--;
        }
    }
}