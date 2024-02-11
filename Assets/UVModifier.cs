using UnityEngine;

public class UVModifier : MonoBehaviour
{
    void Start()
    {
        // Get the mesh of the cube primitive
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // Retrieve the current UVs
        Vector2[] uvs = new Vector2[mesh.uv.Length];
        uvs = mesh.uv;


        // back face
        (uvs[6], uvs[11]) = (uvs[11], uvs[6]);
        (uvs[7], uvs[10]) = (uvs[10], uvs[7]);

        // bottom face
        (uvs[15], uvs[13]) = (uvs[13], uvs[15]);
        (uvs[12], uvs[14]) = (uvs[14], uvs[12]);

        // Apply the modified UVs back to the mesh
        mesh.uv = uvs;
    }
}

