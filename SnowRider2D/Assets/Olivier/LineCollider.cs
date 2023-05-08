using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class LineCollider : MonoBehaviour
{
    EdgeCollider2D edgeCollider;

    LineRenderer ligne;

    // Start is called before the first frame update
    void Start()
    {
        edgeCollider = this.GetComponent<EdgeCollider2D>();
        ligne = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetEdgeCollider(ligne);
    }

    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        edgeCollider.SetPoints(edges);
    }
}
