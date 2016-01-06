using UnityEngine;
using System.Collections;

public class RailgunModelFore : WeaponModel
{
	public Texture2D aimer_atlas;
	public int atlas_textures;
	public int atlas_width;
	protected float atlasw, atlash;
	protected int texh, texw;
	
	public int textures_for_init_charge;
	public int textures_per_extra_charge;
	
	protected override void _Start ()
	{
		base._Start ();
		
		atlasw = 1f / (float)atlas_width;
		atlash = 1f / (float)(atlas_textures / atlas_width);
		
		texh = Mathf.FloorToInt(aimer_atlas.height * atlash);
		texw = Mathf.FloorToInt(aimer_atlas.width * atlasw);
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		anim.SetInteger("ammo", owner.ammo);
		
		EOCWeapon e = owner as EOCWeapon;
		if (e != null)
		{
			anim.SetBool("charging", e.sec_charging > 0f && !e.emptying_mag);
		}
	}
	
	protected override void _OnGUI ()
	{
		base._OnGUI ();
		
		EOCWeapon g = owner as EOCWeapon;
		if (g != null)
		{
			if (Fight.f != null && Fight.f.fight_active)
			{
				int tex_id = 0;
				if (g.ammo == 0)
				{
					tex_id = atlas_textures - textures_for_init_charge + 
						Mathf.Clamp(
							Mathf.FloorToInt(
								(textures_for_init_charge - 1) * g.ammo_cooldown / g.sec_to_regain_one_ammo
							),
							0, textures_for_init_charge - 1);
				}
				else
				{
					tex_id = atlas_textures - textures_for_init_charge -
						textures_per_extra_charge * (g.ammo - g.max_ammo) -
						Mathf.Clamp(
							Mathf.FloorToInt(
								(textures_per_extra_charge - 1) * g.sec_charging / g.sec_to_charge_one_ammo
							),
							0, textures_per_extra_charge - 1);
				}
				tex_id = Mathf.Clamp(tex_id, 0, atlas_textures - 1);
				//tex_id = 0;
				GUI.DrawTextureWithTexCoords(new Rect(Screen.width * 0.5f - texw * 0.5f, texh, texw, texh), 
					aimer_atlas, 
					new Rect(atlasw * (tex_id % atlas_width), atlash * (tex_id / atlas_width), atlasw, atlash));
			}
		}
	}
}
