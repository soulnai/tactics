﻿using UnityEngine;
using System.Collections;

public delegate void UIEvent();
public delegate void UnitBaseEvent(Unit unit);
public delegate void UnitAttributeEvent(Unit unit,BaseAttribute at);
public delegate void UnitInteractionEvent(Unit owner,Unit target);
public delegate void UnitEffectEvent(Unit u,BaseEffect ef);

public static class UnitEvents {
	public static event UIEvent onLockUI;
	public static event UIEvent onUnlockUI;
	public static event UnitBaseEvent onUnitReactionEnd;
	public static event UnitBaseEvent onMouseOverUnit;
	public static event UnitBaseEvent onUnitSelectionChanged;
	public static event UnitAttributeEvent onAttributeChanged;
	public static event UnitInteractionEvent onUnitFXEnd;
	public static event UnitEffectEvent OnUnitEffectChanged;
	public static event UnitEffectEvent OnUnitEffectAdded;
	public static event UnitEffectEvent OnUnitEffectRemoved;


	public static void LockUI(){
		if(onLockUI!=null)
			onLockUI();
	}

	public static void UnlockUI(){
		if(onUnlockUI!=null)
			onUnlockUI();
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
