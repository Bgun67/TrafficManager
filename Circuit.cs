using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
	WaypointRing[] rings;
	int endIndex;
	int currentIndex;
	// Start is called before the first frame update
	void Start()
    {
		rings = GetComponentsInChildren<WaypointRing>();
		endIndex = rings.Length;

		int i = 0;
		foreach (WaypointRing ring in rings)
		{
			int temp = i;
			ring.onComplete += () => { PassWaypoint(temp); };
			i++;
		}
	}

    // Update is called once per frame
    public void PassWaypoint(int pointIndex)
    {
		print(pointIndex);
		if (pointIndex == currentIndex)
		{
			currentIndex = currentIndex+1;
			if (currentIndex == endIndex)
			{
				print("YOU WIN");
			}
		}
	}
}
