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


    private static readonly Quaternion LOOKING_UP = Quaternion.LookRotation(new Vector3Int(0, 90, 0));
    private static readonly Quaternion LOOKING_DOWN = Quaternion.LookRotation(new Vector3Int(0, -90, 0));
    private static readonly Quaternion LOOKING_LEFT = Quaternion.LookRotation(new Vector3Int(90, 0, 0));
    private static readonly Quaternion LOOKING_RIGHT = Quaternion.LookRotation(new Vector3Int(-90, 0, 0));
    private static readonly Quaternion LOOKING_FORWARD = Quaternion.LookRotation(new Vector3Int(0, 0, -90));
    private static readonly Quaternion LOOKING_BACKWARD = Quaternion.LookRotation(new Vector3Int(0, 0, 90));

    private static readonly Quaternion[] lookingDirection = { LOOKING_FORWARD, LOOKING_RIGHT, LOOKING_BACKWARD, LOOKING_LEFT };

    private Box box;
    public int Width = 16, Height = 16, Depth = 16;

    private void OnEnable()
    {
        UIModifier.OnChangeSize += UIManager_OnChanngSize;
    }
    private void UIManager_OnChanngSize(int x, int y, int z)
    {
        Debug.Log("AT UI Manager");
        Width = x;
        Height = y;
        Depth = z;
        box.Cleanup();
        newGame();
    }
    private void newGame()
    {
        box = new Box(Width, Height, Depth);
        Camera.main.transform.position = new Vector3Int(box.Width / 2, box.Height / 2, -20 - Depth/2);
        CreateVoxelBox();
        //box.voxels[new Vector3Int(1,0,0)].GetComponent<Renderer>().material = TileExploded;
    }

    RaycastHit tmpHitHighliht;
    // Start is called before the first frame update
    void Start()
    {
        newGame();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var camera = Camera.main.transform;

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
            camera.position = new Vector3(box.Width / 2, box.Height / 2, box.Depth / 2);

            if (camera.rotation == LOOKING_UP)
                camera.rotation = LOOKING_BACKWARD;
            else if (camera.rotation == LOOKING_DOWN)
                camera.rotation = LOOKING_FORWARD;
            else
                camera.rotation = LOOKING_DOWN;

            camera.Translate(new Vector3Int(0, 0, -box.Width * 2));
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            camera.position = new Vector3(box.Width / 2, box.Height / 2, box.Depth / 2);

            if (camera.rotation == LOOKING_UP)
                camera.rotation = LOOKING_FORWARD;
            else if (camera.rotation == LOOKING_DOWN)
                camera.rotation = LOOKING_BACKWARD;
            else
                camera.rotation = LOOKING_UP;

            camera.Translate(new Vector3Int(0, 0, -box.Width * 2));
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            camera.position = new Vector3(box.Width / 2, box.Height / 2, box.Depth / 2);

            if (camera.rotation == LOOKING_UP || camera.rotation == LOOKING_DOWN)
                camera.rotation = LOOKING_LEFT;
            else
                camera.rotation = NewAngle(camera.rotation, true);

            camera.Translate(new Vector3Int(0, 0, -box.Width * 2));
        }
        if (Input.GetKeyUp(KeyCode.D))
        {

            camera.position = new Vector3(box.Width / 2, box.Height / 2, box.Depth / 2);

            if (camera.rotation == LOOKING_UP || camera.rotation == LOOKING_DOWN)
                camera.rotation = LOOKING_RIGHT;
            else
                camera.rotation = NewAngle(camera.rotation, false);

            camera.Translate(new Vector3Int(0, 0, -box.Width * 2));
        }



    }
    // I honestly hate that i wrote this cause how complex i made it but it gets the job done
    private Quaternion NewAngle(Quaternion currentAngle, bool right)
    {
        int newAngleIndex = 0;
        for (; newAngleIndex < lookingDirection.Length; newAngleIndex++)
            if (currentAngle == lookingDirection[newAngleIndex])
                break;

        newAngleIndex += right ? 1 : -1;
        if (newAngleIndex < 0)
            newAngleIndex = lookingDirection.Length - 1;
        else if (newAngleIndex >= lookingDirection.Length)
            newAngleIndex = 0;


        return lookingDirection[newAngleIndex];
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
    void CreateVoxelBox()
    {
        foreach (KeyValuePair<Vector3Int, GameObject> currentVoxel in box.voxels)
            currentVoxel.Value.transform.GetComponent<Renderer>().material = GetCube(currentVoxel.Value.GetComponent<Cell>());
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
