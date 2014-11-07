using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;

[System.Serializable]
public class Player {

	public string playerName;
	public playerType type = playerType.player;
	public List<Unit> units = new List<Unit>();
	public List<Unit> unitsDead = new List<Unit>();
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
