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

    public int Width = 16, Height = 16, Depth = 16;


    RaycastHit tmpHitHighliht;
    // Start is called before the first frame update
    void Start()
    {
        box = new Box(Width, Height, Depth);
        Camera.main.transform.position = new Vector3Int(box.Width / 2, box.Height / 2, -20);
        CreateVoxelBox();
        //box.voxels[new Vector3Int(1,0,0)].GetComponent<Renderer>().material = TileExploded;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Input.GetMouseButton(0))
        if (Input.GetMouseButtonUp(0))
            if (Physics.Raycast(ray, out tmpHitHighliht, 100))
            {
                //Renderer renderer = tmpHitHighliht.transform.GetComponent<Renderer>();

                Vector3Int position = Vector3Int.CeilToInt(tmpHitHighliht.transform.position);
                Cell cell = box.voxels[position].GetComponent<Cell>();
                Renderer renderer = box.voxels[position].GetComponent<Renderer>();
                if (cell.type == Cell.Type.Mine)
                {
                    Debug.Log($"We hit a bomb: {tmpHitHighliht.transform.name}");
                    //renderer.material = TileExploded;
                    box.voxels[position].GetComponent<Renderer>().material = TileExploded;
                }
                else
                {
                    Debug.Log($"We didn't hit a bomb: {tmpHitHighliht.transform.name}");
                    //cell.showing = true;
                    // box.voxels[position].GetComponent<Cell>().showing = true;
                    //tmpHitHighliht.transform.GetComponent<Renderer>().material = GetCube(box.voxels[position].GetComponent<Cell>());
                    //renderer.material = GetCube(box.voxels[position].GetComponent<Cell>());
                    Reveal(position);
                }
            }
        if (Input.GetMouseButtonUp(1))
            if (Physics.Raycast(ray, out tmpHitHighliht, 100))
            {
                //Renderer renderer = tmpHitHighliht.transform.GetComponent<Renderer>();
                Vector3Int position = Vector3Int.CeilToInt(tmpHitHighliht.transform.position);
                Renderer renderer = box.voxels[position].GetComponent<Renderer>();

                Debug.Log($"renderer shared material: {renderer.sharedMaterial}");
                Debug.Log($"TileUnkonwn material: {TileUnknown}");
                Debug.Log($"TileFlag material: {TileFlag}");

                if (renderer.sharedMaterial == TileUnknown)
                    renderer.material = TileFlag;

                else if (renderer.sharedMaterial == TileFlag)
                    renderer.material = TileUnknown;
            }
        if (Input.GetKeyUp(KeyCode.W))
        {
            Camera.main.transform.position = new Vector3Int(box.Width / 2, box.Height / 2, 40);
            // Camera.main.transform.Rotate(new Vector3Int(90, 0, 0)); // looking down
            Camera.main.transform.Rotate(new Vector3Int(0, 180, 0)); //
            //Camera.main.transform.position = new Vector3Int(box.Width / 2, box.Height / 2, -20);
            //Camera.main.transform.Translate(Vector3.right);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Camera.main.transform.position = new Vector3Int(box.Width / 2, box.Height / 2, -20);
            Camera.main.transform.Rotate(new Vector3Int(0, 180, 0)); //
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            Camera.main.transform.position = new Vector3Int(-box.Width, box.Height / 2, 7);
            Camera.main.transform.Rotate(new Vector3Int(0, 90, 0)); //
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            Camera.main.transform.position = new Vector3Int(2*box.Width, box.Height / 2, 7);
            Camera.main.transform.Rotate(new Vector3Int(0, -90, 0)); //
        }



    }
    public void Reveal(Vector3Int voxel)
    {
        Renderer renderer = box.voxels[voxel].GetComponent<Renderer>();
        Cell cell = box.voxels[voxel].GetComponent<Cell>();

        if (cell.type == Cell.Type.Empty)
            Flood(voxel);

        cell.showing = true;
        tmpHitHighliht.transform.GetComponent<Renderer>().material = GetCube(cell);
    }
    private void Flood(Vector3Int voxel)
    {
        Cell cell = box.voxels[voxel].GetComponent<Cell>();
        if (cell.showing) return;
        if (cell.type == Cell.Type.Mine) return;
        Renderer renderer = box.voxels[voxel].GetComponent<Renderer>();

        cell.showing = true;
        renderer.material = GetCube(cell);

        if (cell.type == Cell.Type.Empty)
        {
            foreach (var neighbor in box.GetNeighbors(voxel))
                Flood(neighbor);
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
        foreach (KeyValuePair<Vector3Int, GameObject> currentVoxel in box.voxels)
        {
            // var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // //go.transform.GetComponent<MeshFilter>().mesh.SetUVs(new UVModifier());
            // go.AddComponent<UVModifier>();
            // go.transform.position = currentVoxel.Key;
            // go.transform.localScale = new Vector3(1, 1, 1);
            // go.transform.name = $"[{currentVoxel.Key.x},{currentVoxel.Key.y},{currentVoxel.Key.z}]";
            // go.transform.GetComponent<Renderer>().material = GetCube(currentVoxel.Value);


            currentVoxel.Value.transform.GetComponent<Renderer>().material = GetCube(currentVoxel.Value.GetComponent<Cell>());

            //if (currentVoxel.Value.transform.GetComponent<Cell>().type == Cell.Type.Mine)
            //    currentVoxel.Value.transform.GetComponent<Renderer>().material = TileMine;
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
