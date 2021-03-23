using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGalaxyMap : MonoBehaviour {

	public int radius;
	public GameObject prefab;
	public int distanceScale;
	public bool stopGeneration;
	public static GenerateGalaxyMap instance;

	public int allGeneratedSector;

	[HideInInspector]public List<Sector> argon;
	[HideInInspector]public List<Sector> boron;
	[HideInInspector]public List<Sector> split;
	[HideInInspector]public List<Sector> teladi;
	[HideInInspector]public List<Sector> free;

	public List<FractionSectors> fractionSectors;

	[HideInInspector]public List<Sector> allSectors;
	public List<SectorAndDistance> sectorDistance;
	[HideInInspector]public List<SectorAndDistance> xCoord;
	[HideInInspector]public List<SectorAndDistance> yCoord;
	[HideInInspector]public List<SectorAndDistance> zCoord;
	public GameObject galaxyMap;

	bool delay = true;

	public List<Fractions> fractions;
	[HideInInspector]public List<SectorAndDistance> availableSectors;


	[Range(0,1)]
	public float maxRadius;

	[Range(1,30)]
	public float percentage;


	void Awake(){
		if (instance != null) {
			Debug.LogWarning ("More than one generator instance found!");
			return;
		}
		instance = this;
	}


	void Start () {
		galaxyMap = new GameObject ();
		galaxyMap.name = "Galaxy Map";
		GenerateMap ();
	}



	void GenerateMap (){
		
		int maximumValue = Mathf.Abs (radius * radius + 2 * radius);
		for (int x = -radius; x < radius; x++) {
			for (int y = radius; y > -radius; y--) {
				for (int z = -radius; z < radius; z++) {
					int cellValue = Mathf.Abs (x * y * z + x + y + z);
					int randomValue = Random.Range (0, maximumValue);
					if (Mathf.Abs (x) < radius * maxRadius) {
						if (Mathf.Abs (y) < radius * maxRadius) {
							if (Mathf.Abs (z) < radius * maxRadius) {
								if (randomValue / percentage >= cellValue) {
									Instantiate (prefab, new Vector3 (x * distanceScale, z * distanceScale, y * distanceScale), Quaternion.identity);
								}
							}
						}
					}
				}
			}
		}
		stopGeneration = true;
		if (stopGeneration == true) {
			DistanceBetweenPoints ();
		}
		AddMainSectors ();
		StartCoroutine (Delay ());
	}

	void AddMainSectors(){
		sectorDistance [0].sector.connectionRadius = sectorDistance [0].sector.radius * 1.2f;
		if (sectorDistance [0].sector.nearbyPlanets.Count < 1) {
			sectorDistance [0].sector.connectionRadius = sectorDistance [0].sector.radius * 1.5f;
		}
		sectorDistance [0].sector.ValidateMap ();
	}

	void GenerateAvailableSectors(){
		availableSectors.Add (xCoord [0]);
		availableSectors.Add (xCoord [xCoord.Count - 1]);
		availableSectors.Add (yCoord [0]);
		availableSectors.Add (yCoord [yCoord.Count - 1]);
		availableSectors.Add (zCoord [0]);
		availableSectors.Add (zCoord [zCoord.Count - 1]);
	}

	void GenerateFractions (){
		GenerateAvailableSectors ();
		for (int i = 0; i < fractions.Count; i++) {
			int randomSector = Random.Range (0, availableSectors.Count);
			availableSectors [randomSector].sector.fractionName = fractions [i].fractionName;
			availableSectors [randomSector].sector.color = fractions [i].fractionColor;
			fractions [i].sector = availableSectors [randomSector].sector;
			availableSectors.RemoveAt (randomSector);
		}
		for (int i = 0; i < fractions.Count; i++) {
			fractions [i].sector.Spreadfraction (fractions [i], 0);
		}
	}

	void AddFreeSectorsToList(){
		for (int i = 0; i < sectorDistance.Count; i++) {
			if (sectorDistance[i].sector.fractionName == "") {
				free.Add (sectorDistance [i].sector);
			}
		}
		ResortList ();
		for (int i = fractionSectors [0].fractionSectors; i < fractionSectors [2].fractionSectors; i++) {
			if (free.Count != 0) {
				free [0].AssignSectorToFraction (fractionSectors [0].fraction);
				free.RemoveAt (0);
			}
		}

		ResortList ();

		for (int i = fractionSectors [0].fractionSectors; i < fractionSectors [2].fractionSectors; i++) {
			if (free.Count != 0) {
				free [0].AssignSectorToFraction (fractionSectors [0].fraction);
				free.RemoveAt (0);
			}
		}
	
		ResortList ();

		for (int i = 0; i < free.Count; i++) {
			free [i].fractionName = "Unpopulated";
			free [i].color = Color.grey;
		}
	}

	void ResortList(){
		fractionSectors.Sort (SortByIndex);
		fractionSectors [0].fractionSectors = argon.Count;
		fractionSectors [1].fractionSectors = boron.Count;
		fractionSectors [2].fractionSectors = split.Count;
		fractionSectors [3].fractionSectors = teladi.Count;
		fractionSectors.Sort (SortBySectors);
	}

	void DistanceBetweenPoints(){
		allGeneratedSector = allSectors.Count;
		for (int i = 0; i < allSectors.Count; i++) {
			float distance = Vector3.Distance (Vector3.zero, allSectors [i].transform.position);
			SectorAndDistance sectorStack = new SectorAndDistance();
			sectorStack.sector = allSectors [i];
			sectorStack.distance = distance;
			sectorStack.coords = allSectors [i].transform.position;
			sectorDistance.Add (sectorStack);

		}	
		sectorDistance.Sort (SortByDistance);
	}

	public IEnumerator Delay(){
		yield return new WaitForSeconds (1f);
		RemoveSeparatedSectors ();
		GenerateColors ();
		StartCoroutine (ValidationDelay ());
	}


	public IEnumerator ValidationDelay(){
		yield return new WaitForSeconds (1f);
		ValidateLists ();
		Debug.Log ("Validated");
		StartCoroutine (FractionDelay ());
	}

	public IEnumerator FractionDelay(){
		yield return new WaitForSeconds (1f);
		GenerateFractions ();
		StartCoroutine (FreeToList ());
	}

	public IEnumerator FreeToList(){
		yield return new WaitForSeconds (1f);
		AddFreeSectorsToList ();
		Debug.Log ("Free Added");
	}


	void ValidateLists(){
		for (int i = 0; i < sectorDistance.Count; i++) {
			if (sectorDistance [i].sector == null) {
				sectorDistance.RemoveAt (i);
			} else {
				xCoord.Add (sectorDistance [i]);
				yCoord.Add (sectorDistance [i]);
				zCoord.Add (sectorDistance [i]);
			}

			xCoord.Sort (SortByX);
			yCoord.Sort (SortByY);
			zCoord.Sort (SortByZ);
		}
	}


	void RemoveSeparatedSectors(){
		for (int i = 0; i < allSectors.Count; i++) {
			if (allSectors [i].isConnectedToMainSystem == false) {
				Destroy (allSectors [i].gameObject);
			}
		}
	}

	void GenerateColors(){
		for (int i = 0; i < sectorDistance.Count; i++) {
			sectorDistance [i].sector.distanceToCenter = sectorDistance [i].distance;
			sectorDistance [i].sector.colorChange = sectorDistance [i].distance / sectorDistance [sectorDistance.Count-1].distance;
		}
	}

	static int SortByDistance (SectorAndDistance sector1, SectorAndDistance sector2 ){
		return sector1.distance.CompareTo (sector2.distance);
	}

	static int SortByX (SectorAndDistance sector1, SectorAndDistance sector2 ){
		return sector1.coords.x.CompareTo (sector2.coords.x);
	}

	static int SortByY (SectorAndDistance sector1, SectorAndDistance sector2 ){
		return sector1.coords.y.CompareTo (sector2.coords.y);
	}

	static int SortByZ (SectorAndDistance sector1, SectorAndDistance sector2 ){
		return sector1.coords.z.CompareTo (sector2.coords.z);
	}

	static int SortBySectors (FractionSectors fraction1, FractionSectors fraction2 ){
		return fraction1.fractionSectors.CompareTo (fraction2.fractionSectors);
	}

	static int SortByIndex (FractionSectors fraction1, FractionSectors fraction2 ){
		return fraction1.index.CompareTo (fraction2.index);
	}
}

[System.Serializable]
public struct SectorAndDistance{
	public Sector sector;
	public float distance;
	public Vector3 coords;
}

[System.Serializable]
public class Fractions{
	public string fractionName;
	public Color fractionColor;
	public Sector sector;
}

[System.Serializable]
public class FractionSectors{
	public int index;
	public Fractions fraction;
	public int fractionSectors;
}



