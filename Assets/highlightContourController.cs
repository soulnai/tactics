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

	public List<Vector3> getContour(List<Tile> selection){
		List<Vector3> tempList = new List<Vector3>();
		int count = 0;
		foreach(Tile t in ownerTile.neighbors)
			if(selection.Contains(t))
				count++;
		if(count == 4)
			return null;

		//up
		if((ownerTile.upNeighbour==null)||(!selection.Contains(ownerTile.upNeighbour))){
			if(!tempList.Contains(cornerLT.transform.position))	tempList.Add(cornerLT.transform.position);
			if(!tempList.Contains(cornerRT.transform.position)) tempList.Add(cornerRT.transform.position);
		}
		//right
		if((ownerTile.rightNeighbour==null)||(!selection.Contains(ownerTile.rightNeighbour))){
			if(!tempList.Contains(cornerRT.transform.position)) tempList.Add(cornerRT.transform.position);
			if(!tempList.Contains(cornerRB.transform.position)) tempList.Add(cornerRB.transform.position);
		}
		//down
		if((ownerTile.downNeighbour==null)||(!selection.Contains(ownerTile.downNeighbour))){
			if(!tempList.Contains(cornerRB.transform.position)) tempList.Add(cornerRB.transform.position);
			if(!tempList.Contains(cornerLB.transform.position)) tempList.Add(cornerLB.transform.position);
		}
		//left
		if((ownerTile.leftNeighbour==null)||(!selection.Contains(ownerTile.leftNeighbour))){
			if(!tempList.Contains(cornerLB.transform.position)) tempList.Add(cornerLB.transform.position);
			if(!tempList.Contains(cornerLT.transform.position)) tempList.Add(cornerLT.transform.position);
		}
		tempList.Sort(new GUImanager.Vector3Sorter());
		return tempList;
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
