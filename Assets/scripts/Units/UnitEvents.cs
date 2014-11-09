using UnityEngine;
using System.Collections;

public delegate void UIEvent();
public delegate void UIConfirmEvent(guiConfirmFunc func);
public delegate void PlayerBaseEvent(Player player);
public delegate void TileBaseEvent(Tile tile);
public delegate void UnitBaseEvent(Unit unit);
public delegate void UnitAttributeEvent(Unit unit,BaseAttribute at);
public delegate void UnitInteractionEvent(Unit owner,Unit target);
public delegate void UnitEffectEvent(Unit u,BaseEffect ef);

public static class UnitEvents {
	public static event PlayerBaseEvent OnPlayerTurnStart;
	public static event PlayerBaseEvent OnPlayerTurnEnd;
	public static event PlayerBaseEvent OnVictoryState;
	public static event UIEvent onLockUI;
	public static event UIEvent onUnlockUI;
	public static event UIConfirmEvent onConfirmRequest;
	public static event TileBaseEvent onTileClick;
	public static event TileBaseEvent onTileCursorOverChanged;
	public static event UnitBaseEvent onUnitReactionEnd;
	public static event UnitBaseEvent onMouseOverUnit;
	public static event UnitBaseEvent onUnitSelectionChanged;
	public static event UnitBaseEvent OnUnitPosChange;
	public static event UnitBaseEvent OnUnitTurnStart;
	public static event UnitBaseEvent OnUnitTurnEnd;
	public static event UnitAttributeEvent onAttributeChanged;
	public static event UnitInteractionEvent onUnitFXEnd;
	public static event UnitEffectEvent OnUnitEffectChanged;
	public static event UnitEffectEvent OnUnitEffectAdded;
	public static event UnitEffectEvent OnUnitEffectRemoved;

	public static void TileCursorOverChanged(Tile t){
		if(onTileCursorOverChanged != null)
			onTileCursorOverChanged(t);
	}

	public static void VictoryState(Player p){
		if(OnVictoryState != null)
			OnVictoryState(p);
	}

	public static void PlayerTurnEnd(Player p){
		if(OnPlayerTurnEnd != null)
			OnPlayerTurnEnd(p);
	}

	public static void PlayerTurnStart(Player p){
		if(OnPlayerTurnStart != null)
			OnPlayerTurnStart(p);
	}

	public static void UnitPosChanged(Unit u){
		if(OnUnitPosChange != null)
			OnUnitPosChange(u);
	}

	public static void UnitTurnStart(Unit u){
		if(OnUnitTurnStart != null)
			OnUnitTurnStart(u);
	}

	public static void UnitTurnEnd(Unit u){
		if(OnUnitTurnEnd != null)
			OnUnitTurnEnd(u);
	}

	public static void TileClick(Tile t){
		if(onTileClick!=null)
			onTileClick(t);
	}

	public static void LockUI(){
		if(onLockUI!=null)
			onLockUI();
	}

	public static void UnlockUI(){
		if(onUnlockUI!=null)
			onUnlockUI();
	}

	public static void ConfirmRequest(guiConfirmFunc func){
		if(onConfirmRequest!=null)
			onConfirmRequest(func);
	}

	public static void UnitSelectionChanged(Unit u){
		if(onUnitSelectionChanged!=null)
			onUnitSelectionChanged(u);
	}

	public static void UnitAttributeChanged(Unit u,BaseAttribute at){
		if(onAttributeChanged!=null)
			onAttributeChanged(u,at);
	}

	public static void UnitEffectChanged(Unit u,BaseEffect ef){
		if(OnUnitEffectChanged!=null)
			OnUnitEffectChanged(u,ef);
	}

	public static void UnitEffectAdded(Unit u,BaseEffect ef){
		if(OnUnitEffectAdded!=null)
			OnUnitEffectAdded(u,ef);
	}

	public static void UnitEffectRemoved(Unit u,BaseEffect ef){
		if(OnUnitEffectRemoved!=null)
			OnUnitEffectRemoved(u,ef);
	}

	public static void ReactionEnd(Unit unit)
	{
		if(onUnitReactionEnd != null)
			onUnitReactionEnd(unit);
	}

	public static void MouseOverUnit(Unit unit)
	{
		if(onMouseOverUnit != null)
			onMouseOverUnit(unit);
	}

	public static void FXEnd(Unit owner,Unit target)
	{
		if(onUnitFXEnd != null)
			onUnitFXEnd(owner,target);
	}
}
