using UnityEngine;
using System.Collections;
using System.IO;

public static class Settings
{
	public class KeyControl
	{
		public KeyCode primary, secondary;
		public string name;
		
		public KeyControl (KeyCode p, KeyCode s, string n)
		{
			primary = p;
			secondary = s;
			name = n;
		}
		
		public bool Held
		{
			get { return Input.GetKey(primary) || Input.GetKey(secondary); }
		}
		public bool Down
		{
			get { return Input.GetKeyDown(primary) || Input.GetKeyDown(secondary); }
		}
		public bool Up
		{
			get { return Input.GetKeyUp(primary) || Input.GetKeyUp(secondary); }
		}
	}
	
	// The current ID for the settings structure
	public const byte SETTINGS_FILE_GUID = 14;
	
	// Name of the settings file
	public const string SETTINGS_FILE = @"shidou_settings";
	
	// Whether fullscreen is enabled or not
	public static bool fullscreen = true;
	
	public static int quality = 4;
	
	// Whether shadows are displayed or not
	public static bool shadow = true;
	
	// Whether using low quality models or not
	public static bool low_poly = false;
	
	// Whether to display tutorial or not
	public static bool tutorial = true, fight_tutorial = true;
	
	// Whether the camera moves with the mouse or not
	public static bool camera_movements = true;

	// Limit the framerate?
	public static bool limit_framerate = true;

	// Mute the music?
	public static bool mute_music = false;
	
	// Key controls
	public static KeyControl 
		left = new KeyControl(KeyCode.A, KeyCode.LeftArrow, "Move Left"), 
		right = new KeyControl(KeyCode.D, KeyCode.RightArrow, "Move Right"),
		shoot_fore = new KeyControl(KeyCode.Space, KeyCode.Mouse0, "Fire"),
		shoot_turret = new KeyControl(KeyCode.LeftShift, KeyCode.Mouse1, "Fire Turret");
	
	#region LOADING
	// Opens a file and checks the settings format GUID.
	// Runs the GUID function related to the file's GUID.
	// If the GUID is unrecognized, throws an error.
	public static void LoadSettings()
	{
		won_with_which_weapons = new bool[GarageController.unlocked_weapons.Length];

#if UNITY_WEBPLAYER
#else
		if (!File.Exists(SETTINGS_FILE) || Application.isEditor)
		{
			for (int i = 0; i < won_with_which_weapons.Length; i++)
				won_with_which_weapons[i] = false;

			SaveSettings();
			return;
		}
		
		using (BinaryReader br = 
			new BinaryReader(File.OpenRead(SETTINGS_FILE)))
		{
			byte guid = br.ReadByte();
			
			switch (guid)
			{
			case 14:
				LoadSettings_GUID14(br);
				break;
			case 13:
				LoadSettings_GUID13(br);
				break;
			case 12:
				LoadSettings_GUID12(br);
				break;
			case 11:
				LoadSettings_GUID11(br);
				break;
			case 10:
				LoadSettings_GUID10(br);
				break;
			case 9:
				LoadSettings_GUID9(br);
				break;
			case 8:
				LoadSettings_GUID8(br);
				break;
			case 7:
				LoadSettings_GUID7(br);
				break;
			case 6:
				LoadSettings_GUID6(br);
				break;
			case 5:
				LoadSettings_GUID5(br);
				break;
			case 4:
				LoadSettings_GUID4(br);
				break;
			case 3:
				LoadSettings_GUID3(br);
				break;
			case 2:
				LoadSettings_GUID2(br);
				break;
			case 1:
				LoadSettings_GUID1(br);
				break;
			default:
				Utilities.Exception(
					"Unrecognized settings GUID.\n" +
					"This is most likely due to a recalled update.\n" +
					"Navigate to your install directory and delete " + SETTINGS_FILE + " to play.\n" +
					"If you would rather keep your settings (and achievements), you will have to wait for an update.");
				break;
			}
			
			br.Close();
		}
#endif
		ApplyQuality();
	}

#if UNITY_WEBPLAYER
#else
	private static void LoadSettings_GUID14(BinaryReader br)
	{
		fight_tutorial = br.ReadBoolean();
		
		LoadSettings_GUID13(br);
	}

	private static void LoadSettings_GUID13(BinaryReader br)
	{
		short num_weaps = br.ReadInt16();
		for (short i = 0; i < won_with_which_weapons.Length; i++)
		{
			if (i < num_weaps)
				won_with_which_weapons[i] = br.ReadBoolean();
			else
				won_with_which_weapons[i] = false;
		}

		LoadSettings_GUID12(br);
	}

	private static void LoadSettings_GUID12(BinaryReader br)
	{
		mute_music = br.ReadBoolean();

		LoadSettings_GUID11(br);
	}

	private static void LoadSettings_GUID11(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		tutorial = br.ReadBoolean();
		camera_movements = br.ReadBoolean();
		limit_framerate = br.ReadBoolean();
		quality = br.ReadInt32();
		
		unlocked_ships[0] = br.ReadBoolean();
		
		int num_achieves = br.ReadInt32();
		for (int i = 0; i < num_achieves; i++)
			if (br.ReadBoolean())
				Utilities.u.UnlockAchievement(i, false);
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
		shoot_turret.primary= (KeyCode)br.ReadInt32();
		shoot_turret.secondary = (KeyCode)br.ReadInt32();
	}

