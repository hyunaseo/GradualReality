using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoundaryBox : MonoBehaviour
{
    private LineRenderer lineRenderer;
    Vector3[] vertices;

    Color lineColor;
    GradualRealityManager GRManager;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        vertices = GetComponent<MeshFilter>().mesh.vertices;

        GRManager = GameObject.FindObjectOfType<GradualRealityManager>();
        lineColor = GRManager.BoundaryBoxLineColor;
    }

    void Update()
    {
        // original
        lineRenderer.startWidth = 0.0022f;

        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.positionCount = 16;

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

        //Upper Side
        lineRenderer.SetPosition(0, boundPoint0);
        lineRenderer.SetPosition(1, boundPoint1);
        lineRenderer.SetPosition(2, boundPoint2);
        lineRenderer.SetPosition(3, boundPoint3);
        lineRenderer.SetPosition(4, boundPoint0);

        //Lower Side
        lineRenderer.SetPosition(5, boundPoint4);

        lineRenderer.SetPosition(6, boundPoint5);
        lineRenderer.SetPosition(7, boundPoint1);
        lineRenderer.SetPosition(8, boundPoint5);  
  
        lineRenderer.SetPosition(9, boundPoint6);
        lineRenderer.SetPosition(10, boundPoint2);
        lineRenderer.SetPosition(11, boundPoint6);

        lineRenderer.SetPosition(12, boundPoint7);
        lineRenderer.SetPosition(13, boundPoint3);
        lineRenderer.SetPosition(14, boundPoint7);

        lineRenderer.SetPosition(15, boundPoint4);
    }
}
