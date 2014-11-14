using UnityEngine;
using System.Collections;
using EnumSpace;

public delegate void UIEvent();
public delegate void UIActionEvent(unitActions actionBefore, unitActions actionAfter);
public delegate void UIConfirmEvent(guiConfirmFunc funcCurrent,guiConfirmFunc funcNew);
public delegate void PlayerBaseEvent(Player player);
public delegate void TileBaseEvent(Tile tile);
public delegate void UnitBaseEvent(Unit unit);
public delegate void UnitAttributeEvent(Unit unit,BaseAttribute at);
public delegate void UnitInteractionEvent(Unit owner,Unit target);
public delegate void UnitEffectEvent(Unit u,BaseEffect ef);

public static class EventManager {
	//player events
	public static event PlayerBaseEvent OnPlayerTurnStart;
	public static event PlayerBaseEvent OnPlayerTurnEnd;
	public static event PlayerBaseEvent OnVictoryState;
	//ui events
	public static event UIEvent onLockUI;
	public static event UIEvent onUnlockUI;
	public static event UIActionEvent onCurrentActionChange;
	public static event UIConfirmEvent onRequestConfirm;
	//tile events
	public static event TileBaseEvent onTileClick;
	public static event TileBaseEvent onTileCursorOverChanged;
	//unit events
	public static event UnitBaseEvent onUnitClick;
	public static event UnitBaseEvent onUnitReactionEnd;
	public static event UnitBaseEvent onMouseOverUnit;
	public static event UnitBaseEvent onUnitSelectionChanged;
	public static event UnitBaseEvent OnUnitPosChange;
	public static event UnitBaseEvent OnUnitTurnStart;
	public static event UnitBaseEvent OnUnitTurnEnd;
	public static event UnitBaseEvent OnUnitCastDelayChanged;
    public static event UnitBaseEvent OnUnitDead;
	public static event UnitAttributeEvent onAttributeChanged;
	public static event UnitInteractionEvent onUnitFXEnd;
	//effect events
	public static event UnitEffectEvent OnUnitEffectChanged;
	public static event UnitEffectEvent OnUnitEffectAdded;
	public static event UnitEffectEvent OnUnitEffectRemoved;

    public static void UnitDead(Unit u)
    {
        if (OnUnitDead != null)
            OnUnitDead(u);
    }
	public static void UnitClick(Unit u){
		if(onUnitClick != null)
			onUnitClick(u);
	}

	public static void UnitCastDelayChanged(Unit u){
		if(OnUnitCastDelayChanged != null)
			OnUnitCastDelayChanged(u);
	}

	public static void TileCursorOverChanged(Tile t){
		if(onTileCursorOverChanged != null)
			onTileCursorOverChanged(t);
	}

	public static void CurrentActionChange(unitActions before,unitActions after){
		Debug.Log(before+" - "+after);
		if(onCurrentActionChange != null)
			onCurrentActionChange(before,after);
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

	public static void RequestConfirm(guiConfirmFunc funcOnConfirm,guiConfirmFunc funcOnDecline){
		if(onRequestConfirm!=null)
			onRequestConfirm(funcOnConfirm,funcOnDecline);
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