	private static void LoadSettings_GUID10(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		tutorial = br.ReadBoolean();
		camera_movements = br.ReadBoolean();
		quality = br.ReadInt32();
		
		unlocked_ships[0] = br.ReadBoolean();
		
		int num_achieves = br.ReadInt32();
		for (int i = 0; i < num_achieves; i++)
			if (br.ReadBoolean())
				Utilities.u.UnlockAchievement(i, false);
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
		shoot_turret.primary= (KeyCode)br.ReadInt32();
		shoot_turret.secondary = (KeyCode)br.ReadInt32();
	}
	
	private static void LoadSettings_GUID9(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		tutorial = br.ReadBoolean();
		quality = br.ReadInt32();
		
		unlocked_ships[0] = br.ReadBoolean();
		
		int num_achieves = br.ReadInt32();
		for (int i = 0; i < num_achieves; i++)
			achievements[i].unlocked = br.ReadBoolean();
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
		shoot_turret.primary= (KeyCode)br.ReadInt32();
		shoot_turret.secondary = (KeyCode)br.ReadInt32();
	}
	
	private static void LoadSettings_GUID8(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		tutorial = br.ReadBoolean();
		quality = br.ReadInt32();
		
		unlocked_ships[0] = br.ReadBoolean();
		
		int num_achieves = br.ReadInt32();
		for (int i = 0; i < num_achieves; i++)
			achievements[i].unlocked = br.ReadBoolean();
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
	}
	
	private static void LoadSettings_GUID7(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		quality = br.ReadInt32();
		
		unlocked_ships[0] = br.ReadBoolean();
		
		int num_achieves = br.ReadInt32();
		for (int i = 0; i < num_achieves; i++)
			achievements[i].unlocked = br.ReadBoolean();
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
	}
	
	private static void LoadSettings_GUID6(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		quality = br.ReadInt32();
		
		int num_achieves = br.ReadInt32();
		for (int i = 0; i < num_achieves; i++)
			achievements[i].unlocked = br.ReadBoolean();
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
	}
	
	private static void LoadSettings_GUID5(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		quality = br.ReadInt32();
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
	}
	
	private static void LoadSettings_GUID4(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		shadow = br.ReadBoolean();
		low_poly = br.ReadBoolean();
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
	}
	
	private static void LoadSettings_GUID3(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		shadow = br.ReadBoolean();
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
	}
	
	private static void LoadSettings_GUID2(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
		
		left.primary = (KeyCode)br.ReadInt32();
		left.secondary = (KeyCode)br.ReadInt32();
		right.primary = (KeyCode)br.ReadInt32();
		right.secondary = (KeyCode)br.ReadInt32();
		shoot_fore.primary= (KeyCode)br.ReadInt32();
		shoot_fore.secondary = (KeyCode)br.ReadInt32();
		
		//left.primary = KeyCode.Alpha0;
	}
	
	private static void LoadSettings_GUID1(BinaryReader br)
	{
		fullscreen = br.ReadBoolean();
	}
#endif
	#endregion
	
	#region SAVING
	public static void SaveSettings()
	{
		QualitySettings.SetQualityLevel(quality);

#if UNITY_WEBPLAYER
#else
		using (BinaryWriter bw = 
			new BinaryWriter(
				File.Open(SETTINGS_FILE, FileMode.Create, FileAccess.Write)))
		{
			bw.Write(SETTINGS_FILE_GUID);

			bw.Write(fight_tutorial);

			bw.Write((short)won_with_which_weapons.Length);
			for (int i = 0; i < won_with_which_weapons.Length; i++)
				bw.Write(won_with_which_weapons[i]);

			bw.Write(mute_music);
			bw.Write(fullscreen);
			bw.Write(tutorial);
			bw.Write(camera_movements);
			bw.Write(limit_framerate);
			bw.Write(quality);
			
			bw.Write(unlocked_ships[0]);
			
			bw.Write(achievements.Length);
			for (int i = 0; i < achievements.Length; i++)
				bw.Write(achievements[i].unlocked);
			
			bw.Write((int)left.primary);
			bw.Write((int)left.secondary);
			bw.Write((int)right.primary);
			bw.Write((int)right.secondary);
			bw.Write((int)shoot_fore.primary);
			bw.Write((int)shoot_fore.secondary);
			bw.Write((int)shoot_turret.primary);
			bw.Write((int)shoot_turret.secondary);
			
			bw.Close();
		}
#endif
	}
	#endregion
	
	static void ApplyQuality()
	{
		shadow = false;
		low_poly = true;
		
		QualitySettings.SetQualityLevel(quality);
		
		if (quality > 1) shadow = true;
		if (quality > 0) low_poly = false;
	}
	
	// --- ACHIEVEMENTS ---

	public static bool[] won_with_which_weapons;

	public static bool[] unlocked_ships = { true, false };
	
	public class Achievement
	{
		public bool unlocked;
		public string name;
		public string description;
		public Texture icon;
		public bool display_description_while_locked;
		
		public Achievement(string n, string d, bool ddwl = true)
		{
			unlocked = false;
			name = n;
			description = d;
			display_description_while_locked = ddwl;
			icon = null;
		}
	}
	public static Achievement[] achievements = 
	{
		new Achievement("Power On", "Open the game."),
		new Achievement("Good Morning", "Start a game."),
		new Achievement("Golden Chariot", "Win the game."),
		new Achievement("Buckshot", "Convert half the map to cyan."),
		new Achievement("Railgun", "Destroy at least five enemies with the same bullet."),
		new Achievement("Jack of All Guns", "Win a fight each of the five weapons.")
	};
	public static int GetAchievementIDByName(string name)
	{
		for (int i = 0; i < achievements.Length; i++)
		{
			if (achievements[i].Equals(name))
				return i;
		}
		return -1;
	}
}
