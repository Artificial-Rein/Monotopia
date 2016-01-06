using UnityEngine;
using System.Collections;

public class MapBlock : MonoBehaviour
{
	// Which sector is this in
	public Sector sector;
	// The render for walls gameobject
	public GameObject walls;
	// The render object for the buildings
	public Renderer[] bldgs;
	public Renderer[] floor;
	
	public Material[] building_mats;
	public Material[] quadwall_mats;
	public Material[] quadwall_mats_selected;
	public Material[] floor_mats;
	
	public Animator animator;

	public void Recolor()
	{
		foreach (Renderer r in bldgs)
			r.material = building_mats[(int)sector.color];
		foreach (Renderer r in floor)
			r.material = floor_mats[Mathf.Clamp((int)sector.color, 0, floor_mats.Length - 1)];
		
		if (walls != null)
		{
			bool active = walls.activeSelf;
			walls.SetActive(true);
			
			foreach (Renderer r in walls.GetComponentsInChildren<Renderer>())
				if (sector.Selected)
				{
					r.material = quadwall_mats_selected[(int)sector.color - 1];
				}
				else
				{
					r.material = quadwall_mats[(int)sector.color];
				}
			
			walls.SetActive(active);
		}
	}
	
	public Sector.SectorType SectorType
	{
		get
		{
			if (sector == null || sector.sector_type == 0)
				return Sector.SectorType.None;
			else
				return sector.sector_type;
		}
	}
	
	public void Rise(bool up)
	{
		if (animator != null)
			animator.SetBool("up", up);
		
		if (up && !walls.activeSelf)
			StartCoroutine(RiseWallFlicker());
		else if (!up) walls.SetActive(false);
	}
	
	const int flickers = 2;
	const float on_dur = 0.1f, off_dur = 0.1f, total_wait = 1.95f, initial_wait = 1f;
	IEnumerator RiseWallFlicker()
	{
		float wait = initial_wait * Random.Range(1f, 1.3f);
		yield return new WaitForSeconds(wait);
		
		for (int i = 0; i < flickers; i++)
		{
			walls.SetActive(true);
			float amt = Random.Range(0.6f, 1.4f) * on_dur;
			wait += amt;
			yield return new WaitForSeconds(amt);
			walls.SetActive(false);
			amt = Random.Range(0.6f, 1.4f) * off_dur;
			wait += amt;
			yield return new WaitForSeconds(amt);
		}
		
		yield return new WaitForSeconds(total_wait - wait);
		
		walls.SetActive(true);
	}
}
