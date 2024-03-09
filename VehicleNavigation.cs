using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleNavigation : MonoBehaviour
{
	Intersection intersection;
	void Awake()
	{
		intersection = FindObjectOfType<Intersection>();
	}

	// Update is called once per frame
	public void PlotRoute (Intersection intersection)
    {
		
	}

	// Update is called once per frame
    public Road GetNextRoad ()
    {
		foreach (Road road in intersection.connectedRoads)
		{
			if (road != null)
			{
				return road;
			}
		}
		return null;

	}
}
