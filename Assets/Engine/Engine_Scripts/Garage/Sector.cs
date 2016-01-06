using UnityEngine;
using System.Collections;

public class Sector : MonoBehaviour
{
	public static string[] possible_names = { "Temporary" };
	
	public enum BlockColor : int { Cyan = 0, Red = 1, Green = 2, Blue = 3 }
	
	public MapBlock[] blocks;
	public Sector[] adj_sectors;
	public BlockColor color;
	public string sector_name;
	public int difficulty;
	public int weapon_unlock;

	public Fight fight;

	public int dist_to_tower, dist_to_start, dist_to_center, dist_to_weapon;
	
	bool selected;
	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			Recolor (color);
		}
	}
	
	public enum SectorType : byte { None = 1, Player = 2, Tower = 4, Standard = 8 }
	public SectorType sector_type;
	
	public void Recolor(BlockColor c)
	{
		color = c;
		
		foreach (MapBlock b in blocks)
			b.Recolor();
	}
}
