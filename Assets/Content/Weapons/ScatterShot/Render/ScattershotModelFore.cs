using UnityEngine;
using System.Collections;

public class ScattershotModelFore : WeaponModel
{
	public Transform left, right;
	protected bool leftbarrel;
	
	public Texture2D reload_atlas;
	public int reload_textures;
	public int reload_width;
	protected float reloadw, reloadh;
	protected int reloadtexh, reloadtexw;
	
	public Texture2D narrow_atlas;
	public int narrow_textures;
	public int narrow_width;
	protected float narroww, narrowh;
	protected int narrowtexh, narrowtexw;
	
	protected override void _Start ()
	{
		base._Start ();
		
		reloadw = 1f / (float)reload_width;
		reloadh = 1f / (float)(reload_textures / reload_width);
		
		reloadtexh = Mathf.FloorToInt(reload_atlas.height * reloadh);
		reloadtexw = Mathf.FloorToInt(reload_atlas.width * reloadw);
		
		narroww = 1f / (float)narrow_width;
		narrowh = 1f / (float)(narrow_textures / narrow_width);
		
		narrowtexh = Mathf.FloorToInt(narrow_atlas.height * narrowh);
		narrowtexw = Mathf.FloorToInt(narrow_atlas.width * narroww);
		
		barrel = left;
		owner._Fire += FireEvent;
	}
	
	protected override void _Update ()
	{
		base._Update ();
		
		ScatterShotWeapon s = owner as ScatterShotWeapon;
		if (s != null)
		{
			anim.SetBool("charging", s.charge > 0f);
			if (s.charge > 0f)
				anim.speed = 0.55f * s.charge_per_second / s.max_charge;
			else anim.speed = 1.05f * s.sec_to_regain_one_ammo;
		}
	}
	
	protected virtual void FireEvent(Weapon w)
	{
		barrel = leftbarrel ? left : right;
		leftbarrel = !leftbarrel;
	}
	
	protected override void _OnGUI ()
	{
		base._OnGUI ();
		
		ScatterShotWeapon g = owner as ScatterShotWeapon;
		if (g != null)
		{
			if (Fight.f != null && Fight.f.fight_active)
			{
				int tex_id = Mathf.Clamp(Mathf.FloorToInt( (1f - g.ammo_cooldown / g.sec_to_regain_one_ammo) * reload_textures), 0, reload_textures - 1);
				GUI.DrawTextureWithTexCoords(new Rect(Screen.width * 0.5f - reloadtexw * 0.5f, reloadtexh, reloadtexw, reloadtexh), 
					reload_atlas, 
					new Rect(reloadw * (tex_id % reload_width), reloadh * (tex_id / reload_width), reloadw, reloadh));
				
				if (g.ammo > 0)
				{
					tex_id = Mathf.Clamp(Mathf.FloorToInt( (1f - g.charge / g.max_charge) * narrow_textures), 0, narrow_textures - 1);
					GUI.DrawTextureWithTexCoords(new Rect(Screen.width * 0.5f - narrowtexw * 0.5f, narrowtexh, narrowtexw, narrowtexh), 
						narrow_atlas, 
						new Rect(narroww * (tex_id % narrow_width), narrowh * (tex_id / narrow_width), narroww, narrowh));
				}
			}
		}
	}
}
