using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

[System.Serializable]
public class UnitManager : MonoBehaviour {
	
	public static UnitManager instance;
	public List<Unit> units;
	
	public void Awake()
	{
		instance = this;
	}
	
	public Unit getUnit(unitClass unitClass)
	{
		return units.Find(Unit => Unit.UnitClass == unitClass); 
	}
}
