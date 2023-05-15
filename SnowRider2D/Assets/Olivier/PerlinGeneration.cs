using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGeneration : MonoBehaviour
{
    //public LineRenderer myLineRenderer;
    public GameObject tree;
    public GameObject rock;
    public int points;
    public Transform target;
    EdgeCollider2D edgeCollider;

    float a = 1664525;
    float c = 1013904223;
    float m = 2 ^ 32;
    float point = 34920;
    float position;
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
    float jumptime = 0;
    float obstacletime = 0;
    List<KeyValuePair<GameObject, float>> list_of_object = new List<KeyValuePair<GameObject, float>>();

    // Start is called before the first frame update
    void Start()
    {
        start = true;
        //myLineRenderer = GetComponent<LineRenderer>();

        x1 = 0;
        y1 = 0;

        x2 = Random.Range(1f, 6f);
        point = NouveauPoint(point);
        y2 = MapPoint(point, 1, -4);

        x3 = x2 + Random.Range(1f, 6f);
        y3 = MapPoint(NouveauPoint(point), 1, -4);

        Interpolation(x1, x2, x3, y1, y2, y3, start);
        start = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (x3 < (target.position.x + 20))
        {
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
                Debug.Log("obstacle");
                ObstacleGeneration(Random.Range(2, 6));
                obstacletime = 0;
            }
            else
            {
                x1 = x2;
                x2 = x3;
                y1 = y2;
                y2 = y3;
                x3 += Random.Range(2f, 6f);

                if (jumptime == 8)
                {
                    y3 = y2 + 1;
                }
                else
                {
                    y2 = y3;
                    y3 = MapPoint(NouveauPoint(y3), (y2 - Random.Range(1.0f, 2.0f)), (y2 - Random.Range(-1.0f, 2.0f)));
                }

                Interpolation(x1, x2, x3, y1, y2, y3, start);
                jumptime++;
                obstacletime++;
            }
        }

        for (int i = 0; i < list_of_object.Count; i++)
        {
            if (list_of_object[i].Value < (target.transform.position.x - 50))
            {
                Destroy(list_of_object[i].Key);
                list_of_object.RemoveAt(i);
            }
        }
    }

    public float NouveauPoint(float point)
    {
        position = (a * point + c) % m;
        position /= m;
        return position;
    }

    public float MapPoint(float point, float min, float desiredMax)
    {
        point *= (desiredMax - min);
        point += min;
        return point;
    }

    public void Interpolation(float val1, float val2, float val3, float val4, float val5, float val6, bool start)
    {
        x1 = val1;
        x2 = val2;
        x3 = val3;

        y1 = val4;
        y2 = val5;
        y3 = val6;

        float[,] matrice = new float[3, 3];
        float[] resultat = new float[3];
        float[] matriceb = new float[3];
        float[,] inverse = new float[3, 3];

        matrice[0, 0] = 2 / (x2 - x1);
        matrice[0, 1] = 1 / (x2 - x1);
        matrice[1, 0] = 1 / (x2 - x1);
        matrice[1, 1] = 2 * (1 / (x2 - x1) + 1 / (x3 - x2));
        matrice[1, 2] = 1 / (x3 - x2);
        matrice[2, 1] = 1 / (x3 - x2);
        matrice[2, 2] = 2 / (x3 - x2);

        matriceb[0] = 3 * (y2 - y1) / (Mathf.Pow((x2 - x1), 2));
        matriceb[1] = matriceb[0] + (3 * (y3 - y2) / (Mathf.Pow((x3 - x2), 2)));
        matriceb[2] = 3 * (y3 - y2) / (Mathf.Pow((x3 - x2), 2));

        //Déterminant
        float det = 0;
        det = matrice[0, 0] * (matrice[1, 1] * matrice[2, 2] - matrice[2, 1] * matrice[1, 2]) - matrice[0, 1] * (matrice[1, 0] * matrice[2, 2]);

        //Adjointe de la matrice
        inverse[0, 0] = matrice[1, 1] * matrice[2, 2] - matrice[2, 1] * matrice[1, 2];
        inverse[1, 0] = -1 * (matrice[1, 0] * matrice[2, 2]);
        inverse[2, 0] = matrice[0, 1] * matrice[2, 1];

        inverse[0, 1] = -1 * (matrice[0, 1] * matrice[2, 2]);
        inverse[1, 1] = matrice[0, 0] * matrice[2, 2];
        inverse[2, 1] = -1 * (matrice[0, 0] * matrice[2, 1]);

        inverse[0, 2] = matrice[0, 1] * matrice[1, 2];
        inverse[1, 2] = -1 * (matrice[0, 0] * matrice[1, 2]);
        inverse[2, 2] = matrice[0, 0] * matrice[1, 1] - matrice[1, 0] * matrice[0, 1];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                resultat[i] += (1 / det) * inverse[i, j] * matriceb[j];
            }
        }

        a1 = resultat[0] * (x2 - x1) - (y2 - y1);
        a2 = resultat[1] * (x3 - x2) - (y3 - y2);
        b1 = -resultat[0] * (x2 - x1) + (y2 - y1);
        b2 = -resultat[1] * (x3 - x2) + (y3 - y2);

        Tracer(start);
    }

    public float Equation1(float x)
    {
        t = (x - x1) / (x2 - x1);
        q1 = (1 - t) * y1 + t * y2 + t * (1 - t) * ((1 - t) * a1 + t * b1);
        return q1;
    }

    public float Equation2(float x)
    {
        t = (x - x2) / (x3 - x2);
        q2 = (1 - t) * y2 + t * y3 + t * (1 - t) * ((1 - t) * a2 + t * b2);
        return q2;
    }

    public void Tracer(bool start)
    {
        if (start == false)
        {
            GameObject line = (new GameObject("line"));
            line.gameObject.AddComponent<LineRenderer>();
            line.gameObject.GetComponent<LineRenderer>().startWidth = 0.2f;
            line.GetComponent<LineRenderer>().endWidth = 0.2f;
            line.gameObject.GetComponent<LineRenderer>().positionCount = points;
            line.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            line.GetComponent<LineRenderer>().startColor = Color.white;
            line.GetComponent<LineRenderer>().endColor = Color.white;
            float xdebut = x2;
            float xfin = x3;

            for (int position = 0; position < points; position++)
            {
                float progress = (float)position / (points - 1);
                float x = xdebut + (xfin - xdebut) * progress;
                float y = Equation2(x);
                line.gameObject.GetComponent<LineRenderer>().SetPosition(position, new Vector3(x, y, 0));
            }

            line.gameObject.AddComponent<EdgeCollider2D>();
            edgeCollider = line.GetComponent<EdgeCollider2D>();
            SetCollider(line);
            list_of_object.Add(new KeyValuePair<GameObject, float>(line.gameObject, xdebut));
        }
        else
        {
            GameObject line = (new GameObject("line"));
            line.gameObject.AddComponent<LineRenderer>();
            line.GetComponent<LineRenderer>().startWidth = 0.2f;
            line.GetComponent<LineRenderer>().endWidth = 0.2f;
            line.gameObject.GetComponent<LineRenderer>().positionCount = points;
            line.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            line.GetComponent<LineRenderer>().startColor = Color.white;
            line.GetComponent<LineRenderer>().endColor = Color.white;
            float xdebut = x1;
            float xfin = x2;

            for (int position = 0; position < points; position++)
            {
                float progress = (float)position / (points - 1);
                float x = xdebut + (xfin - xdebut) * progress;
                float y = Equation1(x);
                line.gameObject.GetComponent<LineRenderer>().SetPosition(position, new Vector3(x, y, 0));
            }

            line.gameObject.AddComponent<EdgeCollider2D>();
            edgeCollider = line.GetComponent<EdgeCollider2D>();
            SetCollider(line);
            list_of_object.Add(new KeyValuePair<GameObject, float>(line, xdebut));

            GameObject line2 = (new GameObject("line"));
            line2.AddComponent<LineRenderer>();
            line2.GetComponent<LineRenderer>().startWidth = 0.2f;
            line2.GetComponent<LineRenderer>().endWidth = 0.2f;
            line2.GetComponent<LineRenderer>().positionCount = points;
            line2.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            line2.GetComponent<LineRenderer>().startColor = Color.white;
            line2.GetComponent<LineRenderer>().endColor = Color.white;
            xdebut = x2;
            xfin = x3;

            for (int position = 0; position < points; position++)
            {
                float progress = (float)position / (points - 1);
                float x = xdebut + (xfin - xdebut) * progress;
                float y = Equation2(x);
                line2.GetComponent<LineRenderer>().SetPosition(position, new Vector3(x, y, 0));
            }

            line2.AddComponent<EdgeCollider2D>();
            edgeCollider = line2.GetComponent<EdgeCollider2D>();
            SetCollider(line2);
            list_of_object.Add(new KeyValuePair<GameObject, float>(line2, xdebut));
        }
    }

    public void SetCollider(GameObject ligne)
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < ligne.gameObject.GetComponent<LineRenderer>().positionCount; point++)
        {
            Vector3 lineRendererPoint = ligne.gameObject.GetComponent<LineRenderer>().GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        edgeCollider.SetPoints(edges);
    }

    public void ObstacleGeneration(int nbObstacles)
    {
        while (nbObstacles > 0)
        {
            x1 = x2;
            x2 = x3;
            x3 += Random.Range(20.0f, 30.0f);

            y1 = y3;
            y2 = y3;
            y3 -= Random.Range(1.0f, 3.0f);

            Interpolation(x1, x2, x3, y1, y2, y3, start);

            if (Random.Range(0, 2) == 0)
            {
                Instantiate(tree, new Vector3(x3, y3 + 2.0f, 0), transform.rotation);
                //list_of_object.Add(new KeyValuePair<GameObject, float>(arbre, x2));
            }
            else
            {
                Instantiate(rock, new Vector3(x3, y3 + 0.5f, 0), transform.rotation);
                //Debug.Log("Roche");
            }
            nbObstacles--;
        }
    }
}