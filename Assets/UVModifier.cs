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
        var tmp1 = uvs[6];
        var tmp2 = uvs[7];

        uvs[6] = uvs[11];
        uvs[7] = uvs[10];
        uvs[10] = tmp2;
        uvs[11] = tmp1;


        // bottom face
        tmp1 = uvs[15];
        tmp2 = uvs[12];

        uvs[15] = uvs[13];
        uvs[13] = tmp1;
        uvs[12] = uvs[14];
        uvs[14] = tmp2;

        // Apply the modified UVs back to the mesh
        mesh.uv = uvs;
    }
}

