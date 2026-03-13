using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edge = ValueHolder.Edge;

public class EdgeFinder : MonoBehaviour
{
    void Start()
    {
        List<Edge> edges = new List<Edge>();
        foreach (Vector2 point in GetComponent<PolygonCollider2D>().points) {
            edges.Add(new Edge(point * transform.localScale + (Vector2)transform.position, 0, gameObject));
        }

        //edges[0].angleOfAttack = Vector2.Angle(edges[1].pos - edges[0].pos,edges[edges.Count-1].pos-edges[0].pos)/2;
        //edges[0].ang = Vector2.SignedAngle(edges[1].pos - edges[0].pos, edges[edges.Count - 1].pos - edges[0].pos) / 2 + Vector2.SignedAngle(Vector2.right,edges[1].pos - edges[0].pos);
        //GameObject.FindGameObjectWithTag("ValueHolder").GetComponent<ValueHolder>().edges.Add(edges[0]);
        //edges[edges.Count-1].angleOfAttack = Vector2.Angle(edges[0].pos - edges[edges.Count-1].pos,edges[edges.Count-2].pos-edges[edges.Count-1].pos)/2;
        //edges[edges.Count-1].ang = Vector2.SignedAngle(edges[0].pos - edges[edges.Count-1].pos, edges[edges.Count - 2].pos - edges[edges.Count-1].pos) / 2 + Vector2.SignedAngle(Vector2.right,edges[0].pos - edges[edges.Count-1].pos);
        //GameObject.FindGameObjectWithTag("ValueHolder").GetComponent<ValueHolder>().edges.Add(edges[edges.Count-1]);

        for (int i = 0; i < edges.Count; i++) {
            edges[i].angleOfAttack = Vector2.Angle(edges[(i+1+edges.Count)%edges.Count].pos - edges[i].pos,edges[(i-1+edges.Count)%edges.Count].pos-edges[i].pos)/2;
            edges[i].ang = Vector2.SignedAngle(edges[(i + 1+edges.Count)%edges.Count].pos - edges[i].pos, edges[(i - 1+edges.Count)%edges.Count].pos - edges[i].pos) / 2 + Vector2.SignedAngle(Vector2.right,edges[(i + 1+edges.Count)%edges.Count].pos - edges[i].pos);
            GameObject.FindGameObjectWithTag("ValueHolder").GetComponent<ValueHolder>().edges.Add(edges[i]);
        }
        
    }

}
