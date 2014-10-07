using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Player {

	public string playerName;
	public List<Unit> units = new List<Unit>();
	private int currentUnit;

	public void addUnit(Unit u)
	{
		units.Add(u);
	}

	public void removeUnit(Unit u)
	{
		units.Remove(u);
		Debug.Log("removed - "+u.name);
	}

	public Unit nextUnit()
	{
		currentUnit++;
		if(currentUnit >= units.Count)
		{
			currentUnit = 0;
		}
		return units[currentUnit];
	}
}
