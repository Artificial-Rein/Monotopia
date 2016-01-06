using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class CustomBuildScript : EditorWindow
{
	int version = 0,
		subversion = 0,
		build = 0;
	string save_directory = "";
	string zip_file = "";

	bool display_build_buttons = true, display_directory = true,
		display_scene_list = true, building = false;

	void OnGUI()
	{
		GUILayout.Label("Current version: " + version + "." + subversion + "." + build);

		EditorGUILayout.Space ();

		display_directory =
			EditorGUILayout.Foldout (display_directory, "Build Controls");
		if (display_directory)
		{
			GUILayout.Label("Builds Directory: ");
			GUILayout.TextArea(save_directory);

			if (zip_file.Equals("") && GUILayout.Button ("Select 7za.exe"))
			{
				zip_file = EditorUtility.OpenFilePanel(
					"7za",
					save_directory,
					"exe");

				string[] sevza = zip_file.Split('/', '\\', '.');
				if (!sevza[sevza.Length - 2].Equals("7za"))
				{
					UnityEngine.Debug.LogError("That's not 7za.exe you liar.");
					zip_file = "";
				}
			}

			if (GUILayout.Button("Set Builds Directory"))
			{
				save_directory = EditorUtility.OpenFolderPanel(
					"Select Builds Directory",
					save_directory,
					"Builds");

				using (StreamReader sr = new StreamReader(save_directory + "/cloudver.data"))
				{
					string fl = sr.ReadLine();
					string[] vers = fl.Split('.');
					version = int.Parse(vers[0]);
					subversion = int.Parse (vers[1]);
					build = int.Parse(vers[2]);
					
					sr.Close();
				}
			}
		}

		EditorGUILayout.Space ();

		if (building)
			GUILayout.Label ("Building players...");
		else if (version == 0 && subversion == 0 && build == 0)
			GUILayout.Label ("Could not find cloudver.data");
		else
		{
			display_build_buttons =
				EditorGUILayout.Foldout (display_build_buttons, "Build Buttons");
			if (display_build_buttons)
			{
				if (GUILayout.Button("Compile new Build"))
				{
					build++;
					CustomBuild();
				}
				if (GUILayout.Button("Compile new Subversion"))
				{
					subversion++;
					build = 0;
					CustomBuild();
				}
				if (GUILayout.Button("Compile new Version"))
				{
					version++;
					subversion = 0;
					build = 0;
					CustomBuild();
				}
			}
		}
	}

	private void CustomBuild()
	{
		building = true;

		// Get levels
		string[] levels = Directory.GetFiles(Application.dataPath + "/Engine/Scenes");
		string first = levels[0];
		int num_levels = 1;
		levels[0] = "scene_load";
		for (int i = 1; i < levels.Length; i++)
		{
			string[] li = levels[i].Split('.', '/', '\\');
			if (li[li.Length - 1].Equals("unity"))
			{
				num_levels++;

				if (li[li.Length - 2].Equals("scene_load"))
				{
					levels[i] = first;
					li = levels[i].Split('.', '/', '\\');

					if (!li[li.Length - 1].Equals("unity"))
						num_levels--;
				}

				levels[i] = li[li.Length - 2];
			}
		}

		string[] levels_upd = new string[num_levels];
		num_levels = 0;
		for (int i = 0; i < levels.Length; i++)
		{
			if (!levels[i].Contains("."))
			{
				levels_upd[num_levels++] = "Assets/Engine/Scenes/" + levels[i] + ".unity";
				UnityEngine.Debug.Log(num_levels + ": " + levels_upd[num_levels - 1]);
			}
		}

		// Write cloudver.data
		using (StreamWriter sw = new StreamWriter(save_directory + "/cloudver.data", false))
		{
			sw.WriteLine(version + "." + subversion + "." + build);
			sw.WriteLine();

			sw.Close();
		}

		string version_str = version + "." + subversion + "." + build;
		// Windows build
		Directory.CreateDirectory(save_directory + "/" + version_str);
		BuildPipeline.BuildPlayer(levels_upd, 
		                          save_directory + "/" + version_str + "/" +
		                          version_str + ".exe",
		                          BuildTarget.StandaloneWindows, BuildOptions.None);
		// Write version data
		using (StreamWriter sw = new StreamWriter(save_directory + "/" + version_str + "/" + "v.data", false))
		{
			sw.WriteLine(version + "." + subversion + "." + build);
			sw.WriteLine();
			sw.Close();
		}
		//Compress
		ProcessStartInfo p = new ProcessStartInfo();
		p.FileName = zip_file;
		p.Arguments = "a -tzip -mx9 " 
			+ save_directory + "/zips/" + version_str + ".zip "
			+ save_directory + "/" + version_str + "/";
		p.WindowStyle = ProcessWindowStyle.Normal;
		Process compress_windows = Process.Start(p);
		
		// OSX build
		Directory.CreateDirectory(save_directory + "/" + version_str + "m");
		BuildPipeline.BuildPlayer(levels_upd, 
		                          save_directory + "/" + version_str + "m/" +
		                          version_str + "m.app",
		                          BuildTarget.StandaloneOSXIntel, BuildOptions.None);
		// Write version data
		using (StreamWriter sw = new StreamWriter(save_directory + "/" + version_str + "m/" + "v.data", false))
		{
			sw.WriteLine(version + "." + subversion + "." + build);
			sw.WriteLine();
			sw.Close();
		}
		// Compress
		ProcessStartInfo p_mac = new ProcessStartInfo();
		p_mac.FileName = zip_file;
		p_mac.Arguments = "a -tzip -mx9 " 
			+ save_directory + "/zips/" + version_str + "m.zip "
				+ save_directory + "/" + version_str + "m/";
		p_mac.WindowStyle = ProcessWindowStyle.Normal;
		Process compress_mac = Process.Start(p_mac);

		// Linux build
		Directory.CreateDirectory(save_directory + "/" + version_str + "l");
		BuildPipeline.BuildPlayer(levels_upd, 
		                          save_directory + "/" + version_str + "l/" +
		                          version_str + "l.x86",
		                          BuildTarget.StandaloneLinux, BuildOptions.None);
		// Write version data
		using (StreamWriter sw = new StreamWriter(save_directory + "/" + version_str + "l/" + "v.data", false))
		{
			sw.WriteLine(version + "." + subversion + "." + build);
			sw.WriteLine();
			sw.Close();
		}
		// Compress
		ProcessStartInfo p_linux = new ProcessStartInfo();
		p_linux.FileName = zip_file;
		p_linux.Arguments = "a -ttar -mx9 " 
			+ save_directory + "/zips/" + version_str + "l.tar "
				+ save_directory + "/" + version_str + "l/";
		p_linux.WindowStyle = ProcessWindowStyle.Normal;
		Process compress_linux = Process.Start(p_linux);

		// Webplayer build
		Directory.CreateDirectory(save_directory + "/" + version_str + "w/monotopia");
		BuildPipeline.BuildPlayer(levels_upd, 
		                          save_directory + "/" + version_str + "w/monotopia",
		                          BuildTarget.WebPlayer, BuildOptions.None);

		compress_linux.WaitForExit();

		ProcessStartInfo p_linux2 = new ProcessStartInfo();
		p_linux2.FileName = zip_file;
		p_linux2.Arguments = "a -tgzip -mx9 " 
			+ save_directory + "/zips/" + version_str + "l.tar.gz "
				+ save_directory + "/zips/" + version_str + "l.tar";
		p_linux2.WindowStyle = ProcessWindowStyle.Normal;
		Process compress_linux2 = Process.Start(p_linux2);

		compress_windows.WaitForExit();
		compress_mac.WaitForExit();
		compress_linux2.WaitForExit();

		File.Delete(save_directory + "/zips/" + version_str + "l.tar");

		building = false;
	}

	void InitializeLocals()
	{

	}

	[MenuItem("Window/Builds")]
	static void Init()
	{
		EditorWindow window = EditorWindow.GetWindow(typeof(CustomBuildScript));
		window.Show();
		(window as CustomBuildScript).InitializeLocals();
	}
}
