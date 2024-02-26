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

}

