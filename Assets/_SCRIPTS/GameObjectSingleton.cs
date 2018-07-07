using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSingleton : MonoBehaviour {

	private static Dictionary<string,GameObject> instances;

	[SerializeField] private string UniqueIdentifier;

	public GameObjectSingleton()
	{
		/* Initialize the Dictionary if it doesn't yet exist */
		if (instances == null)
		{
			instances = new Dictionary<string,GameObject>();
		}
	}

	private void Awake()
	{
		if (instances == null)
			Debug.Log("Constructor failed(?)");
		/* If a gameObject already exists with this identifier, destroy this duplicate */
		if (instances.ContainsKey(UniqueIdentifier))
		{
			Debug.Log("Destroying duplicate " + this.name);
			DestroyImmediate(this.gameObject);
		}
		/* Otherwise this is the first instance. Add it to the dictionary and mark it to not be destroyed */
		else
		{
			Debug.Log("First instance of " + this.name);
			instances.Add(UniqueIdentifier, this.gameObject);
			DontDestroyOnLoad(this.gameObject);
		}
	}

	public GameObject GetInstance()
	{
		GameObject obj = null;
		if (instances.TryGetValue(UniqueIdentifier, out obj))
			return obj;
		else
			{
				Debug.Log("No instance exists for: " + UniqueIdentifier + " (called from " + this.name + ")");
				return null;
			}
	}
	
	public void RemoveGameObject()
	{
		if (instances.ContainsKey(UniqueIdentifier))
			instances.Remove(UniqueIdentifier);
		Destroy(this.gameObject);
	}

	void OnDestroy()
	{
		GameObject original;
		instances.TryGetValue(UniqueIdentifier, out original);
		if (original == this.gameObject)
			instances.Remove(UniqueIdentifier);
	}
}
