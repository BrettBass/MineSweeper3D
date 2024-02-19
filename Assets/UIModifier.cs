using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;


public class UIModifier : MonoBehaviour
{
    [SerializeField] private Slider widthSlider = null;
    [SerializeField] private Slider heightSlider = null;
    [SerializeField] private Slider depthSlider = null;

    [SerializeField] private TextMeshProUGUI sliderTextWidth = null;
    [SerializeField] private TextMeshProUGUI sliderTextHeight = null;
    [SerializeField] private TextMeshProUGUI sliderTextDepth = null;

    [SerializeField] private TextMeshProUGUI textTimer = null;

    public delegate void ChangeSize(int x, int y, int z);
    public static event ChangeSize OnChangeSize;

    Stopwatch stopwatch = Stopwatch.StartNew();

    void Start()
    {
        widthSlider.onValueChanged.AddListener((v) => {
                sliderTextWidth.text = v.ToString("0");
                });
        heightSlider.onValueChanged.AddListener((v) => {
                sliderTextHeight.text = v.ToString("0");
                });
        depthSlider.onValueChanged.AddListener((v) => {
                sliderTextDepth.text = v.ToString("0");
                });
    }
    void Update()
    {

        //Debug.Log("Update - Time value: " + timeValue); // Add this line

        DisplayTime();
        //DisplayTime(timeValue);
    }

    public void DisplayTime()
    {
        textTimer.text = stopwatch.Elapsed.ToString(@"mm\:ss");
    }
    public void onDifficultyButtonClick(int id)
    {
        //Debug.Log($"Button clicked");
        stopwatch.Reset();
        stopwatch.Start();
        switch (id)
        {
            case 0:
                OnChangeSize?.Invoke(8, 8, 8);
                break;
            case 1:
                OnChangeSize?.Invoke(16, 16, 16);
                break;
            case 2:
                OnChangeSize?.Invoke(32, 32, 32);
                break;
            case 3:
                OnChangeSize?.Invoke((int)widthSlider.value, (int)heightSlider.value, (int)depthSlider.value);
                break;
            default:
                break;
        }
    }

}
