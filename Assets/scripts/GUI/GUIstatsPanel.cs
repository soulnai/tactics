using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIstatsPanel : MonoBehaviour {
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
		currentUnit = gm.units[gm.currentUnitIndex];
		Name.text ="Name - " + currentUnit.unitName;
		AP.text = "AP - " + currentUnit.actionPoints;
		MP.text = "MP - " + currentUnit.MP;
		HP.text = "HP - " + currentUnit.HP;
	}
}
