using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileHighlight {
	
	public TileHighlight () {
		
	}

	public static List<Tile> FindHighlight(Tile originTile, int movementPoints, float maxHeightDiff = 100f) {
		return FindHighlight(originTile, movementPoints, new Vector2[0],maxHeightDiff);
	}

	public static List<Tile> FindHighlight(Tile originTile, int movementPoints, Vector2[] occupied,float maxHeightDiff = 100f) {
		List<Tile> closed = new List<Tile>();
		List<TilePath> open = new List<TilePath>();
		
		TilePath originPath = new TilePath();
		originPath.addTile(originTile);
		
		open.Add(originPath);
		
		while (open.Count > 0) {
			TilePath current = open[0];
			open.Remove(open[0]);
			
			if (closed.Contains(current.lastTile)) {
				continue;
			} 
			if (current.costOfPath > movementPoints) {
				continue;
			}
			
			closed.Add(current.lastTile);
			
			foreach (Tile t in current.lastTile.neighbors) {	
				if (t.impassible || occupied.Contains(t.gridPosition) || Mathf.Abs(current.lastTile.height-t.height)>maxHeightDiff) continue;
				TilePath newTilePath = new TilePath(current);
				newTilePath.addTile(t);
				open.Add(newTilePath);
			}
		}
		closed.Remove(originTile);
		closed.Distinct();
		return closed;
	}
}
