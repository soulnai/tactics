using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ColorHolder : MonoBehaviour {
	public static ColorHolder instance;
	public Color attack;
	public Color area;
	public Color move;
	public Material pathMat;
    public List<ColorObj> ColorsList = new List<ColorObj>();

	void Awake () {
		instance = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Color GetAttributeColor(BaseAttribute at)
    {
        foreach (ColorObj c in ColorsList)
        {
            if (c.Attribute == at.attribute)
            {
                return c.Color;
            }
        }
        return Color.red;
    }
}
