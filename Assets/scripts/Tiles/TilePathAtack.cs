using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TilePathAtack {
	public List<Tile> listOfTiles = new List<Tile>();
	
	public int costOfPath = 0;	
	
	public Tile lastTile;
	
	public TilePathAtack() {}
	
	public TilePathAtack(TilePathAtack tp) {
		listOfTiles = tp.listOfTiles.ToList();
		costOfPath = tp.costOfPath;
		lastTile = tp.lastTile;
	}
	
	public void addTile(Tile t) {
		costOfPath += 1;
		listOfTiles.Add(t);
		lastTile = t;
	}
}