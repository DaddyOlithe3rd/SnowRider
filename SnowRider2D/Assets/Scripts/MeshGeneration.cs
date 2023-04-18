using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MeshGeneration: MonoBehaviour
{
    // the length of segment (world space)
    public float SegmentLength = 32;

    // the segment resolution (number of horizontal points)
    public int SegmentResolution = 32;

    // the size of meshes in the pool
    public int MeshCount = 4;

    // the maximum number of visible meshes. Should be lower or equal than MeshCount
    public int VisibleMeshes = 4;

    // the prefab including MeshFilter and MeshRenderer
    public MeshFilter SegmentPrefab;
    public SpriteRenderer RockPrefab;
    public SpriteRenderer TreePrefab;

    public Vector3 startPoint;

    public int NextSegment = 1;

    // helper array to generate new segment without further allocations
    private Vector3[] _vertexArray;

    // the pool of free mesh filters
    private List<MeshFilter> _freeMeshFilters = new List<MeshFilter>();

    Vector2[] points = new Vector2[32];
   
    void Start()
    {
        // Create vertex array helper
        _vertexArray = new Vector3[SegmentResolution * 2];

        // Build triangles array. For all meshes this array always will
        // look the same, so I am generating it once 
        int iterations = _vertexArray.Length / 2 - 1;
        var triangles = new int[(_vertexArray.Length - 2) * 3];

        for (int i = 0; i < iterations; ++i)
        {
            int i2 = i * 6;
            int i3 = i * 2;

            triangles[i2] = i3 + 2;
            triangles[i2 + 1] = i3 + 1;
            triangles[i2 + 2] = i3 + 0;

            triangles[i2 + 3] = i3 + 2;
            triangles[i2 + 4] = i3 + 3;
            triangles[i2 + 5] = i3 + 1;
        }

        // Create colors array. For now make it all white.
        var colors = new Color32[_vertexArray.Length];
        for (int i = 0; i < colors.Length; ++i)
        {
            colors[i] = new Color32(255, 255, 255, 255);
        }

        // Create game objects (with MeshFilter) instances.
        // Assign vertices, triangles, deactivate and add to the pool.
        for (int i = 0; i < MeshCount; ++i)
        {
            MeshFilter filter = Instantiate(SegmentPrefab);

            Mesh mesh = filter.mesh;
            mesh.Clear();

            mesh.vertices = _vertexArray;
            mesh.triangles = triangles;

            filter.gameObject.SetActive(false);
            _freeMeshFilters.Add(filter);
        }
    }

    Vector3 CalculateQuadraticBezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // B(t) = (1-t)^2*p0 + 2(1 -t)t*p1 + t^2*p2
        //          a             b          c

        float a = (1 - t) * (1 - t);
        float b = 2 * (1 - t) * t;
        float c = t * t;

        Vector3 point = a * p0;
        point += b * p1;
        point += c * p2;
        return point;
    }
    Vector3 CalculateCubicBezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // B(t) = (1-t)^3*p0 + 3(1 - t)^2*t*p1 + 3(1 - t)*t^2*p2 + t^3*p3
        //            a             b                 c           d

        float a = (1 - t) * (1 - t) * (1 - t);
        float b = 3 * ((1 - t) * (1 - t)) * t;
        float c = 3 * (1 - t) * (t * t);
        float d = t * t * t;

        Vector3 point = a * p0;
        point += b * p1;
        point += c * p2;
        point += d * p3;
        return point;
    }
    Vector3 CalculaticOrder5BezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5)
    {
        //B(t) = (1-t)^5*p0 + 5*t*(1-t)^4*p1 + 10*t^2*(1-t)^3*p2 + 10*t^3*(1-t)^2*p3 + 5*t^4*(1-t)*p4 + t^5*p5
        //            a             b                c                   d                   e            f

        float a = (1 - t) * (1 - t) * (1 - t) * (1 - t) * (1 - t);
        float b = 5 * ((1 - t) * (1 - t) * (1 - t) * (1 - t) * t);
        float c = 10 * (t * t * (1 - t) * (1 - t) * (1 - t));
        float d = 10 * (t * t * t) * (1 - t) * (1 - t);
        float e = 5 * (t * t * t * t) * (1 - t);
        float f = t * t * t * t * t;

        Vector3 point = a * p0;
        point += b * p1;
        point += c * p2;            
        point += d * p3;
        point += e * p4;
        point += f * p5;
        return point;
    }
    // This function generates a mesh segment.
    // Index is a segment index (starting with 0).
    // Mesh is a mesh that this segment should be written to.

    public void GenerateSegment(int index, ref Mesh mesh, Vector3 startPos)
    {
        float step = SegmentLength / (SegmentResolution - 1);
        float xPos = 0;
        float yPosTop = 0;
        float t = 0.03125f;
        float time = 0;
        Random rnd = new Random();
        int jumpheight = rnd.Next(0, 8);
        int p1Change = rnd.Next(4, 14);
        int p3Change = rnd.Next(6, 12);

        Vector3 pointBezier;
        Vector3 p0 = new Vector3(0, startPoint.y, 0);
        Vector3 p1 = new Vector3(12, (startPoint.y - p1Change), 0);
        Vector3 p2 = new Vector3(20, startPoint.y + 2, 0);
        Vector3 p3 = new Vector3(32, (startPoint.y - p3Change), 0); ;

        SpriteRenderer Rock = Instantiate(RockPrefab);
        SpriteRenderer Tree = Instantiate(TreePrefab);
        Vector3 scaleChange = new Vector3(8, 16, 0);
        int treeCoord = rnd.Next(0, 31);
        int rockPos = rnd.Next(0, 31);

        for (int i = 0; i < SegmentResolution; ++i)
        {
            startPoint.x += step;
            // get the relative x position
            xPos = step * i;
            time = t * i;
            if (NextSegment % 4 == 0)
            {
                p0 = new Vector3(0, startPoint.y, 0);
                p1 = new Vector3(16, (startPoint.y - 10), 0);
                p2 = new Vector3(32, startPoint.y - jumpheight, 0);
                pointBezier = CalculateQuadraticBezierCurve(time, p0, p1, p2);
            }
            else
            {               
                p0 = new Vector3(0, startPoint.y, 0);
                p1 = new Vector3(12, (startPoint.y - p1Change), 0);
                p2 = new Vector3(20, startPoint.y + 2, 0);
                p3 = new Vector3(32, (startPoint.y - p3Change), 0);
                pointBezier = CalculateCubicBezierCurve(time, p0, p1, p2, p3);
            }
            yPosTop = pointBezier.y;

            if (i == treeCoord)
            {
                Tree.transform.position = new Vector3(xPos + (32 * (NextSegment - 2)), yPosTop + 2, 0);
                Debug.Log(yPosTop);
            }
            // top vertex          
            _vertexArray[i * 2] = new Vector3(xPos, yPosTop, 0);
            // bottom vertex always at y=0
            _vertexArray[i * 2 + 1] = new Vector3(xPos, -10000, 0);

            points[i] = new Vector2(xPos, yPosTop);
        }

        Tree.transform.localScale += scaleChange;
        if (NextSegment % 4 == 0)
        {
            startPoint.y = (yPosTop - 6);
        }
        else
        {
            startPoint.y = yPosTop;
        }
        NextSegment += 1;

        mesh.vertices = _vertexArray;      
        // need to recalculate bounds, because mesh can disappear too early
        mesh.RecalculateBounds();
    }

    private bool IsSegmentInSight(int index)
    {
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 worldRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));

        // check left and right segment side
        float x1 = index * SegmentLength;
        float x2 = x1 + SegmentLength;

        return x1 <= worldRight.x && x2 >= worldLeft.x;
    }

    private struct Segment
    {
        public int Index { get; set; }
        public MeshFilter MeshFilter { get; set; }
    }

    // the list of used segments
    private List<Segment> _usedSegments = new List<Segment>();

    private bool IsSegmentVisible(int index)
    {
        return SegmentCurrentlyVisibleListIndex(index) != -1;
    }

    private int SegmentCurrentlyVisibleListIndex(int index)
    {
        for (int i = 0; i < _usedSegments.Count; ++i)
        {
            if (_usedSegments[i].Index == index)
            {
                return i;
            }
        }

        return -1;
    }

    private void EnsureSegmentVisible(int index)
    {
        if (!IsSegmentVisible(index))
        {
            // get from the pool
            int meshIndex = _freeMeshFilters.Count - 1;
            MeshFilter filter = _freeMeshFilters[meshIndex];
            _freeMeshFilters.RemoveAt(meshIndex);

            // generate
            Mesh mesh = filter.mesh;
            GenerateSegment(index, ref mesh, startPoint);
             
            filter.gameObject.GetComponent<EdgeCollider2D>().points = points;

            //if (filter.gameObject.GetComponent<MeshCollider>() == null)
            //{
            //    filter.gameObject.AddComponent<MeshCollider>();
            //}

            // position
            filter.transform.position = new Vector3(index * SegmentLength, 0, 0);

            // make visible
            filter.gameObject.SetActive(true);

            // register as visible segment
            var segment = new Segment();
            segment.Index = index;
            segment.MeshFilter = filter;

            _usedSegments.Add(segment);
        }
    }

    private void EnsureSegmentNotVisible(int index)
    {
        if (IsSegmentVisible(index))
        {
            int listIndex = SegmentCurrentlyVisibleListIndex(index);
            Segment segment = _usedSegments[listIndex];
            _usedSegments.RemoveAt(listIndex);

            MeshFilter filter = segment.MeshFilter;
            filter.gameObject.SetActive(false);

            _freeMeshFilters.Add(filter);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // get the index of visible segment by finding the center point world position
        Vector3 worldCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        int currentSegment = (int)(worldCenter.x / SegmentLength);

        // Test visible segments for visibility and hide those if not visible.
        for (int i = 0; i < _usedSegments.Count;)
        {
            int segmentIndex = _usedSegments[i].Index;
            if (!IsSegmentInSight(segmentIndex))
            {
                EnsureSegmentNotVisible(segmentIndex);
            }
            else
            {
                // EnsureSegmentNotVisible will remove the segment from the list
                // that's why I increase the counter based on that condition
                ++i;
            }
        }

        // Test neighbor segment indexes for visibility and display those if should be visible.
        for (int i = currentSegment - VisibleMeshes / 2; i < currentSegment + VisibleMeshes / 2; ++i)
        {
            if (IsSegmentInSight(i))
            {
                EnsureSegmentVisible(i);
            }
        }
    }
}

