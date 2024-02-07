using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum Type
    {
        Empty,
        Mine,
        Number,
    }
    public Type type;
    public int num;
    public bool flagged;
    public bool showing;
    public bool detonated;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

