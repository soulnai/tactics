using UnityEngine;
using System.Collections;

public class ColorHolder : MonoBehaviour {
	public static ColorHolder instance;
	public Color attack;
	public Color area;
	public Color move;
	public Material pathMat;

	void Awake () {
		instance = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
