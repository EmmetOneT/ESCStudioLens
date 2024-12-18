using UnityEngine;

public class LensPickup : MonoBehaviour
{
    //Variables
    public string lensColour;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
			if (playerInventory != null)
			{
				playerInventory.AddLens(lensColour);
				Destroy(gameObject);
			}
		}
	}
}
