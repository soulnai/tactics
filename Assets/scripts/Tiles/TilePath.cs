using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TilePath {
	public List<Tile> listOfTiles = new List<Tile>();

	public int costOfPath = 0;	
	
	public Tile lastTile;
	
	public TilePath() {}
	
	public TilePath(TilePath tp) {
		listOfTiles = tp.listOfTiles.ToList();
		costOfPath = tp.costOfPath;
		lastTile = tp.lastTile;
	}
	
	public void addTile(Tile t) {
		if (GameManager.instance.units [GameManager.instance.currentUnitIndex].currentTile == t) {
						costOfPath += 0;
				} else if (GameManager.instance.units [GameManager.instance.currentUnitIndex].isFlying == true) {
						costOfPath += 1;
				} else {
			costOfPath += t.movementCost;
				}
		listOfTiles.Add(t);
		lastTile = t;
	}
}