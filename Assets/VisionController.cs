using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Edge = ValueHolder.Edge;


public class VisionController : MonoBehaviour {
    public float fov;//fov is half the total fov
    public float lookDis;
    public int amountOfPoints = 10;
    public Sprite fovSprite;
    public Vector2 originOffset;
    List<Edge> gizmoPoints = new List<Edge>();
    List<Vector2> gizmoLinesx = new List<Vector2>();
    List<Vector2> gizmoLinesy = new List<Vector2>();
    public bool wrapAround;

    

    private Transform origin;
    public void LateUpdate() {
        origin = GameObject.FindGameObjectWithTag("Player").transform;
        gizmoLinesx.Clear();
        gizmoLinesy.Clear();
        Mesh mesh = new Mesh();
        
        //Collects every edge within the look distance
        List<Edge> allVerts = AllInRadius(lookDis);
        
        
        //Adds the points at the far end of the cone
        Vector2[] outsidepoints = new Vector2[amountOfPoints];
        for (int i = 0; i < amountOfPoints; i++) {
            outsidepoints[i] = new Vector2(
                Mathf.Cos((origin.rotation.eulerAngles.z - (2 * fov * i / (amountOfPoints-1) - fov)) * Mathf.Deg2Rad),
                Mathf.Sin((origin.rotation.eulerAngles.z - (2 * fov * i / (amountOfPoints-1) - fov)) *
                          Mathf.Deg2Rad)) * lookDis + (Vector2)origin.position;
        }

        
        
        //Adds points in between the vertices in the far end of the cone to go along walls
        RaycastHit2D outsideray;
        for (int i = 0; i < amountOfPoints-1; i++) {//sets points forward
            outsideray = Physics2D.Raycast(outsidepoints[i], outsidepoints[i+1]-outsidepoints[i],(outsidepoints[i]- outsidepoints[i+1]).magnitude);
            if (outsideray) {
                allVerts.Add(new Edge(outsideray.point,(origin.rotation.eulerAngles.z - (2 * fov * i / (amountOfPoints-1) - fov)),gameObject));
            }
        }
        for (int i = 1; i < amountOfPoints; i++) {//sets points backward
            outsideray = Physics2D.Raycast(outsidepoints[i], outsidepoints[i-1]-outsidepoints[i],(outsidepoints[i]- outsidepoints[i-1]).magnitude);
            if (outsideray) {
                allVerts.Add(new Edge(outsideray.point,(origin.rotation.eulerAngles.z - (2 * fov * i / (amountOfPoints-1) - fov)),gameObject));
            }
        }
        
        //Actually adds the points on the far end of the cone(excludes the first and last vertices as it is added at another time)
        for (int i = 1; i < amountOfPoints-1; i++) {
            allVerts.Add(new Edge(outsidepoints[i],(origin.rotation.eulerAngles.z  - (2 * fov * i / (amountOfPoints-1) - fov)),gameObject));
        }
        
        
        //Remove all points outside FOV
        Edge[] allVertsInFov = AllInAngle(allVerts);


        
        //Sorts points from left to right(bubble sort)
        Edge temp;
        for (int j = 0; j <= allVertsInFov.Length - 2; j++)
        {
            for (int i = 0; i <= allVertsInFov.Length - 2; i++)
            {
                if (Vector2.SignedAngle(origin.up,allVertsInFov[i].pos - (Vector2)origin.position + originOffset)-origin.rotation.eulerAngles.z >
                    Vector2.SignedAngle(origin.up,allVertsInFov[i+1].pos - (Vector2)origin.position + originOffset)-origin.rotation.eulerAngles.z)
                {
                    temp = allVertsInFov[i + 1];
                    allVertsInFov[i + 1] = allVertsInFov[i];
                    allVertsInFov[i] = temp;
                }
            }
        }
        
        gizmoPoints = allVertsInFov.ToList();
        
        List<Vector3> fovVerts = new List<Vector3>();
        fovVerts.Add(origin.transform.position);
        
        //Adds leftmost vertex
        RaycastHit2D ray = Raycast(-fov+ origin.rotation.eulerAngles.z);
        fovVerts.Add(ray ? ray.point : AngleToVector2(-fov + origin.rotation.eulerAngles.z) * lookDis +(Vector2)origin.position + originOffset );
        
        //Adds a vertex for each point as long as there is a direct line of sight
        foreach (Edge v in allVertsInFov) {
            
            Vector2 offset = v.pos - (Vector2)origin.position + originOffset; 
            float ang = (Vector2.SignedAngle(Vector2.right, offset) - v.ang + 720) % 360;
            
            ray = RaycastTo(v.pos);
            if (!ray) {//Ignores edge if there is a collider in between the edge and the origin
                
                float modifidelookdis = lookDis - Vector2.Distance(origin.position, v.pos);
                float angv2 = Vector2.SignedAngle(Vector2.right, offset);
                if (ang >= v.angleOfAttack && ang <=180 - v.angleOfAttack) {//Places a vertex on the edge and then adds a vertex past the edge
                    fovVerts.Add(v.pos);
                    ray = Raycastfrom(v.pos,angv2,modifidelookdis); 
                    fovVerts.Add(ray ? ray.point : v.pos + new Vector2(Mathf.Cos(angv2*Mathf.Deg2Rad), Mathf.Sin(angv2*Mathf.Deg2Rad))*modifidelookdis);
                } else if (ang >= 180 + v.angleOfAttack && ang <= 360-v.angleOfAttack) { //Places a vertex past the edge and then adds a vertex on the edge
                    ray = Raycastfrom(v.pos,angv2,modifidelookdis); 
                    fovVerts.Add(ray ? ray.point : v.pos + new Vector2(Mathf.Cos(angv2*Mathf.Deg2Rad), Mathf.Sin(angv2*Mathf.Deg2Rad))*modifidelookdis); 
                    fovVerts.Add(v.pos);
                } else {//Places a vertex on the edge but not past it since the edge is being looked at head on
                    fovVerts.Add(v.pos);
                }
            }

        }
        
        //Adds rightmost vertex
        ray = Raycast(fov+ origin.rotation.eulerAngles.z);
        fovVerts.Add(ray? ray.point : AngleToVector2(fov+origin.rotation.eulerAngles.z) * lookDis +(Vector2)origin.position + originOffset);
        
        
        
        
        
        List<int> triangles = new List<int>();
        for (int i = 1; i < fovVerts.Count-1; i++) {
            triangles.Add(i+1);
            triangles.Add(i);
            triangles.Add(0);
        }

        if (wrapAround) {
            
            triangles.Add(fovVerts.Count-1);
            triangles.Add(1);
            triangles.Add(0);
            
        }

        
        
        
        
        Vector2[] verts2d = new Vector2[fovVerts.Count];
        for (int i = 0; i<fovVerts.Count;i++) {
            verts2d[i] = new Vector2(fovVerts[i].x + 128, fovVerts[i].y + 128);
        }
        ushort[] vertCount = new ushort[triangles.Count];
        for (int i = 0; i < triangles.Count; i++) {
            vertCount[i] = (ushort)triangles[i];
        }


        fovSprite.OverrideGeometry(verts2d,vertCount);
    }

