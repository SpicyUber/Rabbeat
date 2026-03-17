using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierRoad : MonoBehaviour
{
    public Vector3 P0, P1, P2;
    public float Width=10;
    public float Height=10;
    public MeshFilter Filter;
    public MeshRenderer Renderer;
    public MeshCollider Collider;
    public int DetailLevel;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    public float lowVal,highVal;
    public GameObject[] Pillars;
    public MeshFilter ShadowFilter;
    public MeshFilter DropshadowFilter;
    private Vector3[] shadowVertices;
    private Vector3[] dropShadowVertices;
    private int[] shadowTriangles;
    private int[] dropshadowTriangles;
    public float PillarOffset;

    //P1 is the curve point
    // Start is called before the first frame update
    void Start()
    {
        UpdateMesh(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateMesh()
    {   if (DetailLevel < 3) DetailLevel = 3;
        vertices = new Vector3[4*DetailLevel];
        shadowVertices  = new Vector3[2 * DetailLevel];
        dropShadowVertices = new Vector3[2 * DetailLevel];
        shadowTriangles = new int[6 * (DetailLevel - 1)];
        dropshadowTriangles = new int[6 * (DetailLevel - 1)];
        uvs   = new Vector2[4 * DetailLevel];
        triangles = new int[(((8*(DetailLevel-1))+4))*3];
        if (Filter.sharedMesh == null) { Filter.sharedMesh = new Mesh(); }
        Filter.sharedMesh.Clear();
      
        
        

        //calculate vertices
        for(int i = 0; i < DetailLevel; i++)
        {
            //bottom left, bottom right, top right, top left
            vertices[4 * i]=new Vector3(-(Width/2), -Height /2,0)+Bezier(i/ (float)(DetailLevel-1));
            vertices[4 * i + 1]= new Vector3(+(Width / 2), -Height / 2,0) + Bezier(i / (float)(DetailLevel - 1));
                vertices[4 * i + 2]= new Vector3((Width / 2), Height / 2, 0) + Bezier(i / (float)(DetailLevel - 1));
            vertices[4 * i + 3]= new Vector3(-(Width / 2), Height / 2, 0) + Bezier(i / (float)(DetailLevel - 1));

            


        }
        //shadow vertices
        for (int i = 0;i < DetailLevel; i++)
        {
            shadowVertices[2 * i] = new Vector3(-(Width / 2), -Height / 2, 0) + Bezier(i / (float)(DetailLevel - 1));
            shadowVertices[2 * i + 1] = new Vector3(+(Width / 2), -Height / 2, 0) + Bezier(i / (float)(DetailLevel - 1));

        }
        //dropshadow vertices
        for (int i = 0; i < DetailLevel; i++)
        {
            Vector3 temp =Bezier(i / (float)(DetailLevel - 1));
            temp.y = 0;
            dropShadowVertices[2 * i] = new Vector3(-(Width / 2), -Height / 2, 0) + temp;
            dropShadowVertices[2 * i + 1] = new Vector3(+(Width / 2), -Height / 2, 0) + temp;
        }
        //shadow triangles
        for (int i = DetailLevel - 2; i >= 0 ; i--)
        {
            shadowTriangles[i * 6] = 0 + i * 2;
            shadowTriangles[i * 6 + 1] = 1 + i * 2;
            shadowTriangles[i * 6 + 2] = 3 + i * 2;
            shadowTriangles[i * 6 + 3] = 0 + i * 2;
            shadowTriangles[i * 6 + 4] = 3 + i * 2;
            shadowTriangles[i * 6 + 5] = 2 + i * 2;
        


        }
        //dropshadow traingles
        for (int i = 0; i < DetailLevel - 1; i++)
        {
            dropshadowTriangles[i * 6] = 0 + i * 2;
            dropshadowTriangles[i * 6 + 1] = 2 + i * 2;
            dropshadowTriangles[i * 6 + 2] = 3 + i * 2;
            dropshadowTriangles[i * 6 + 3] = 3 + i * 2;
            dropshadowTriangles[i * 6 + 4] = 1 + i * 2;
            dropshadowTriangles[i * 6 + 5] = 0 + i * 2;



        }

        //calculate pillar positions
        if (Pillars!=null && Pillars.Length == 4)
        {

            Pillars[0].transform.position=transform.TransformPoint(new Vector3(-(Width / 2)+3, -Height / 2, 0 + 3) + Bezier(0) + new Vector3(0,PillarOffset,0));
            Pillars[1].transform.position = transform.TransformPoint(new Vector3(+(Width / 2) - 3, -Height / 2, 0 + 3) + Bezier(0) + new Vector3(0, PillarOffset, 0));
            Pillars[2].transform.position = transform.TransformPoint(new Vector3(-(Width / 2) + 3, -Height / 2, 0-3) + Bezier(1) + new Vector3(0, PillarOffset, 0));
            Pillars[3].transform.position = transform.TransformPoint(new Vector3(+(Width / 2) - 3, -Height / 2, 0-3) + Bezier(1) + new Vector3(0, PillarOffset, 0));
        }
        //calculate triangles
        for(int i = 0;i < DetailLevel - 1; i++)
        {
            triangles[i * 24]=0+i*4;
            triangles[i * 24+1]=1+i*4;
            triangles[i * 24+2]=5 + i * 4;
            triangles[i * 24 + 3]=0 + i * 4;
            triangles[i * 24 + 4]= 5+i * 4;
            triangles[i * 24 + 5] = 4+i * 4;
            triangles[i * 24 + 6] = 2 + i * 4;
            triangles[i * 24 + 7]=3 + i * 4;
            triangles[i * 24 + 8]=7 + i * 4;
            triangles[i * 24 + 9]=2 + i * 4;
            triangles[i * 24 + 10]=7 + i * 4;
            triangles[i * 24 + 11]=6 + i * 4;
              triangles[i * 24 + 12] =3+ i * 4;
             triangles[i * 24 + 13]= 0+ i * 4;
             triangles[i * 24 + 14]= 4+ i * 4;
             triangles[i * 24 + 15]= 7+ i * 4;
             triangles[i * 24 + 16]= 3+ i * 4;
             triangles[i * 24 + 17]= 4+ i * 4;
             triangles[i * 24 + 18]= 1+ i * 4;
             triangles[i * 24 + 19]=2+ i * 4;
             triangles[i * 24 + 20]=6+ i * 4;
             triangles[i * 24 + 21]=1+ i * 4;
             triangles[i * 24 + 22]=6+ i * 4;
             triangles[i * 24 + 23]=5+ i * 4; 


        }
        //calculate start and end triangles
        triangles[triangles.Length-12] = 3;
        triangles[triangles.Length - 11]=2;
        triangles[triangles.Length - 10] = 0;
        triangles[triangles.Length - 9] = 2;
        triangles[triangles.Length - 8] = 1;
        triangles[triangles.Length - 7] = 0;
        triangles[triangles.Length - 6] = vertices.Length - 4;
        triangles[triangles.Length - 5] = vertices.Length - 3;
        triangles[triangles.Length - 4] = vertices.Length-2;
        triangles[triangles.Length - 3] = vertices.Length - 4;
        triangles[triangles.Length - 2] = vertices.Length - 2;
        triangles[triangles.Length - 1] = vertices.Length - 1;
        //set uvs
         
        
        for(int x = 0; x < uvs.Length/4; x++)
        {
            uvs[x * 4 + 0] = new(lowVal, lowVal);
            uvs[x * 4+1] =new(highVal, lowVal);
            
            uvs[x * 4 + 2] = new(highVal, highVal);
            uvs[x * 4 + 3] = new(lowVal, highVal);
         
        }
        //set values
        Filter.sharedMesh.SetVertices(vertices);
        Filter.sharedMesh.SetTriangles(triangles,0);
        Filter.sharedMesh.RecalculateBounds();
       Filter.sharedMesh.RecalculateNormals();
        Filter.sharedMesh.RecalculateTangents();
        Filter.sharedMesh.SetUVs(0,uvs);
       if(ShadowFilter.mesh ==null) { ShadowFilter.mesh = new(); }
        if (DropshadowFilter.mesh == null) { ShadowFilter.mesh = new(); }
        ShadowFilter.mesh.Clear();
                ShadowFilter.mesh.SetVertices(shadowVertices);
        ShadowFilter.mesh.SetTriangles(shadowTriangles,0);
        DropshadowFilter.mesh.Clear();
        DropshadowFilter.mesh.SetVertices(dropShadowVertices);
        DropshadowFilter.mesh.SetTriangles(dropshadowTriangles, 0);
         Collider.sharedMesh = Filter.sharedMesh;
    }

    private Vector3 Bezier(float t)
    {
        Vector3 L1 = P0 * (1 - t) + P1 * t;
        Vector3 L2 = P1 * (1-t) + P2 * t;
        return L1 * (1 - t) + L2 * t;


    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) { 
            UpdateMesh();
            if (vertices != null)
            {
                Gizmos.color = Color.yellow;
                foreach(Vector3 vert in vertices) {
                    Gizmos.DrawCube(transform.TransformPoint(vert), Vector3.one);
                }
            }
                }
    }
}
