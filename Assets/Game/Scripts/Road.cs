using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;
using PathCreation;

public class Road : MonoBehaviour
{
    [SerializeField] private SplineComputer spline;
    [SerializeField] private PathCreator pathCreator;
    private float closestDistance = 0;

    private void Awake()
    {
        Generate();
    }
    private void Generate()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < 44; i++)
        {
            Vector3 point = new Vector3(0, i % 2 == 0 ? -10 : 10, i * 25 - 15);
            points.Add(point);
        }
        pathCreator.bezierPath = new BezierPath(points);
        pathCreator.bezierPath.GlobalNormalsAngle = 90;
        /*SplinePoint[] splinePoints = points.Select(point => new SplinePoint(point)).ToArray();

        spline.SetPoints(splinePoints);
        spline.Rebuild();*/
        
    }
}
