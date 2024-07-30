using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoundingBox : MonoBehaviour
{
    GameObject cube;
    private LineRenderer _lineRenderer;
    Vector3[] vertices;

    Color lineColor;
    InitParams initParams;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        vertices = GetComponent<MeshFilter>().mesh.vertices;

        initParams = GameObject.FindObjectOfType<InitParams>();
        lineColor = initParams.lineColor;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 minPoint = GetComponent<MeshFilter>().mesh.bounds.min;
        Vector3 maxPoint = GetComponent<MeshFilter>().mesh.bounds.max;

        //Upper vertices 
        Vector3 boundPoint0 = maxPoint;
        Vector3 boundPoint1 = new Vector3(minPoint.x, maxPoint.y, maxPoint.z);
        Vector3 boundPoint2 = new Vector3(minPoint.x, maxPoint.y, minPoint.z);
        Vector3 boundPoint3 = new Vector3(maxPoint.x, maxPoint.y, minPoint.z);

        //Lower vertices 
        Vector3 boundPoint4 = new Vector3(maxPoint.x, minPoint.y, maxPoint.z);
        Vector3 boundPoint5 = new Vector3(minPoint.x, minPoint.y, maxPoint.z);
        Vector3 boundPoint6 = minPoint;
        Vector3 boundPoint7 = new Vector3(maxPoint.x, minPoint.y, minPoint.z);

        // original
        _lineRenderer.startWidth = 0.0022f;

        // for figure
        // _lineRenderer.startWidth = 0.005f;
        _lineRenderer.startColor = lineColor;
        _lineRenderer.endColor = lineColor;
        _lineRenderer.positionCount = 16;
        
        //Upper Side
        _lineRenderer.SetPosition(0, boundPoint0);
        _lineRenderer.SetPosition(1, boundPoint1);
        _lineRenderer.SetPosition(2, boundPoint2);
        _lineRenderer.SetPosition(3, boundPoint3);
        _lineRenderer.SetPosition(4, boundPoint0);

        //Lower Side
        _lineRenderer.SetPosition(5, boundPoint4);

        _lineRenderer.SetPosition(6, boundPoint5);
        _lineRenderer.SetPosition(7, boundPoint1);
        _lineRenderer.SetPosition(8, boundPoint5);  
  
        _lineRenderer.SetPosition(9, boundPoint6);
        _lineRenderer.SetPosition(10, boundPoint2);
        _lineRenderer.SetPosition(11, boundPoint6);

        _lineRenderer.SetPosition(12, boundPoint7);
        _lineRenderer.SetPosition(13, boundPoint3);
        _lineRenderer.SetPosition(14, boundPoint7);

        _lineRenderer.SetPosition(15, boundPoint4);
    }
}
