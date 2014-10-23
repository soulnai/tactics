using UnityEngine;
using System.Collections;

public class UnitEvents : MonoBehaviour {

	public delegate void UnitEventHandler(Unit unit);
	public delegate void UnitInteractionEventHandler(Unit owner,Unit target);
	
	public static event UnitEventHandler onUnitReactionEnd;
	public static event UnitEventHandler onMouseOverUnit;
	public static event UnitInteractionEventHandler onUnitFXEnd;


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
