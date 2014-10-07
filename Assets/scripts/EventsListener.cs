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
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void AddListener(Unit u)
	{
		u.OnUnitAnimationEnd += HandleOnUnitAnimationEnd;
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
