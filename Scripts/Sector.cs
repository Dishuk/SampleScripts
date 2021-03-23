using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour {

	public float radius;
	public bool isConnectedToMainSystem;
	public Collider[] nearbyPlanetsArray;
	public List<Sector> nearbyPlanets;
	public bool isWarpsGenerated = false;
	public bool isValidated;
	[HideInInspector]public float distanceToCenter;
	[HideInInspector]public float colorChange;
	public float amountOfSectors;
	public bool isCaptured;

	public string fractionName;
	public float connectionRadius;
	public Color color;

	public bool isBordered;

	public Fractions sectorFraction;


	void Awake(){
		radius = GenerateGalaxyMap.instance.distanceScale;
		GetComponent<SphereCollider> ().radius = radius / 4.6f;
	}

	void Start(){
		color.a = 0.2f;
		connectionRadius = radius * 1.2f;
	}

	public void OnDrawGizmosSelected(){
		Gizmos.color = Color.Lerp(Color.green, Color.red, colorChange);
		for (int i = 0; i < nearbyPlanets.Count; i++) {
			Gizmos.DrawLine (transform.position, nearbyPlanets[i].transform.position);
		}

		if (fractionName != "") {
			Gizmos.color = color;
			Gizmos.DrawSphere (transform.position, this.GetComponent<SphereCollider> ().radius);
		}
	}

	public IEnumerator FractionSpreading(Fractions fraction, float amountOfSecs){
		isCaptured = true;
		yield return new WaitForSeconds (amountOfSecs * 0.1f);
		fractionName = fraction.fractionName;
		sectorFraction = fraction;
		color = fraction.fractionColor;
		AddToLists ();
		for (int i = 0; i < nearbyPlanets.Count; i++) {
			if (amountOfSectors < 10) {
				if (nearbyPlanets [i].isCaptured == false) {
					nearbyPlanets [i].Spreadfraction (fraction, amountOfSecs);
				}
			}
		} 
		BorderStatusToSector ();
	}

	void BorderStatusToSector(){
		for (int i = 0; i < nearbyPlanets.Count; i++) {
			if (nearbyPlanets [i].sectorFraction != sectorFraction) {
				isBordered = true;
			} else {
				isBordered = false;
			}
		}
	}

	public void AssignSectorToFraction (Fractions fraction){
		fractionName = fraction.fractionName;
		sectorFraction = fraction;
		color = fraction.fractionColor;
		AddToLists ();
	}


	void AddToLists(){
		if (sectorFraction.fractionName == "Argon") {
			GenerateGalaxyMap.instance.argon.Add (this);
			amountOfSectors = GenerateGalaxyMap.instance.argon.Count;
		} else if (sectorFraction.fractionName == "Boron") {
			GenerateGalaxyMap.instance.boron.Add (this);
			amountOfSectors = GenerateGalaxyMap.instance.boron.Count;
		} else if (sectorFraction.fractionName == "Teladi") {
			GenerateGalaxyMap.instance.teladi.Add (this);
			amountOfSectors = GenerateGalaxyMap.instance.teladi.Count;
		} else if (sectorFraction.fractionName == "Split") {
			GenerateGalaxyMap.instance.split.Add (this);
			amountOfSectors = GenerateGalaxyMap.instance.split.Count;
		} else {
			GenerateGalaxyMap.instance.free.Add (this);
		}
	}


	public void Spreadfraction(Fractions fraction, float amountOfSectors){
		StartCoroutine (FractionSpreading(fraction, amountOfSectors));
	}


	void GenerateWarpWays(){
		nearbyPlanetsArray = Physics.OverlapSphere (transform.position, radius*1.2f);
		for (int i = 0; i < nearbyPlanetsArray.Length; i++) {
			nearbyPlanets.Add( nearbyPlanetsArray [i].GetComponent<Sector> ());
		}
		for (int np = 0; np < nearbyPlanets.Count; np++) {
			if (nearbyPlanets[np] == this) {
				nearbyPlanets.RemoveAt (np);
			}
		}
		isWarpsGenerated = true;
	}



	public void ValidateMap(){
		isConnectedToMainSystem = true;
		isValidated = true;

		if (isWarpsGenerated == false) {
			GenerateWarpWays ();
		}

		for (int i = 0; i < nearbyPlanets.Count; i++) {
			if (nearbyPlanets [i].isValidated == false) {
				nearbyPlanets [i].ValidateMap ();
			}
		}
	}
}


