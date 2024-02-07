using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Minesweeper : MonoBehaviour
{

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
    //public Texture TileUnknown;
    private Box box;


    RaycastHit tmpHitHighliht;
    // Start is called before the first frame update
    void Start()
    {
        box = new Box(9,9,9);
        CreateCube();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Input.GetMouseButton(0))
        if (Input.GetMouseButtonUp(0))
            if (Physics.Raycast(ray, out tmpHitHighliht, 100))
            {
                if (tmpHitHighliht.transform.gameObject.GetComponent<Cell>().type == Cell.Type.Mine)
                    Debug.Log($"We hit a bomb: {tmpHitHighliht.transform.name}");
                else
                    Debug.Log($"We didn't hit a bomb: {tmpHitHighliht.transform.name}");
            }
        if (Input.GetMouseButtonUp(1))
            if (Physics.Raycast(ray, out tmpHitHighliht, 100))
            {
                //Renderer renderer = tmpHitHighliht.transform.gameObject.GetComponent<Renderer>();
                Renderer renderer = tmpHitHighliht.transform.GetComponent<Renderer>();

                // use shared material to avoid "(instance)"
                Debug.Log($"renderer shared material: {renderer.sharedMaterial}");
                Debug.Log($"TileUnkonwn material: {TileUnknown}");
                Debug.Log($"TileFlag material: {TileFlag}");

                if (renderer.sharedMaterial == TileUnknown)
                    renderer.material = TileFlag;

                else if (renderer.sharedMaterial == TileFlag)
                    renderer.material = TileUnknown;
            }



    }

    void CreateCube()
    {
        for (int k = 0; k<9; k++)
            for (int i = 0; i<9; i++)
                for (int j = 0; j<9; j++)
                {
                    if (!box.Surface(i,j,k)) continue;
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = new Vector3(i, k, j);
                    //go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    go.transform.localScale = new Vector3(1,1,1);
                    go.transform.name = $"[{i},{j}]";

                    //go.transform.GetComponent<Renderer>().material = TileFlag;
                    go.transform.GetComponent<Renderer>().material = TileUnknown;
                    var cd = go.AddComponent<Cell>();

                    if (Random.Range(1,10) > 5)
                    {
                        cd.type = Cell.Type.Mine;
                        //go.transform.GetComponent<Renderer>().material.color = Color.red;
                    }
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
        switch(cell.type)
        {
            case Cell.Type.Mine:    return TileMine;
            case Cell.Type.Empty:   return TileEmpty;
            case Cell.Type.Number:  return GetTileNumber(cell);

            default: return null;
        }
    }
    private Material GetTileNumber(Cell cell)
    {
        switch(cell.num)
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
