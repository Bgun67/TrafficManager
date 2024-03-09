using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaypointRing : MonoBehaviour
{
	public UnityAction onComplete;

	// Start is called before the first frame update
	public void OnTriggerEnter(Collider other)
    {
		onComplete?.Invoke();
	}
}
