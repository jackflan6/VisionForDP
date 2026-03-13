using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueHolder : MonoBehaviour
{
    public List<Edge> edges = new List<Edge>();
    public bool gizmo;
    
    public void OnDrawGizmos() {
        if (gizmo) {
            foreach (Edge v in edges) {
                if (v != null) {
                    Gizmos.DrawSphere(v.pos, 0.05f);
                    Gizmos.DrawLine(v.pos, v.pos + new Vector2(Mathf.Cos((v.ang+v.angleOfAttack)*Mathf.Deg2Rad),Mathf.Sin((v.ang+v.angleOfAttack)*Mathf.Deg2Rad))*0.2f);
                    Gizmos.DrawLine(v.pos, v.pos + new Vector2( Mathf.Cos((v.ang-v.angleOfAttack)*Mathf.Deg2Rad),Mathf.Sin((v.ang-v.angleOfAttack)*Mathf.Deg2Rad))*0.2f); }
                
            }
        }
    }
    [Serializable]
    public class Edge {
        public Vector2 pos;
        public GameObject edgeObject;//the Object ths edge is attached to
        public float ang;//direction of the angle
        public float angleOfAttack ;//the angle of the corner
        
        public Edge(Vector2 position,Edge edge) {
            pos = position;
            edgeObject = edge.edgeObject;
            ang = edge.ang;
            angleOfAttack = edge.angleOfAttack;
        }
        
        public Edge(Vector2 position, float innerAngle, GameObject edgeObject) {
            pos = position;
            this.edgeObject = edgeObject;
            ang = innerAngle;
            angleOfAttack = 45;
        }
        
        public Edge(Vector2 position, float innerAngle, GameObject edgeObject, float angleOfAttack) {
            pos = position;
            this.edgeObject = edgeObject;
            ang = innerAngle;
            this.angleOfAttack = angleOfAttack;
        }

    }
}
