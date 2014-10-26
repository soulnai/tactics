using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnumSpace;



public class AIPlayer : Unit {
	public BaseAbility a;

	private static GameManager gm{
		get{
			return GameManager.instance;
		}
	}

	// Use this for initialization
	void Start () {
			
	}
		
	// Update is called once per frame
	public override void Update () {
		if (GameManager.instance.currentUnit == this &&
		    UnitAction != unitActions.moving &&
		    UnitAction != unitActions.attacking &&
		    UnitState != unitStates.dead) {
						AIturn ();
				}
		base.Update();
	}
	
	public void AIturn ()
	{
		if (AP <= 0) {
			EndTurn();
		}
		else{
		a = unitAbilitiesController.abilities[Random.Range(0, unitAbilitiesController.abilities.Count-1)];

			if ((AP > 0) && (MP >= a.MPCost)) {
								attackRange = a.range;
								attackDistance = a.range;
								damageBase = a.baseDamage;
								//magic
				if (a.attackType == attackTypes.magic) {
						gm.MagicPrefab = MagicPrefabHolder.instance.Freeze;
						gm.MagicExplosionPrefab = MagicPrefabHolder.instance.FreezeExplode;
				}
				//ranged
				else if (a.attackType == attackTypes.ranged) {
										gm.MagicPrefab = MagicPrefabHolder.instance.Lightning;
										gm.MagicExplosionPrefab = MagicPrefabHolder.instance.LightningExplode;
								}
				//melee
				else if (a.attackType == attackTypes.melee) {

								}
				//stun
				else if (a.attackType == attackTypes.ranged) {
										//UnitAction = unitActions.rangedAttack;
										gm.MagicPrefab = MagicPrefabHolder.instance.Poison;
										gm.MagicExplosionPrefab = MagicPrefabHolder.instance.PoisonExplode;
								} else if (a.attackType == attackTypes.heal) {
										//UnitAction = unitActions.healAttack;
										gm.MagicPrefab = MagicPrefabHolder.instance.Heal;
										gm.MagicExplosionPrefab = MagicPrefabHolder.instance.HealExplode;
								}
			}
					//priority queue
					List<Tile> attacktilesInRange = TileHighlightAtack.FindHighlight (gm.map [(int)gridPosition.x] [(int)gridPosition.y], attackRange);
					List<Tile> movementToAttackTilesInRange = TileHighlight.FindHighlight (gm.map [(int)gridPosition.x] [(int)gridPosition.y], movementPerActionPoint + attackRange,maxHeightDiff);
					List<Tile> movementTilesInRange = TileHighlight.FindHighlight (gm.map [(int)gridPosition.x] [(int)gridPosition.y], movementPerActionPoint + 100,maxHeightDiff);
					//attack if in range and with lowest HP
					if (attacktilesInRange.Where (x => gm.unitsAll.Where (y => y.GetType () != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0).Count () > 0) {
					var opponentsInRange = attacktilesInRange.Select (x => gm.unitsAll.Where (y => y.GetType () != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? gm.unitsAll.Where (y => y.gridPosition == x.gridPosition).First () : null).ToList ();
					Unit opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).First ();

					gm.removeTileHighlights ();

					UnitAction = unitActions.attacking;
					gm.AttackhighlightTiles (gridPosition, Color.red, attackRange, true);
					gm.useAbility(a,this,null,opponent);	
					AP = 0;
					
			}
				//move toward nearest attack range of opponent
			else if (UnitAction != unitActions.moving && movementToAttackTilesInRange.Where(x => gm.unitsAll.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count () > 0) {
				var opponentsInRange = movementToAttackTilesInRange.Select(x => gm.unitsAll.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? gm.unitsAll.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
				Unit opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath(gm.map[(int)gridPosition.x][(int)gridPosition.y],gm.map[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First ();

				gm.removeTileHighlights();

				gm.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);
				UnitAction = unitActions.moving;
				List<Tile> path = TilePathFinder.FindPath (gm.map[(int)gridPosition.x][(int)gridPosition.y],gm.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], gm.unitsAll.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
				gm.moveUnitTo(path[(int)Mathf.Max(0, path.Count - 1 - attackRange)]);
			}
			//move toward nearest opponent
			else if (UnitAction != unitActions.moving && movementTilesInRange.Where(x => gm.unitsAll.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count () > 0) {
				var opponentsInRange = movementTilesInRange.Select(x => gm.unitsAll.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? gm.unitsAll.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
				Unit opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath(gm.map[(int)gridPosition.x][(int)gridPosition.y],gm.map[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First ();

				gm.removeTileHighlights();

				UnitAction = unitActions.moving;

				gm.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);
				UnitAction = unitActions.moving;
				List<Tile> path = TilePathFinder.FindPath (gm.map[(int)gridPosition.x][(int)gridPosition.y],gm.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], gm.unitsAll.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
				int movementCost = 0;
				int pathPoint = 0;
				for(int i=0;i<path.Count-1;i++)
				{
					movementCost +=path[i].movementCost;
					if(movementCost <= movementPerActionPoint)
					{
						pathPoint = i;
					}
				}
				Debug.Log(movementCost);
				Debug.Log(pathPoint);
				gm.moveUnitTo(path[pathPoint]);
			}
		}
	}
}
