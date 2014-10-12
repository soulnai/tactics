using UnityEngine;
using System.Collections;

public class UnitEvents : MonoBehaviour {

	public delegate void UnitEventHandler(Unit unit);

	public static event UnitEventHandler onUnitReactionStart;
	public static event UnitEventHandler onUnitReactionEnd;

	public static void ReactionEnd(Unit unit)
	{
		if(onUnitReactionEnd != null)
			onUnitReactionEnd(unit);
	}
}
