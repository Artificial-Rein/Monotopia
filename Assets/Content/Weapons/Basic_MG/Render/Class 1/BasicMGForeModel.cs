using UnityEngine;
using System.Collections;

public class BasicMGForeModel : WeaponModel
{
	public Transform artillery, left, right;
	protected bool left_barrel;
	
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
		
		owner._Fire += FireEvent;
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		MachineGun g = owner as MachineGun;
		if (g != null)
		{
			anim.SetBool("armed", g.artillery_shots_left > 0);
			if (g.artillery_shots_left > 0)
				barrel = artillery;
			else
				barrel = left_barrel ? left : right;
		}
	}
	
	protected virtual void FireEvent(Weapon w)
	{
		left_barrel = !left_barrel;
	}
	
	protected override void _OnGUI ()
	{
		base._OnGUI ();
		
		MachineGun g = owner as MachineGun;
		if (g != null)
		{
			if (Fight.f != null && Fight.f.fight_active)
			{
				int tex_id = Mathf.Clamp(Mathf.FloorToInt( g.sec_until_artillery_fire / g.post_mortem_cooldown * atlas_textures), 0, atlas_textures - 1);
				GUI.DrawTextureWithTexCoords(new Rect(Screen.width * 0.5f - texw * 0.5f, texh, texw, texh), 
					aimer_atlas, 
					new Rect(atlasw * (tex_id % atlas_width), atlash * (tex_id / atlas_width), atlasw, atlash));
			}
		}
	}
}
