using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointscript : MonoBehaviour {
        public float size = 1f;
    private List<Transform> waypointstest = new List<Transform>();
       private Transform[] waypoints;
        void OnDrawGizmos()
        {
            waypoints = gameObject.GetComponentsInChildren<Transform>();
            Vector3 last = waypoints[waypoints.Length - 1].position;
        waypointstest.ForEach(thingy => System.Console.WriteLine(thingy));
        for (int i = 1; i < waypoints.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(waypoints[i].position, size);
                Gizmos.DrawLine(last, waypoints[i].position);
                last = waypoints[i].position;
            }
        }
    }