    //Collects every edge within the look distance
    public List<Edge> AllInRadius(float radius) {
        List<Edge> allVerts = new List<Edge>();
        foreach (Edge v in GameObject.FindGameObjectWithTag("ValueHolder").GetComponent<ValueHolder>().edges) {
            if (v!=null && v.edgeObject != null && (v.pos - (Vector2)origin.position + originOffset).magnitude < radius) {
                allVerts.Add(v);
            }
        }
        return allVerts;
    }
    
    //draws gizmo(for debugging and visualization)
    public void OnDrawGizmos() {
        foreach (Edge v in gizmoPoints) {
            Gizmos.DrawSphere(v.pos, 0.05f);
        }
        
        Gizmos.color = Color.red;
        for (int i = 0; i < gizmoLinesx.Count; i++) {
            Gizmos.DrawLine(gizmoLinesx[i],gizmoLinesy[i]);
        }
    }
    
    //returns an array of each point that is within the fov
    Edge[] AllInAngle(List<Edge> allVerts) {
        List<Edge> newVerts = new List<Edge>();
        foreach (Edge v in allVerts) {
            if (Vector2.Angle(AngleToVector2(origin.rotation.eulerAngles.z), v.pos - (Vector2)origin.position + originOffset) < fov) {
                newVerts.Add(v);
            }
        }
        return newVerts.ToArray();
    }
    
    //Sends a ray in a direction from the origin
    RaycastHit2D Raycast(float angle) {
        return Physics2D.Raycast(origin.position + (Vector3)originOffset, AngleToVector2(angle ) ,lookDis,LayerMask.GetMask("Walls"));
    }
    
    //Sends ray to point from origin(stops right before the point so the point can rest on the edge of a collider without the ray colliding with it)
    RaycastHit2D RaycastTo(Vector2 pos) {
        return Physics2D.Raycast(origin.position + (Vector3)originOffset, pos-(Vector2)origin.position + originOffset,(pos-(Vector2)origin.position + originOffset).magnitude-0.02f,LayerMask.GetMask("Walls"));
    }
    
    //Sends a ray form a point in a direction (the offset is there so that the point can rest on the edge of a collider without the ray colliding with it)
    RaycastHit2D Raycastfrom(Vector2 start,float angle,float dis) {
        Vector2 a = AngleToVector2(angle)*0.2f;//small offset at start
        gizmoLinesx.Add(start);
        RaycastHit2D ray = Physics2D.Raycast(start + a, AngleToVector2(angle),dis - 0.2f,LayerMask.GetMask("Walls"));
        gizmoLinesy.Add(ray ? ray.point : start+ new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad))*dis);
        return ray;
    }
    
    //Converts angle in degrees to a vector direction
    Vector2 AngleToVector2(float angle) {
        return new Vector2(Mathf.Cos((angle) * Mathf.Deg2Rad),Mathf.Sin((angle)* Mathf.Deg2Rad));
    }
    
}