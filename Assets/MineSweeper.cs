using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Minesweeper : MonoBehaviour
{
    #region MaterialTypes
    public Material TileUnknown;
    public Material TileFlag;
    public Material TileExploded;
    public Material TileEmpty;
    public Material TileMine;
    public Material Tile1;
    public Material Tile2;
    public Material Tile3;
    public Material Tile4;
    public Material Tile5;
    public Material Tile6;
    public Material Tile7;
    public Material Tile8;
    #endregion

    private Box box;


    RaycastHit tmpHitHighliht;
    // Start is called before the first frame update
    void Start()
    {
        box = new Box(9, 9, 9);
        CreateVoxelBox();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Input.GetMouseButton(0))
        if (Input.GetMouseButtonUp(0))
            if (Physics.Raycast(ray, out tmpHitHighliht, 100))
            {
                Renderer renderer = tmpHitHighliht.transform.GetComponent<Renderer>();

                Vector3Int tmp = Vector3Int.CeilToInt(tmpHitHighliht.transform.position);
                Debug.Log($"POSITION: {Vector3Int.CeilToInt(tmpHitHighliht.transform.position)}");
                foreach (Vector3Int b in box.GetNeighbors(tmp))
                {
                    Debug.Log($"Position {b}");
                }
                if (box.voxels[tmp].type == Cell.Type.Mine)
                {
                    Debug.Log($"We hit a bomb: {tmpHitHighliht.transform.name}");
                    renderer.material = TileExploded;
                }
                else
                {
                    Debug.Log($"We didn't hit a bomb: {tmpHitHighliht.transform.name}");
                    box.voxels[tmp].showing = true;
                    tmpHitHighliht.transform.GetComponent<Renderer>().material = GetCube(box.voxels[tmp]);

                }
            }
        if (Input.GetMouseButtonUp(1))
            if (Physics.Raycast(ray, out tmpHitHighliht, 100))
            {
                Renderer renderer = tmpHitHighliht.transform.GetComponent<Renderer>();

                Debug.Log($"renderer shared material: {renderer.sharedMaterial}");
                Debug.Log($"TileUnkonwn material: {TileUnknown}");
                Debug.Log($"TileFlag material: {TileFlag}");

                if (renderer.sharedMaterial == TileUnknown)
                    renderer.material = TileFlag;

                else if (renderer.sharedMaterial == TileFlag)
                    renderer.material = TileUnknown;
            }



    }
    // void CreateVoxelBox()
    // {
    //     foreach (KeyValuePair<Vector3Int, Cell> currentVoxel in box.voxels)
    //     {
    //         // Instantiate a new custom cube mesh
    //         CustomCubeMesh customCubeMeshInstance = Instantiate(customCubeMeshPrefab, currentVoxel.Key, Quaternion.identity);

    //         // Set the name of the GameObject
    //         customCubeMeshInstance.gameObject.name = $"[{currentVoxel.Key.x},{currentVoxel.Key.y},{currentVoxel.Key.z}]";

    //         // Set the material of the cube mesh based on the cell type
    //         Material material = GetCube(currentVoxel.Value);
    //         customCubeMeshInstance.GetComponent<Renderer>().material = material;
    //     }
    // }
    void CreateVoxelBox()
    {
        foreach (KeyValuePair<Vector3Int, Cell> currentVoxel in box.voxels)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //go.transform.GetComponent<MeshFilter>().mesh.SetUVs(new UVModifier());
            go.AddComponent<UVModifier>();
            go.transform.position = currentVoxel.Key;
            go.transform.localScale = new Vector3(1,1,1);
            go.transform.name = $"[{currentVoxel.Key.x},{currentVoxel.Key.y},{currentVoxel.Key.z}]";
            go.transform.GetComponent<Renderer>().material = GetCube(currentVoxel.Value);
            if (currentVoxel.Value.type == Cell.Type.Mine)
                go.transform.GetComponent<Renderer>().material = TileMine;
            var cg = go.AddComponent<Cell>();
            cg = currentVoxel.Value;
        }
    }


    private Material GetCube(Cell cell)
    {
        if (cell.showing) return GetTileType(cell);
        if (cell.flagged) return TileFlag;

        return TileUnknown;

    }
    private Material GetTileType(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Mine: return TileMine;
            case Cell.Type.Empty: return TileEmpty;
            case Cell.Type.Number: return GetTileNumber(cell);

            default: return null;
        }
    }
    private Material GetTileNumber(Cell cell)
    {
        switch (cell.num)
        {
            case 1: return Tile1;
            case 2: return Tile2;
            case 3: return Tile3;
            case 4: return Tile4;
            case 5: return Tile5;
            case 6: return Tile6;
            case 7: return Tile7;
            case 8: return Tile8;

            default: return null;
        }
    }
}
