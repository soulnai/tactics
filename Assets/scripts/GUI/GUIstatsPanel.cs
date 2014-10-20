using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnumSpace;


public enum track{
	currentUnit,
	currentTarget
}

public class GUIstatsPanel : MonoBehaviour {
	public track t;
	public Text Name;
	public Text AP;
	public Text MP;
	public Text HP;

	private Unit currentUnit;
	private GameManager gm;
	// Use this for initialization
	void Start () {
		gm = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		switch (t) {
		case track.currentUnit:
			currentUnit = gm.currentUnit;
			break;
		case track.currentTarget:
			currentUnit = gm.targetPub;
			break;
		}
		if(currentUnit != null){
			Name.text ="Name - " + currentUnit.unitName;
			AP.text = "AP - " + currentUnit.getAttribute(unitAttributes.AP).valueMod;
			MP.text = "MP - " + currentUnit.getAttribute(unitAttributes.HP).valueMod;
			HP.text = "HP - " + currentUnit.getAttribute(unitAttributes.MP).valueMod;
		}
	}
}
