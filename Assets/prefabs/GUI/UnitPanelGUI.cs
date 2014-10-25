using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnumSpace;

public class UnitPanelGUI : MonoBehaviour {
	public Image icon;
	public Slider HPslider;
	public Slider MPslider;
	public Slider APslider;

	public Unit targetUnit;
	private bool canUpdate = false;
	// Use this for initialization
	void Start () {
	
	}

	public void Init(Unit target)
	{
		targetUnit = target;
		updateValue(unitAttributes.AP);
		updateValue(unitAttributes.HP);
		updateValue(unitAttributes.MP);
		canUpdate = true;
	}

	public void updateValue(unitAttributes at)
	{
		switch(at){
		case unitAttributes.AP:
			APslider.value = ((float)targetUnit.AP/(float)targetUnit.APmax);
			break;
		case unitAttributes.HP:
			HPslider.value = ((float)targetUnit.HP/(float)targetUnit.HPmax);
			break;
		case unitAttributes.MP:
			MPslider.value = ((float)targetUnit.MP/(float)targetUnit.MPmax);
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		//TODO rework on Events
		if(canUpdate){
			updateValue(unitAttributes.AP);
			updateValue(unitAttributes.HP);
			updateValue(unitAttributes.MP);
		}
	}
}
