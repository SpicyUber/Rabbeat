using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SquareMountain : MonoBehaviour
{
    public Vector3 Normal;
    public MeshFilter Filter;
    public MeshRenderer Renderer;
    public MeshCollider Collider;
    private Vector3[] _vertices;
    private Vector2[] _uvs;
    private int[] _triangles;
    public bool KeepMountainParallelepiped=false;
    private Vector3[] _verticesTop;
    private Vector2[] _uvsTop;
    private int[] _trianglesTop;
    public float TopOffset;
    public int X, Z,Rotation;
    [SerializeField]
    public int offset00,offset01,offset11,offset10;
    

    // Start is called before the first frame update
    void Start()
    {   
      Normal.Normalize();
       
        
      
          
        UpdateMesh();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {if(!Application.isPlaying)
        UpdateMesh();
    Gizmos.color = Color.red;
        Normal.Normalize();
        Gizmos.DrawLine(transform.position,transform.position+Normal*200);

    }

    public void UpdateMesh()
    {
        
        if (X < 1) { X = 1; }
        if (Z < 1) { Z = 1; }
        if (Filter.sharedMesh == null) { Filter.sharedMesh = new Mesh(); }
        _uvs = new Vector2[]
    {new(0,0),new(0,0.5f),new(0.5f,0.5f),new(0.5f,0),new(0.5f,0.5f),new(0.5f,1),new(1,1),new(1,0.5f)



    };
        transform.rotation = Quaternion.Euler(0, Rotation, 0);
        Filter.sharedMesh.Clear();
        float[] y = new float[4];
        Vector3[] tempVertices = new Vector3[] { new(0, 0, 0), new(0, 0, Z), new(X, 0, Z), new(X, 0, 0) };
        float minY = 1;
        for (int i = 0; i < 4; i++) {

            RaycastHit[] info= Physics.RaycastAll(this.transform.TransformPoint(tempVertices[i]), Vector3.down, 300);
         //   Gizmos.DrawLine(this.transform.TransformPoint(tempVertices[i]), this.transform.TransformPoint(tempVertices[i])+300* Vector3.down);
            if (info.Length == 0) y[i] = -300;
            else
            {
                float tempY = -300;
                foreach (RaycastHit hit in info)
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        tempY = -(this.transform.TransformPoint(tempVertices[i]) - hit.point).y;
                    }

                }

                y[i] = tempY;
            }
            
            if (y[i] < minY) { minY = y[i]; }
           
        }
        Quaternion inv = Quaternion.identity;
        if(!KeepMountainParallelepiped ) { 
        Vector3 v1 = new(0, y[0], 0);
        v1 = inv * v1;
        Vector3 v2 = new(0, y[1], Z);
        v2 = inv * v2;
        Vector3 v3= new(X, y[2], Z);
        v3 = inv * v3;
        Vector4 v4= new(X, y[3], 0);
        v4 = inv * v4;
            _vertices = new Vector3[] { new(0, offset00, 0), new(0, offset01, Z), new(X, offset11, Z), new(X, offset10, 0), v1, v2, v3, v4 };
            //_triangles = new int[] { 0, 1, 3, 3, 1, 2, 4, 0, 7, 7, 0, 3, 7, 3, 6, 6, 3, 2, 6, 2, 5, 5, 2, 1, 5, 1, 4, 4, 1, 0 };
            _triangles = new int[]
    {
    0, 1, 3, 3, 1, 2,
    4, 0, 7, 7, 0, 3,
    7, 3, 6, 6, 3, 2,
    6, 2, 5, 5, 2, 1,
    5, 1, 4, 4, 1, 0,
    0, 3, 1, 1, 3, 2
    };
        }
        else
        {
            minY = minY - Mathf.Max(new float[]{offset00,offset01,offset10,offset11 });
            Vector3 v1 = new(0, minY + offset00, 0);
            v1 = inv * v1;
            Vector3 v2 = new(0, minY + offset01, Z);
            v2 = inv * v2;
            Vector3 v3 = new(X, minY + offset11, Z);
            v3 = inv * v3;
            Vector4 v4 = new(X, minY + offset10, 0);
            v4 = inv * v4;
            _vertices = new Vector3[] { new(0, offset00, 0), new(0, offset01, Z), new(X, offset11, Z), new(X, offset10, 0), v1, v2, v3, v4 };
            //_triangles = new int[] { 0, 1, 3, 3, 1, 2, 4, 0, 7, 7, 0, 3, 7, 3, 6, 6, 3, 2, 6, 2, 5, 5, 2, 1, 5, 1, 4, 4, 1, 0 };
            _triangles = new int[]
    {
    0, 1, 3, 3, 1, 2,
    4, 0, 7, 7, 0, 3,
    7, 3, 6, 6, 3, 2,
    6, 2, 5, 5, 2, 1,
    5, 1, 4, 4, 1, 0,
    0, 3, 1, 1, 3, 2
    };
        }
    

        Filter.sharedMesh.SetVertices(_vertices);
        Filter.sharedMesh.SetTriangles(_triangles,0);
       Filter.sharedMesh.SetUVs(0,_uvs);
        Filter.sharedMesh.RecalculateNormals();
        Collider.sharedMesh = Filter.sharedMesh;
        string str = "";
        foreach(Vector3 vec in Filter.sharedMesh.normals) { str += " " + vec; /*Gizmos.DrawLine(transform.position, transform.position + vec * 10); */}
       // Debug.Log("NORMALZ" + str);

        SetTop();
    }

    public void SetTop()
    {
        if (transform.childCount== 0 || transform.GetChild(0).GetComponent<MeshFilter>()==null) return;
        MeshFilter top = transform.GetChild(0).GetComponent<MeshFilter>();
        if (top.sharedMesh == null) 
        top.sharedMesh = new();
        _verticesTop = new Vector3[] { new(0, offset00 + TopOffset, 0), new(0, offset01 + TopOffset, Z), new(X, offset11 + TopOffset, Z), new(X, offset10 + TopOffset, 0) };
        _trianglesTop = new int[] { 0, 1, 3, 3, 1, 2 };
        _uvsTop = new Vector2[] { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };
        top.sharedMesh.SetVertices(_verticesTop);
        top.sharedMesh.SetTriangles(_trianglesTop,0);
        top.sharedMesh.SetUVs(0,_uvsTop);
        top.sharedMesh.RecalculateNormals();
        top.sharedMesh.RecalculateBounds();
        top.sharedMesh.RecalculateTangents();
    }
}
