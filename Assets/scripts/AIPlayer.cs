using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnumSpace;

public class AIPlayer : Unit {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
		if(GameManager.instance.currentUnit == this)
			AIturn();
		base.Update();
	}
	
	public void AIturn ()
	{
		if (UnitAction != unitActions.moving){
			//priority queue
			List<Tile> attacktilesInRange = TileHighlightAtack.FindHighlight(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], attackRange);
			List<Tile> movementToAttackTilesInRange = TileHighlight.FindHighlight(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], movementPerActionPoint + attackRange);
			List<Tile> movementTilesInRange = TileHighlight.FindHighlight(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], movementPerActionPoint + 1000);
			//attack if in range and with lowest HP
			if (attacktilesInRange.Where(x => GameManager.instance.units.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count () > 0) {
				var opponentsInRange = attacktilesInRange.Select(x => GameManager.instance.units.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? GameManager.instance.units.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
				Unit opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).First ();

				GameManager.instance.removeTileHighlights();

				UnitAction = unitActions.meleeAttack;

				GameManager.instance.AttackhighlightTiles(gridPosition, Color.red, attackRange,true);

				GameManager.instance.attackWithCurrentPlayer(GameManager.instance.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y]);
			}
			//move toward nearest attack range of opponent
			else if (UnitAction != unitActions.moving && movementToAttackTilesInRange.Where(x => GameManager.instance.units.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count () > 0) {
				var opponentsInRange = movementToAttackTilesInRange.Select(x => GameManager.instance.units.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? GameManager.instance.units.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
				Unit opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y],GameManager.instance.map[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First ();

				GameManager.instance.removeTileHighlights();

				GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);

				List<Tile> path = TilePathFinder.FindPath (GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y],GameManager.instance.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], GameManager.instance.units.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
				GameManager.instance.moveCurrentPlayer(path[(int)Mathf.Max(0, path.Count - 1 - attackRange)]);
			}
			//move toward nearest opponent
			else if (UnitAction != unitActions.moving && movementTilesInRange.Where(x => GameManager.instance.units.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count () > 0) {
				var opponentsInRange = movementTilesInRange.Select(x => GameManager.instance.units.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? GameManager.instance.units.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
				Unit opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y],GameManager.instance.map[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First ();

				GameManager.instance.removeTileHighlights();

				UnitAction = unitActions.moving;

				GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);
				
				List<Tile> path = TilePathFinder.FindPath (GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y],GameManager.instance.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], GameManager.instance.units.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
				GameManager.instance.moveCurrentPlayer(path[(int)Mathf.Min(Mathf.Max (path.Count - 1 - 1, 0), movementPerActionPoint - 1)]);
			}
		}
		checkAP();
	}
}
