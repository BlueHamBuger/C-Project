using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackPlane : MonoBehaviour
{
    MeshCollider meshCollider;
    // Start is called before the first frame update
    void Start()
    {
        
        
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        List<int> Indices = new List<int>(meshFilter.mesh.GetIndices(0));
        List<int>  InverseIndices = new List<int>(meshFilter.mesh.GetIndices(0));
        InverseIndices.Reverse();
        Indices.AddRange(InverseIndices);
        // List<Vector3> normals = new List<Vector3>();
        // meshFilter.mesh.GetNormals(normals);
        // List<Vector3> flippednormals = new List<Vector3>();
        // foreach(var normal in normals){
        //     flippednormals.Add(new Vector3(-normal.x,-normal.y,-normal.z));
        // }
        
        //meshFilter.mesh.SetNormals(flippednormals);
        meshFilter.mesh.SetIndices(Indices.ToArray(), MeshTopology.Triangles, 0);   
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        meshCollider.sharedMesh = meshFilter.mesh;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
