using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

public class UnitAttributesHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void createAttributesList()
	{
		Unit u = GetComponent<Unit>();
		List<BaseAttribute> attributes = u.attributes;
		attributes.Clear();

		//standart parameters
		attributes.Add(new BaseAttribute());
		attributes[0].name = "HP";
		attributes[0].attribute = unitAttributes.HP;

		attributes.Add(new BaseAttribute());
		attributes[1].name = "HPmax";
		attributes[1].attribute = unitAttributes.HPmax;

		attributes.Add(new BaseAttribute());
		attributes[2].name = "AP";
		attributes[2].attribute = unitAttributes.AP;

		attributes.Add(new BaseAttribute());
		attributes[3].name = "APmax";
		attributes[3].attribute = unitAttributes.APmax;

		attributes.Add(new BaseAttribute());
		attributes[4].name = "MP";
		attributes[4].attribute = unitAttributes.MP;

		attributes.Add(new BaseAttribute());
		attributes[5].name = "MPmax";
		attributes[5].attribute = unitAttributes.MPmax;

		//stats
		attributes.Add(new BaseAttribute());
		attributes[6].name = "Strenght";
		attributes[6].attribute = unitAttributes.strenght;

		attributes.Add(new BaseAttribute());
		attributes[7].name = "Dexterity";
		attributes[7].attribute = unitAttributes.dexterity;

		attributes.Add(new BaseAttribute());
		attributes[8].name = "Magic";
		attributes[8].attribute = unitAttributes.magic;

		//defs
		attributes.Add(new BaseAttribute());
		attributes[9].name = "Physical def";
		attributes[9].attribute = unitAttributes.PhysicalDef;

		attributes.Add(new BaseAttribute());
		attributes[10].name = "Magic def";
		attributes[10].attribute = unitAttributes.magicDef;

		attributes.Add(new BaseAttribute());
		attributes[11].name = "Fire def";
		attributes[11].attribute = unitAttributes.fireDef;

		attributes.Add(new BaseAttribute());
		attributes[12].name = "Ice def";
		attributes[12].attribute = unitAttributes.iceDef;

		attributes.Add(new BaseAttribute());
		attributes[13].name = "Poison def";
		attributes[13].attribute = unitAttributes.poisonDef;
	}
}
