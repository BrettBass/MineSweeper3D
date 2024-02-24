using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [SerializeField] private TextMeshProUGUI gameStatus = null;
    [SerializeField] private Camera cam = null;
    private int buffer = 5;

    private Box box;
    public int Width = 16, Height = 16, Depth = 16;
    private Vector3 center = new Vector3();
    RaycastHit tmpHitHighliht;
    private int noneMineCount;
    private bool gameover = false;

    [SerializeField] private TextMeshProUGUI textTimer = null;
    public float currentTime;

    private void OnEnable()
    {
        UIModifier.OnChangeSize += UIManager_OnChanngSize;
    }
    private void UpdateGameStatus(string status)
    {
        gameStatus.text = status;
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
        center = new Vector3(Width / 2f, Height / 2f, Depth / 2f);
        buffer = Mathf.Max(Width, Height, Depth) / 4;
        CreateVoxelBox();
        SetupCamera();
        noneMineCount = box.voxels.Count - box.getMineCount();
        UpdateGameStatus(noneMineCount.ToString());
        gameover = false;
        ResetTimer();
    }

    // Start is called before the first frame update
    void Start()
    {
        newGame();
    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var camera = cam.transform;

        if (noneMineCount <= 0)
        {
            Debug.Log("winner");
            box.MarkMines(TileFlag);
            UpdateGameStatus("Winner");
            gameover = true;
        }
        if (!gameover)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= 3600) // an hour
                ResetTimer();

            DisplayTime(currentTime);

            if (Input.GetMouseButtonUp(0))
                if (Physics.Raycast(ray, out tmpHitHighliht, 100))
                {
                    //Renderer renderer = tmpHitHighliht.transform.GetComponent<Renderer>();

                    Vector3Int position = Vector3Int.CeilToInt(tmpHitHighliht.transform.position);
                    Cell cell = box.voxels[position].GetComponent<Cell>();
                    Renderer renderer = box.voxels[position].GetComponent<Renderer>();
                    if (!cell.showing && !cell.flagged)
                    {
                        if (cell.type == Cell.Type.Mine)
                        {
                            Debug.Log($"We hit a bomb: {tmpHitHighliht.transform.name}");
                            //renderer.material = TileExploded;
                            box.MarkMines(TileMine);
                            box.voxels[position].GetComponent<Renderer>().material = TileExploded;
                            UpdateGameStatus("Loser");
                            gameover = true;
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
                }
            if (Input.GetMouseButtonUp(1))
                if (Physics.Raycast(ray, out tmpHitHighliht, 100))
                {
                    //Renderer renderer = tmpHitHighliht.transform.GetComponent<Renderer>();
                    Vector3Int position = Vector3Int.CeilToInt(tmpHitHighliht.transform.position);
                    Renderer renderer = box.voxels[position].GetComponent<Renderer>();
                    Cell cell = box.voxels[position].GetComponent<Cell>();

                    Debug.Log($"renderer shared material: {renderer.sharedMaterial}");
                    Debug.Log($"TileUnkonwn material: {TileUnknown}");
                    Debug.Log($"TileFlag material: {TileFlag}");

                    if (renderer.sharedMaterial == TileUnknown)
                        renderer.material = TileFlag;


                    else if (renderer.sharedMaterial == TileFlag)
                        renderer.material = TileUnknown;

                    cell.flagged = !cell.flagged;
                }
        }
        if (Input.GetKey(KeyCode.W))
        {
            camera.RotateAround(center, Vector3.right, 60 * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.S))
        {
            camera.RotateAround(center, Vector3.left, 60 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            camera.RotateAround(center, Vector3.up, 60 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            camera.RotateAround(center, Vector3.down, 60 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("Escape hit");
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            SetupCamera();
        }



    }
    private void SetupCamera()
    {
        cam.transform.rotation = Quaternion.LookRotation(new Vector3Int(0, 0, 0));
        cam.transform.position = new Vector3(Width / 2f - buffer, Height / 2f, -1 * Mathf.Sqrt(Mathf.Pow(Width, 2) + Mathf.Pow(Height, 2)));
    }
    public void Reveal(Vector3Int voxel)
    {
        Renderer renderer = box.voxels[voxel].GetComponent<Renderer>();
        Cell cell = box.voxels[voxel].GetComponent<Cell>();

        if (cell.type == Cell.Type.Empty)
            Flood(voxel);

        if (!cell.showing)
            noneMineCount--;
        cell.showing = true;
        tmpHitHighliht.transform.GetComponent<Renderer>().material = GetCube(cell);
        UpdateGameStatus(noneMineCount.ToString());
    }
    private void Flood(Vector3Int voxel)
    {
        Cell cell = box.voxels[voxel].GetComponent<Cell>();
        if (cell.showing) return;
        if (cell.type == Cell.Type.Mine) return;
        Renderer renderer = box.voxels[voxel].GetComponent<Renderer>();

        cell.showing = true;
        noneMineCount--;
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
    private void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        textTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void ResetTimer()
    {
        currentTime = 0;
    }
}
