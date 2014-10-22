using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnumSpace;

public class AIPlayer : Unit {
	public BaseAbility a;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
		if (GameManager.instance.currentUnit == this && UnitAction != unitActions.moving && UnitAction != unitActions.rangedAttack && UnitAction != unitActions.magicAttack) {
						AIturn ();
				}
		base.Update();
	}
	
	public void AIturn ()
	{
		if (AP <= 0) {
//			GameManager.instance.nextTurn();		
		}

		a = unitAbilitiesController.abilities[Random.Range(0, unitAbilitiesController.abilities.Count)];
		//onAbility (ability);

		if ((AP > 0) && (MP >= a.MPCost)) {
						if (unitAbilitiesController.abilities.Contains (a)) {
				
								attackRange = a.range;
								attackDistance = a.range;
								damageBase = a.baseDamage;
								//magic
								if (a.attackType == attackTypes.magic) {
										//UnitAction = unitActions.magicAttack;
										GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Freeze;
										GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.FreezeExplode;
								}
				//ranged
				else if (a.attackType == attackTypes.ranged) {
										//UnitAction = unitActions.rangedAttack;
										GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Lightning;
										GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.LightningExplode;
								}
				//melee
				else if (a.attackType == attackTypes.melee) {
										//UnitAction = unitActions.meleeAttack;
								}
				//stun
				else if (a.attackType == attackTypes.ranged) {
										//UnitAction = unitActions.rangedAttack;
										GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Poison;
										GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.PoisonExplode;
								} else if (a.attackType == attackTypes.heal) {
										//UnitAction = unitActions.healAttack;
										GameManager.instance.MagicPrefab = MagicPrefabHolder.instance.Heal;
										GameManager.instance.MagicExplosionPrefab = MagicPrefabHolder.instance.HealExplode;
								}
						}
				}



		if (UnitAction != unitActions.moving) {
						//priority queue
						List<Tile> attacktilesInRange = TileHighlightAtack.FindHighlight (GameManager.instance.map [(int)gridPosition.x] [(int)gridPosition.y], attackRange);
						List<Tile> movementToAttackTilesInRange = TileHighlight.FindHighlight (GameManager.instance.map [(int)gridPosition.x] [(int)gridPosition.y], movementPerActionPoint + attackRange);
						List<Tile> movementTilesInRange = TileHighlight.FindHighlight (GameManager.instance.map [(int)gridPosition.x] [(int)gridPosition.y], movementPerActionPoint + 1000);
						//attack if in range and with lowest HP
						if (attacktilesInRange.Where (x => GameManager.instance.units.Where (y => y.GetType () != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0).Count () > 0) {
								var opponentsInRange = attacktilesInRange.Select (x => GameManager.instance.units.Where (y => y.GetType () != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? GameManager.instance.units.Where (y => y.gridPosition == x.gridPosition).First () : null).ToList ();
								Unit opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).First ();

								GameManager.instance.removeTileHighlights ();

								if (unitAbilitiesController.abilities.Contains (a)) {
				
										//magic
										if (a.attackType == attackTypes.magic) {
												UnitAction = unitActions.magicAttack;

										}
					//ranged
					else if (a.attackType == attackTypes.ranged) {
												UnitAction = unitActions.rangedAttack;

										}
					//melee
					else if (a.attackType == attackTypes.melee) {
												UnitAction = unitActions.meleeAttack;
										}
					//stun
					else if (a.attackType == attackTypes.ranged) {
												UnitAction = unitActions.rangedAttack;

										}/* else if (a.attackType == attackTypes.heal) {
						UnitAction = unitActions.healAttack;

					}*/
								}

								GameManager.instance.AttackhighlightTiles (gridPosition, Color.red, attackRange, true);

								if (UnitAction == unitActions.rangedAttack || UnitAction == unitActions.magicAttack) {
										//GameManager.instance.distanceAttackWithCurrentPlayer (GameManager.instance.map [(int)opponent.gridPosition.x] [(int)opponent.gridPosition.y],a); 
					GameManager.instance.useAbility(a,this,null,opponent);
								} else {
										//GameManager.instance.attackWithCurrentPlayer (GameManager.instance.map [(int)opponent.gridPosition.x] [(int)opponent.gridPosition.y],a); 
					GameManager.instance.useAbility(a,this,null,opponent);
								}
						}
				//move toward nearest attack range of opponent
			else if (UnitAction != unitActions.moving && movementToAttackTilesInRange.Where(x => GameManager.instance.units.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count () > 0) {
				var opponentsInRange = movementToAttackTilesInRange.Select(x => GameManager.instance.units.Where (y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count () > 0 ? GameManager.instance.units.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
				Unit opponent = opponentsInRange.OrderBy (x => x != null ? -x.HP : 1000).ThenBy (x => x != null ? TilePathFinder.FindPath(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y],GameManager.instance.map[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First ();

				GameManager.instance.removeTileHighlights();

				GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);
				UnitAction = unitActions.moving;
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
				UnitAction = unitActions.moving;
				List<Tile> path = TilePathFinder.FindPath (GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y],GameManager.instance.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], GameManager.instance.units.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
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
				GameManager.instance.moveCurrentPlayer(path[(int)Mathf.Min(Mathf.Max (path.Count - 1 - attackRange, 0), pathPoint)]);
			}
		}
	}
}
