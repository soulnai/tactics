using UnityEngine;
using System.Collections;

public class EventsListener : MonoBehaviour {

	void Awake()
	{

	}
	// Use this for initialization
	void Start () {
		foreach(Unit u in GameManager.instance.units)
		{
			AddListener(u);
//			Debug.Log("Added");
		}
		AddListener(GameManager.instance);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void AddListener(Unit u)
	{
		u.OnUnitAnimationEnd += HandleOnUnitAnimationEnd;
	}

	private void AddListener(GameManager gm)
	{
		gm.OnVictoryState += HandleOnVictoryState;	
	}

	void HandleOnVictoryState (GameManager gm,Player p)
	{
		Debug.Log("Victory - "+p.playerName);
	}

	private void RemoveListener(Unit u)
	{
		u.OnUnitAnimationEnd -= HandleOnUnitAnimationEnd;
	}

	void HandleOnUnitAnimationEnd (Unit unit)
	{
		Debug.Log("EVENT! - "+unit.unitName+" - "+unit.animation.clip.name);	
	}
}
