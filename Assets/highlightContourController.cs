using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class highlightContourController : MonoBehaviour {
	public GameObject cornerLT;
	public GameObject cornerLB;
	public GameObject cornerRT;
	public GameObject cornerRB;
	public GameObject sideL;
	public GameObject sideR;
	public GameObject sideT;
	public GameObject sideB;

	private List<GameObject> highlightElements = new List<GameObject>();
	private Tile ownerTile;
	// Use this for initialization
	void Awake() {
		ownerTile = GetComponentInParent<Tile>();
		highlightElements.Add(cornerLT);
		highlightElements.Add(cornerLB);
		highlightElements.Add(cornerRT);
		highlightElements.Add(cornerRB);
		highlightElements.Add(sideL);
		highlightElements.Add(sideR);
		highlightElements.Add(sideT);
		highlightElements.Add(sideB);

//		foreach(GameObject go in highlightElements)
//			go.SetActive(false);
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void showContour(List<Tile> selection){
		foreach(Tile t in ownerTile.neighbors){
			//up
			if(t.gridPosition.y < ownerTile.gridPosition.y){
				if(selection.Contains(t)){
//					cornerLT.SetActive(false);
//					cornerLB.SetActive(false);
					sideT.SetActive(false);
				}
				else
				{
//					cornerLT.SetActive(true);
//					cornerLB.SetActive(true);
					sideT.SetActive(true);
				}
			}
			//down
			if(t.gridPosition.y > ownerTile.gridPosition.y){
				if(selection.Contains(t)){
//					cornerRB.SetActive(false);
//					cornerLB.SetActive(false);
					sideB.SetActive(false);
				}
				else
				{
//					cornerRB.SetActive(true);
//					cornerLB.SetActive(true);
					sideB.SetActive(true);
				}
			}
			//left
			if(t.gridPosition.x < ownerTile.gridPosition.x){
				if(selection.Contains(t)){
//					cornerLB.SetActive(false);
					sideL.SetActive(false);
				}
				else
				{
//					cornerLB.SetActive(true);
					sideL.SetActive(true);
				}
			}
			//right
			if(t.gridPosition.x > ownerTile.gridPosition.x){
				if(selection.Contains(t)){
//					cornerRT.SetActive(false);
//					cornerRB.SetActive(false);
					sideR.SetActive(false);
				}
				else
				{
//					cornerRT.SetActive(true);
//					cornerRB.SetActive(false);
					sideR.SetActive(true);
				}
			}
		}
	}

	public void hideContour(){
		foreach(GameObject go in highlightElements)
			go.SetActive(false);
	}
}
