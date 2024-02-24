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

    public delegate void ChangeSize(int x, int y, int z);
    public static event ChangeSize OnChangeSize;

    void Start()
    {
        InitializeSliders();
    }

    public void onDifficultyButtonClick(int id)
    {
        switch (id)
        {
            case 0:
                OnChangeSize?.Invoke(4, 4, 4);
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
    public void InitializeSliders()
    {
        widthSlider.onValueChanged.AddListener((v) =>
        {
            sliderTextWidth.text = v.ToString("0");
        });
        heightSlider.onValueChanged.AddListener((v) =>
        {
            sliderTextHeight.text = v.ToString("0");
        });
        depthSlider.onValueChanged.AddListener((v) =>
        {
            sliderTextDepth.text = v.ToString("0");
        });
    }

}
