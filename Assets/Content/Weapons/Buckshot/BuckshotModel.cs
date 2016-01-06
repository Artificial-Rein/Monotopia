using UnityEngine;
using System.Collections;

public class BuckshotModel : WeaponModel
{
	public GameObject[] bullets;
	protected float[] cooldowns;
	protected Vector3[] orig_pos;
	protected int current_shot;
	public float cooldown = 4f;
	
	public Texture2D aimer_atlas;
	public int atlas_textures;
	public int atlas_width;
	protected float atlasw, atlash;
	protected int texh, texw;
	
	protected override void _Start ()
	{
		base._Start ();
		
		atlasw = 1f / (float)atlas_width;
		atlash = 1f / (float)(atlas_textures / atlas_width);
		
		texh = Mathf.FloorToInt(aimer_atlas.height * atlash);
		texw = Mathf.FloorToInt(aimer_atlas.width * atlasw);
		
		owner._Fire += Fire;
		
		cooldowns = new float[bullets.Length];
		orig_pos = new Vector3[bullets.Length];
		for (int i = 0; i < bullets.Length; i++)
		{
			cooldowns[i] = 0f;
			orig_pos[i] = bullets[i].transform.localPosition;
		}
		
		current_shot = 0;
		barrel = bullets[current_shot].transform;
	}
	
	protected virtual void Fire (Weapon w)
	{
		cooldowns[current_shot] = cooldown;
		
		while (current_shot < cooldowns.Length && cooldowns[current_shot] > 0f)
			current_shot++;
		
		if (current_shot < bullets.Length)
			barrel = bullets[current_shot].transform;
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		for (int i = 0; i < cooldowns.Length; i++)
		{
			if (cooldowns[i] > 0f)
			{
				cooldowns[i] -= Time.deltaTime;
				if (cooldowns[i] <= 0f && i < current_shot)
				{
					current_shot = i;
					barrel = bullets[current_shot].transform;
				}
			}
			
			bullets[i].transform.localPosition = orig_pos[i] + new Vector3(0f, 0f, cooldowns[i] * 0.5f);
		}
	}
	
	protected override void _OnGUI ()
	{
		base._OnGUI ();
		
		if (Fight.f != null && Fight.f.fight_active)
		{
			for (int i = 0; i < bullets.Length; i++)
			{
				Rect pos = new Rect(Screen.width * 0.5f - texw * (0.5f - i%2 - (i%2 - 0.5f) * (i / 2)), texh + texh * (i / 2), texh, texw);
				int tex_id = Mathf.Clamp(Mathf.FloorToInt( cooldowns[i] / cooldown * atlas_textures), 0, atlas_textures);
				GUI.DrawTextureWithTexCoords(pos, aimer_atlas, new Rect(atlasw * (tex_id % atlas_width), atlash * (tex_id / atlas_width), atlasw, atlash));
			}
		}
	}
}
