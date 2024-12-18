using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class PlayerInventory : MonoBehaviour
{
    //Variables
    public List<string> inventory = new List<string>();
    private int currentLensIndex = 0;

    public Material[] lensMaterials;
    public Renderer glassesRenderer;

    //Add lens to the inventory
    public void AddLens(string lensColour)
    {
        if (!inventory.Contains(lensColour)) //Prevent duplicates for now
        {
            inventory.Add(lensColour);
        }
    }

	//Equip lens from inventory
	public void EquipLens(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            currentLensIndex = index;
            Debug.Log("Equipped: " + inventory[currentLensIndex]);

            //Update the glasses renderer with the new lens material
            if (glassesRenderer != null && lensMaterials.Length > index)
            { 
                glassesRenderer.material = lensMaterials[index]; //Change material based on lens
            }
        }
    }

	//Cycle through lenses with "E" key is pressed
	private void Update()
	{
        if (inventory.Count > 0) //Only allow lens swapping if the inventory is not empty
        {
            if (Input.GetKeyDown(KeyCode.E))
            { 
                currentLensIndex = (currentLensIndex + 1) % inventory.Count; //Loop through inventory
                EquipLens(currentLensIndex);
            }
        }
	}
}
