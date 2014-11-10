using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VictoryPanelControllerGUI : MonoBehaviour {
	public Text winnerText;
	public Player winnerPlayer;
	// Use this for initialization
	void Awake () {

	}

	void Start () {
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(Player p){
		winnerPlayer = p;
		winnerText.text = "Winner - "+winnerPlayer.playerName;
		gameObject.SetActive(true);
	}
}
