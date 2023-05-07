using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    public Toggle toggle1;
    public Toggle toggle2;
    public static bool AI1;
    public static bool AI2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AI1 = toggle1.isOn;
        AI2 = toggle2.isOn;
    }
}
