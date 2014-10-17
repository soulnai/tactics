using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

[System.Serializable]
public class UnitManager : MonoBehaviour {
	
	public static UnitManager instance;
	public List<GameObject> units;
	
	public void Awake()
	{
		instance = this;
	}
	
	public Unit getUnit(unitClass unitClass)
	{
		return units.Find(Unit => Unit.GetComponent<Unit>().UnitClass == unitClass).GetComponent<Unit>(); 
	}

	public Unit getUnit(Unit u)
	{
		return units.Find(Unit => Unit.GetComponent<Unit>().UnitClass == u.UnitClass).GetComponent<Unit>(); 
	}
}
