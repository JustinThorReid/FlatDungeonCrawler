using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MeshFilter))]
public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float meshResolution = 1;
    [SerializeField] private int edgeResolveIterations = 6;
    private float maxViewDistance = 100;
    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    private void Start() {
        viewMeshFilter = GetComponent<MeshFilter>();

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView() {
        int stepCount = Mathf.RoundToInt(360f * meshResolution);
        float stepAngleSize = 360f / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldCast = new ViewCastInfo();
        for(int i = 0; i <= stepCount; i++) {
            float angle = transform.eulerAngles.y + stepAngleSize * i;
            ViewCastInfo cast = ViewCast(angle);
            //Debug.DrawLine(transform.position, cast.point);

            if(i > 0) {
                if(oldCast.hit != cast.hit) {
                    EdgeInfo edge = FindEdge(oldCast, cast, false);
                    viewPoints.Add(edge.pointA.point);
                    if(edge.pointB.point != cast.point)
                        viewPoints.Add(edge.pointB.point);
                } else if(oldCast.normal != cast.normal) {
                    EdgeInfo edge = FindEdge(oldCast, cast, true);
                    viewPoints.Add(edge.pointA.point);
                    if(edge.pointB.point != cast.point)
                        viewPoints.Add(edge.pointB.point);
                }
            }

            viewPoints.Add(cast.point);
            oldCast = cast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] verticies = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        verticies[0] = Vector3.zero;
        for(int i = 0; i < vertexCount-1; i++) {
            verticies[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            verticies[i + 1].z = 0;

            if(i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = verticies;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float angle) {
        Vector3 dir = DirFromAngle(angle);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxViewDistance, LayerMask.GetMask("Default"));

        if(hit.collider != null) {
            return new ViewCastInfo(hit.collider, hit.point, hit.normal, hit.distance, angle);
        }
        return new ViewCastInfo(null, transform.position + dir * maxViewDistance, dir, maxViewDistance, angle);
    }

    public struct ViewCastInfo {
        public Collider2D hit;
        public Vector3 point;
        public Vector3 normal;
        public float dst;
        public float angle;

        public ViewCastInfo(Collider2D hit, Vector3 point, Vector3 normal, float dst, float angle) {
            this.hit = hit;
            this.point = point;
            this.dst = dst;
            this.angle = angle;
            this.normal = normal;
        }
    }

    public EdgeInfo FindEdge(ViewCastInfo minCast, ViewCastInfo maxCast, bool useNormals) {
        float midAngle = maxCast.angle;
        for(int i = 0; i < edgeResolveIterations; i++) {
            float testAngle = (minCast.angle + midAngle) / 2;
            ViewCastInfo cast = ViewCast(testAngle);
            
            if(cast.normal == minCast.normal && cast.hit == minCast.hit) {
                minCast = cast;
            } else {
                midAngle = testAngle;
            }
        }

        midAngle = minCast.angle;
        for(int i = 0; i < edgeResolveIterations; i++) {
            float testAngle = (maxCast.angle + midAngle) / 2;
            ViewCastInfo cast = ViewCast(testAngle);

            if(cast.normal == maxCast.normal && cast.hit == maxCast.hit) {
                maxCast = cast;
            } else {
                midAngle = testAngle;
            }
        }

        return new EdgeInfo(minCast, maxCast);
    }

    public struct EdgeInfo {
        public ViewCastInfo pointA;
        public ViewCastInfo pointB;

        public EdgeInfo(ViewCastInfo pointA, ViewCastInfo pointB) {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }

    Vector3 DirFromAngle(float angleInDegrees) {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}
