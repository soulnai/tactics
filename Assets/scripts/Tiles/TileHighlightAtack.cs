using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileHighlightAtack {
	
	public TileHighlightAtack () {
		
	}
	
	public static List<Tile> FindHighlight(Tile originTile, int atackRange, float maxHeightDiff = 100f) {
		return FindHighlight(originTile, atackRange, new Vector2[0],maxHeightDiff);
	}
	
	public static List<Tile> FindHighlight(Tile originTile, int atackRange, Vector2[] occupied, float maxHeightDiff = 100f) {
		List<Tile> closed = new List<Tile>();
		List<TilePathAtack> open = new List<TilePathAtack>();
		
		TilePathAtack originPath = new TilePathAtack();
		originPath.addTile(originTile);
		
		open.Add(originPath);
		
		while (open.Count > 0) {
			TilePathAtack current = open[0];
			open.Remove(open[0]);
			
			if (closed.Contains(current.lastTile)) {
				continue;
			} 
			if (current.costOfPath > atackRange + 1) {
				continue;
			}
			
			closed.Add(current.lastTile);
			
			foreach (Tile t in current.lastTile.neighbors) {	
				if (t.impassible || occupied.Contains(t.gridPosition) || (Mathf.Abs(current.lastTile.height-t.height)>maxHeightDiff && GameManager.instance.currentUnit.currentAbility.attackType == EnumSpace.attackTypes.melee)) continue;
				TilePathAtack newTilePath = new TilePathAtack(current);
				newTilePath.addTile(t);
				open.Add(newTilePath);
			}
		}
		closed.Remove(originTile);
		closed.Distinct();
		return closed;
	}
}

