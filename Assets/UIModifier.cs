using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIModifier : MonoBehaviour
{
    [SerializeField] private Slider widthSlider = null;
    [SerializeField] private Slider heightSlider = null;
    [SerializeField] private Slider depthSlider = null;

    [SerializeField] private TextMeshProUGUI sliderTextWidth = null;
    [SerializeField] private TextMeshProUGUI sliderTextHeight = null;
    [SerializeField] private TextMeshProUGUI sliderTextDepth = null;
    [SerializeField] private int maxSliderAmount = 32;

    public delegate void ChangeSize(int x, int y, int z);
    public static event ChangeSize OnChangeSize;

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

    public void onSliderChange(int id, int value)
    {
        switch (id)
        {
            case 0:
                sliderTextWidth.text = (value * maxSliderAmount).ToString("0");
                break;
            case 1:
                sliderTextHeight.text = (value * maxSliderAmount).ToString("0");
                break;
            case 2:
                sliderTextDepth.text = (value * maxSliderAmount).ToString("0");
                break;
            default:
                break;
        }
    }

    public void onDifficultyButtonClick(int id)
    {
        Debug.Log($"Button clicked");
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
