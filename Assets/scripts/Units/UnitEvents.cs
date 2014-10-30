using UnityEngine;
using System.Collections;

public delegate void UnitEventHandler(Unit unit);
public delegate void UnitAttributeHandler(Unit unit,BaseAttribute at);
public delegate void UnitInteractionEventHandler(Unit owner,Unit target);
public delegate void UnitEffectEvent(Unit u,BaseEffect ef);

public static class UnitEvents {
	public static event UnitEventHandler onUnitReactionEnd;
	public static event UnitEventHandler onMouseOverUnit;
	public static event UnitAttributeHandler onAttributeChanged;
	public static event UnitInteractionEventHandler onUnitFXEnd;
	public static event UnitEffectEvent OnUnitEffectChanged;
	public static event UnitEffectEvent OnUnitEffectAdded;
	public static event UnitEffectEvent OnUnitEffectRemoved;

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
