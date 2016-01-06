using UnityEngine;
using System.Collections;

public class Chassis : HittableObject
{
	Animator anim;
	
	public Transform fore_slot,
		turret_slot, module_slot;
	public Weapon fore, turret;
	
	public float max_speed;
	public float accel;
	public int max_shield;
	public float sec_to_recharge_shield;
	public bool sc_class_1, sc_class_2, ability;
	public int stage;
	
	public int shield;
	public float speed;
	float shield_recharge;
	float shield_flash;
	
	public GameObject explosion;
	
	public delegate void UnlockEventHandler(int stage, Chassis chassis);
	public event UnlockEventHandler Unlock;
	
	public Material shield_level_mat;
	
	public static Chassis c;

	public bool alive;
	
	//For achieves
	int num_hits_taken;
	public int NumHitsTaken { get { return num_hits_taken; } }
	
	void Start()
	{
		DontDestroyOnLoad(gameObject);
		
		shield = max_shield;
		speed = 0f;
		shield_recharge = 0f;
		num_hits_taken = 0;
		shield_flash = 0f;
		
		dmgGeneric += TakeDamage;
	}
	
	public void StartGame()
	{
		GarageController.WipeUnlocks();
		alive = true;
		c = this;
		
		_StartGame();
	}
	
	protected virtual void _StartGame()
	{
		GarageController.UnlockWeapon(0);
	}
	
	public void TakeDamage(HittableObject h)
	{
		if (Fight.f == null || !Fight.f.fight_active)
			return;

		shield--;
		num_hits_taken++;
		
		if (shield < 0)
			StartCoroutine(Lose());
	}
	
	IEnumerator Lose()
	{
		Destroy(Fight.f.gameObject);
		
		if (explosion != null)
		{
			GameObject ex = (GameObject)Instantiate(explosion.gameObject);
			ex.transform.position = transform.position;
		}
		transform.position = new Vector3(0f, 10000f, 0f);

		alive = false;

		yield return new WaitForSeconds(2.5f);
		
		GameObject gobj = new GameObject("you lost");
		gobj.AddComponent<YouLose>();

		gameObject.SetActive(false);
	}
	
	public void Equip(Weapon wep, bool fore)
	{
		if (fore) EquipFore(wep);
	}
	
	public void EquipFore(Weapon wep)
	{
		if (wep == fore) return;
		
		if (fore != null)
		{
			fore.owner = null;
			Destroy(fore.gameObject);
		}
		
		if (wep == null) return;
		
		wep.transform.parent = fore_slot;
		wep.transform.localPosition = Vector3.zero;
		wep.transform.localRotation = Quaternion.identity;
		wep.owner = this;
		fore = wep;
	}
	
	public void EquipTurret(Weapon wep)
	{
		if (wep == turret) return;
		
		if (turret != null)
		{
			turret.owner = null;
			Destroy(turret.gameObject);
		}
		
		if (wep == null) return;
		
		wep.transform.parent = turret_slot;
		wep.transform.localPosition = Vector3.zero;
		wep.transform.localRotation = Quaternion.identity;
		wep.owner = this;
		turret = wep;
	}

	const float shield_flash_time = 0.25f;
	protected override void _Update ()
	{
		base._Update();

		shield_flash += Time.deltaTime;
		if (shield_flash > shield_flash_time * 2)
			shield_flash = 0f;
		
		// Shield level update
		if (shield > 0 || shield_flash< shield_flash_time)
			shield_level_mat.SetFloat("_ShieldLevel", 1f * shield / max_shield + (1f * shield_recharge / sec_to_recharge_shield / max_shield));
		else
			shield_level_mat.SetFloat("_ShieldLevel", 0f);
		
		// Recharge shield
		if (shield < max_shield)
		{
			if (shield_recharge < sec_to_recharge_shield)
				shield_recharge += Time.deltaTime;
			else
			{
				shield++;
				shield_recharge = 0f;
			}
		}
		else shield_recharge = 0f;
		
		// Move
		transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
		transform.localRotation = Quaternion.Euler(0f, 0f, speed * -0.25f);
	}
	
	public virtual void Accelerate(float acceleration)
	{
		if (acceleration > accel) acceleration = accel;
		else if (acceleration < -accel) acceleration = -accel;
		
		speed += acceleration * Time.deltaTime;
		
		if (speed > max_speed)
			speed = max_speed;
		else if (speed < -max_speed)
			speed = -max_speed;
	}

	public virtual void Reset()
	{
		shield = max_shield;
		shield_recharge = 0f;

		speed = 0;

		if (turret != null)
			turret.Reset();
		if (fore != null)
			fore.Reset();
	}
}
