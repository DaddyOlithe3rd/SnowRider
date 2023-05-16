using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle toggle1;//Ai1
    public Toggle toggle2;//AI2
    public Toggle toggle3;//Courbes de Bézier
    public Toggle toggle4;//Bruit de Perlin

    public static bool AI1 = true;
    public static bool AI2 = true;
    public static bool bezier = true;
    public static bool perlin = false;

    void Start()
    {
        toggle1.isOn = AI1;
        toggle2.isOn = AI2;
        toggle3.isOn = bezier;
        toggle4.isOn = perlin;
    }

    // Update is called once per frame
    void Update()
    {
        AI1 = toggle1.isOn;
        AI2 = toggle2.isOn;
        bezier = toggle3.isOn;
        perlin = toggle4.isOn;
    }
}
