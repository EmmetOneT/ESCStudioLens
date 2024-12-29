using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
	// Variables
	public List<string> inventory = new List<string>();
	private int currentLensIndex = 0;

	public Material[] lensMaterials;
	public Renderer glassesRenderer;

	// Cached references for performance
	private Dictionary<string, List<GameObject>> lensObjects;

	private void Start()
	{
		// Cache objects at the start for performance
		lensObjects = new Dictionary<string, List<GameObject>>
		{
			{ "RedLens", new List<GameObject>(GameObject.FindGameObjectsWithTag("RedLens")) },
			{ "GreenLens", new List<GameObject>(GameObject.FindGameObjectsWithTag("GreenLens")) },
			{ "BlueLens", new List<GameObject>(GameObject.FindGameObjectsWithTag("BlueLens")) }
		};
	}

	// Add lens to the inventory
	public void AddLens(string lensColour)
	{
		if (!inventory.Contains(lensColour)) // Prevent duplicates for now
		{
			inventory.Add(lensColour);
		}
	}

	// Equip lens from inventory
	public void EquipLens(int index)
	{
		if (index >= 0 && index < inventory.Count)
		{
			currentLensIndex = index;
			Debug.Log("Equipped: " + inventory[currentLensIndex]);

			// Update the glasses renderer with the new lens material
			if (glassesRenderer != null && lensMaterials.Length > index)
			{
				glassesRenderer.material = lensMaterials[index]; // Change material based on lens
			}

			// Update object visibility
			UpdateObjectVisibility();
		}
	}

	// Cycle through lenses with "E" key
	private void Update()
	{
		if (inventory.Count > 0) // Only allow lens swapping if the inventory is not empty
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				currentLensIndex = (currentLensIndex + 1) % inventory.Count; // Loop through inventory
				EquipLens(currentLensIndex);
			}
		}
	}

	// Manage object visibility based on the equipped lens
	private void UpdateObjectVisibility()
	{
		// Get the currently equipped lens
		string equippedLens = inventory[currentLensIndex];

		// Update visibility for all cached objects
		foreach (var pair in lensObjects)
		{
			bool isVisible = pair.Key == $"{equippedLens}Lens"; // Check if the lens matches the object tag

			foreach (GameObject obj in pair.Value)
			{
				obj.SetActive(isVisible); // Show or hide object
			}
		}
	}
}
