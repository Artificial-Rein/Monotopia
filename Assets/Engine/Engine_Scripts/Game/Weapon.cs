using UnityEngine;
using System.Collections;

public class Weapon : ShidouGameObject
{
	public Chassis owner;
	// Displayed in hangar
	public Texture icon;
	public string weapon_name;
	public string description;
	
	public Bullet bullet;
	
	// This is the model to show the weapon
	protected WeaponModel model;
	protected bool class_1;
	public WeaponModel fab_class_1, fab_class_2;
	
	public delegate void FireEventHandler(Weapon weapon);
	public event FireEventHandler _Fire;
	
	public delegate void CooldownEventHandler(Weapon weapon);
	public event CooldownEventHandler _CooledDown;
	
	public delegate void AmmoEventHandler(int ammo, Weapon weapon);
	public event AmmoEventHandler _AmmoRegained;
	
	public float sec_to_cooldown;
	public int max_ammo;
	public float sec_to_regain_one_ammo;
	
	public float maximum_delta_degrees = 0f;
	
	public bool async_ammo = true;
	public float cooldown;
	public int ammo;
	public float ammo_cooldown;
	
	protected static void ReloadAllAmmoAtOnce(int ammo, Weapon weapon)
	{
		weapon.ammo = weapon.max_ammo;
	}
	
	protected virtual void Start()
	{
		class_1 = false;
		SetClass(true);
		
		ammo = max_ammo;
		
		if (async_ammo)
			ammo_cooldown = sec_to_regain_one_ammo;
	}
	
	protected override void _Update ()
	{
		if (!async_ammo)
		{
			if (ammo < max_ammo)
			{
				ammo_cooldown -= Time.deltaTime;
				if (ammo_cooldown <= 0f)
				{
					ammo++;
					ammo_cooldown += sec_to_regain_one_ammo;
					if (_AmmoRegained != null)
						_AmmoRegained(ammo, this);
				}
			}
			else ammo_cooldown = sec_to_regain_one_ammo;
		}
		
		if (cooldown > 0f)
		{
			cooldown -= Time.deltaTime;
			if (cooldown <= 0f && _CooledDown != null)
				_CooledDown(this);
		}
	}
	
	protected IEnumerator RechargeAmmo()
	{
		float acd = 0f;
		
		while (acd < sec_to_regain_one_ammo)
		{
			yield return null;
			if (!Utilities.paused)
			{
				acd += Time.deltaTime;
				if (ammo_cooldown < acd)
					ammo_cooldown = acd;
			}
		}
		
		ammo++;
		if (_AmmoRegained != null)
			_AmmoRegained(ammo, this);
	}
	
	protected virtual Bullet InstantiateBullet()
	{
		// Istantiate bullet and send it flying
		GameObject b = (GameObject)Instantiate(bullet.gameObject);
		b.transform.position = model.barrel.position;
		b.transform.rotation = model.barrel.rotation;
		if (maximum_delta_degrees > 0f)
			b.transform.Rotate(Vector3.up, Random.Range(-maximum_delta_degrees, maximum_delta_degrees));
		
		Fight.f.NewBullet(b.GetComponent<Bullet>());
		
		return b.GetComponent<Bullet>();
	}
	
	public virtual void Fire()
	{
		if (cooldown <= 0f && ammo > 0)
		{
			ammo--;
			cooldown = sec_to_cooldown;
			
			InstantiateBullet();
			if (_Fire != null)
				_Fire(this);
			
			if (async_ammo)
			{
				ammo_cooldown = 0f;
				StartCoroutine(RechargeAmmo());
			}
		}
	}
	
	public virtual void ReleaseFire()
	{
		
	}
	
	public virtual void BeginFire()
	{
		
	}
	
	public void SetClass(bool class_1)
	{
		if (this.class_1 == class_1)
			return;
		
		this.class_1 = class_1;
		
		if (model != null)
		{
			model.owner = null;
			Destroy(model);
		}
		
		GameObject m = (GameObject)Instantiate(
			class_1 ? fab_class_1.gameObject :
			fab_class_2.gameObject);
		model = m.GetComponent<WeaponModel>();
		model.owner = this;
		model.transform.parent = transform;
	}

	public virtual void Reset()
	{
		if (!async_ammo)
		{
			if (_AmmoRegained != null)
				_AmmoRegained(max_ammo, this);
			ammo = max_ammo;
			ammo_cooldown = 0f;
		}
		cooldown = 0f;
		if (_CooledDown != null)
			_CooledDown(this);
	}
}
