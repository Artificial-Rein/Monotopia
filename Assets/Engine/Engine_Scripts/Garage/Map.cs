using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour
{
	public static Map m;
	
	public Sector[] sectors;
	public MapBlock[,] blocks;
	
	public GameObject[] tower_fabs;
	public GameObject[] block_fabs;
	public GameObject empty_block_fab;
	
	public GameObject[] quadwalls;
	public Material[] quadwall_mats;
	public Material[] quadwall_selected_mats;
	const int map_width = 18;
	const int num_sectors = 30;
	public const int max_difficulty = 7;
	public int[] spread;

	public int max_dist_to_center, max_dist_to_start, max_dist_to_tower;
	
	Vector3 block_placement_correction = new Vector3(0.5f, 0f, 0.5f);

	public bool[] enemies_encountered;

	bool[] weapons_spawned;

	// Fights
	protected int[][] fight_ids;
	public Fight[] fights;
	public int[] num_fights;
	
	/// <summary>
	/// Gets a random adjacent block allowed with the mask.
	/// </summary>
	/// <param name='mask'>
	/// Binary Or together types which you want allowed.
	/// </param>
	MapBlock GetAdjBlock(MapBlock block, Sector.SectorType mask)
	{
		MapBlock b = null;;
		
		int dir = Random.Range(0,4);
		int num = 0;
		do
		{
			dir++;
			if (dir >= 4) dir = 0;
			
			switch (dir)
			{
			case 0:
				b = WorldToBlock(block.transform.position + new Vector3(1f, 0f, 0f));
				break;
			case 1:
				b = WorldToBlock(block.transform.position + new Vector3(-1f, 0f, 0f));
				break;
			case 2:
				b = WorldToBlock(block.transform.position + new Vector3(0f, 0f, 1f));
				break;
			case 3:
				b = WorldToBlock(block.transform.position + new Vector3(0f, 0f, -1f));
				break;
			}
			
			num++;
		}
		while ((b == null 
			|| (b.SectorType & mask) == 0)
			&& num < 4);
		
		if (b == null 
			|| (b.SectorType & mask) == 0)
			return null;
		
		return b;
	}
	MapBlock WorldToBlock(Vector3 worldpoint)
	{
		worldpoint -= transform.position;
		worldpoint -= block_placement_correction;
		int x, y;
		x = Mathf.RoundToInt(worldpoint.x + map_width / 2f);
		y = Mathf.RoundToInt(worldpoint.z + map_width / 2f);
		
		if (x >= 0 && x < map_width &&
			y >= 0 && y < map_width)
			return blocks[x, y];
		
		return null;
	}
	void AddBlockToSector(Sector s, MapBlock b)
	{
		b.sector = s;
		b.transform.parent = s.transform;
		
		if (s.blocks != null)
		{
			MapBlock[] temp = new MapBlock[s.blocks.Length + 1];
			for (int i = 0; i < s.blocks.Length; i++)
				temp[i] = s.blocks[i];
			temp[s.blocks.Length] = b;
			s.blocks = temp;
		}
		else
		{
			s.blocks = new MapBlock[1];
			s.blocks[0] = b;
		}
	}
	void AddNeighborToSector(Sector main, Sector neighbor)
	{
		if (main == neighbor) return;
		
		if (main.adj_sectors == null)
		{
			main.adj_sectors = new Sector[1];
			main.adj_sectors[0] = neighbor;
		}
		else
		{
			bool redundent = false;
			for (int k = 0; k < main.adj_sectors.Length; k++)
				if (main.adj_sectors[k] == neighbor)
					redundent = true;
			if (!redundent)
			{
				Sector[] temp = new Sector[main.adj_sectors.Length + 1];
				for (int i = 0; i < main.adj_sectors.Length; i++)
					temp[i] = main.adj_sectors[i];
				temp[main.adj_sectors.Length] = neighbor;
				main.adj_sectors = temp;
			}
		}
	}

	int sector_uid = 0;
	Sector NewSector()
	{
		GameObject s = new GameObject("Sector_" + (sector_uid++));
		s.transform.parent = transform;
		s.transform.localPosition = Vector3.zero;
		s.AddComponent(typeof(Sector));
		Sector ret = s.GetComponent<Sector>();
		
		ret.sector_type = Sector.SectorType.Standard;
		ret.color = Sector.BlockColor.Red;
		ret.difficulty = 0;
		ret.weapon_unlock = -1;

		ret.dist_to_center = -1;
		ret.dist_to_start = -1;
		ret.dist_to_tower = -1;
		ret.dist_to_weapon = num_sectors;

		ret.fight = null;

		ret.sector_name = "";
		
		return ret;
	}
	int mapblock_uid = 0;
	MapBlock NewMapBlock() { return NewMapBlock(0, 0); }
	MapBlock NewMapBlock(int x, int y) { return NewMapBlock(x, y, block_fabs[Random.Range(0,block_fabs.Length)]); }
	MapBlock NewMapBlock(int x, int y, GameObject fab)
	{
		GameObject b = (GameObject)Instantiate(fab);
		b.name = "MapBlock_" + x + "_" + y + "_" + (mapblock_uid++);
		MapBlock ret = b.GetComponent<MapBlock>();
		b.transform.parent = transform;
		b.transform.rotation = Quaternion.Euler(0f, 90f * Random.Range(0, 4), 0f);
		b.transform.localPosition = new Vector3(x - map_width / 2f, 0f, y - map_width / 2f)
			+ block_placement_correction;

		return ret;
	}
	
	public static void OpenMap(bool open)
	{
		if (m == null) return;
		m._OpenMap(open);
	}
	void _OpenMap(bool open)
	{
		for (int i = 0; i < sectors.Length; i++)
		{
			if (sectors[i].color != Sector.BlockColor.Cyan)
				continue;
			
			foreach (Sector s in sectors[i].adj_sectors)
				if (s.color != Sector.BlockColor.Cyan)
					foreach (MapBlock b in s.blocks)
					b.Rise(open);
		}
	}

	public void RaiseCyan()
	{
		for (int i = 0; i < sectors.Length; i++)
		{
			if (sectors[i].color != Sector.BlockColor.Cyan)
				continue;

			foreach (MapBlock b in sectors[i].blocks)
				b.Rise(true);
		}
	}

	public void LowerAll()
	{
		foreach (MapBlock b in blocks)
			b.Rise (false);
	}
	
	public void ConvertSector(Sector s)
	{
		s.color = Sector.BlockColor.Cyan;
		_GenerateWalls();
		if (s != sectors[0])
			_GenerateNextTier();
		foreach (MapBlock b in s.blocks)
			b.Rise(true);
	}
	
	void Start()
	{
		// If m already exists, make sure its active
		if (m != null)
		{
			m.gameObject.SetActive(true);
			Destroy(this);
			return;
		}

		int[] fights_per_diff = new int[Map.max_difficulty + 1];
		for (int i = 0; i < Map.max_difficulty + 1; i++)
			fights_per_diff[i] = 0;
		foreach (Fight f in fights)
			fights_per_diff[Mathf.Clamp(f.difficulty, 0, Map.max_difficulty)]++;
		fight_ids = new int[Map.max_difficulty + 1][];
		for (int i = 0; i < Map.max_difficulty + 1; i++)
			fight_ids[i] = new int[fights_per_diff[i]];
		for (int i = 0; i < fights.Length; i++)
			fight_ids[fights[i].difficulty][--fights_per_diff[fights[i].difficulty]] = i;
		num_fights = new int[fights.Length];
		for (int i = 0; i < num_fights.Length; i++)
			num_fights[i] = 0;

		string output_info = "{\n";
		for (int i = 0; i < Map.max_difficulty + 1; i++)
		{
			output_info += "\t{ ";
			for (int j = 0; j < fight_ids[i].Length; j++)
			{
				output_info += fight_ids[i][j];
				if (j < fight_ids[i].Length - 1)
					output_info += ", ";
			}
			output_info += " }\n";
		}
		output_info += "}";
		Debug.Log(output_info);

		// Otherwise, set this to m and generate the map
		DontDestroyOnLoad(this);
		m = this;
		blocks = new MapBlock[map_width, map_width];
		sectors = new Sector[num_sectors + 1 + tower_fabs.Length];
		for (int i = 0; i < sectors.Length; i++)
			sectors[i] = NewSector();
		
		// Create tower blocks
		// Determine player start location
		// Create tower and player start sectors
		_GenerateTowersAndStart();
		
		// Create all blocks
		_GenerateBlocks();
		
		// Add a single random block to each sector
		_SectorFirstBlocks();
		
		// As long as it can, add a block to each sector which is adjacent to another block in that sector
		_SectorExpansion();
		
		// For each block, check what sectors neighbor it. Any sectors that aren't it's
			// own sector get added to it's sector's adj list
		_GenerateNeighborhoods();
		
		// For each block generate the wall object for it
		ConvertSector(sectors[0]);

		// Create distances
		_GenerateDistances();

		// Generate sector difficulties
		_GenerateDifficulties();

		// Generate sector weapon spawn chances
		max_chance_top = 0f;
		foreach (Sector s in sectors)
		{
			s.dist_to_weapon = max_dist_to_start;
			float wsc = GetSpawnWeaponChanceTop(s);
			if (wsc > max_chance_top)
				max_chance_top = wsc;
		}
		foreach (Sector s in sectors)
			Debug.Log(s.name + " has a " + Mathf.Round(GetSpawnWeaponChance(s) * 100) + "% chance of spawning a weapon.",
			          s);

		// Set that no weapons have been spawned
		weapons_spawned = new bool[GarageController.weapons.Length];
		for (int i = 0; i < weapons_spawned.Length; i++)
			weapons_spawned[i] = false;

		// Generate sector fights for all sectors that don't have fights which are adjacent to a cyan sector.
		// Must pick a fight of the correct difficulty
		// Pick applicable fight with lowest number of unencountered things
		// More common fights on the map are rarer to spawn
		_GenerateNextTier();
	}

	float max_chance_top;
	const float max_weapon_chance = 0.9f, min_weapon_chance = 0.1f;
	float GetSpawnWeaponChanceTop(Sector sect)
	{
		for (int i = 0; i < tower_fabs.Length + 1; i++)
			if (sect == sectors[i])
				return 0f;

		return Mathf.Pow (4f, 2f * ((float)sect.dist_to_center) / max_dist_to_center + 
		                  ((float)sect.dist_to_start) / max_dist_to_start + 
						  ((float)sect.dist_to_tower) / max_dist_to_tower);
	}
	float GetSpawnWeaponChance(Sector sect)
	{
		return min_weapon_chance +
			(0.5f + (sect.dist_to_weapon / max_dist_to_start) * 0.5f) *
			(max_weapon_chance - min_weapon_chance) *
			GetSpawnWeaponChanceTop(sect) /
			max_chance_top;
	}

	void _GenerateNextTier()
	{
		foreach (Sector s in sectors)
		{
			if (s.color != Sector.BlockColor.Cyan)
				continue;

			foreach (Sector sect in s.adj_sectors)
			{
				if (sect.fight != null || sect.difficulty < 0 || 
				    fight_ids[sect.difficulty].Length == 0 || sect.color == Sector.BlockColor.Cyan) 
					continue;

				int[] num_new_enemies = new int[fight_ids[sect.difficulty].Length];
				int num_on_lowest = 0;
				int[] lowest_ids = new int[fight_ids[sect.difficulty].Length];
				for (int i = 0; i < num_new_enemies.Length; i++)
				{
					if (enemies_encountered == null)
					{
						enemies_encountered = new bool[fights[fight_ids[sect.difficulty][i]].enemies_in_this_fight.Length];
						for (int j = 0; j < enemies_encountered.Length; j++)
							enemies_encountered[j] = false;
					}
					else if (enemies_encountered.Length < fights[fight_ids[sect.difficulty][i]].enemies_in_this_fight.Length)
					{
						bool[] t = new bool[fights[fight_ids[sect.difficulty][i]].enemies_in_this_fight.Length];
						for (int j = 0; j < t.Length; j++)
						{
							if (j < enemies_encountered.Length)
								t[j] = enemies_encountered[j];
							else t[j] = false;
						}
						enemies_encountered = t;
					}

					num_new_enemies[i] = 0;
					for (int j = 0; j < fights[fight_ids[sect.difficulty][i]].enemies_in_this_fight.Length; j++)
						if (fights[fight_ids[sect.difficulty][i]].enemies_in_this_fight[j] && !enemies_encountered[j])
							num_new_enemies[i]++;

					if ((num_on_lowest == 0 || num_new_enemies[i] < num_on_lowest) && num_new_enemies[i] != 0)
						num_on_lowest = num_new_enemies[i];
				}

				for (int i = 0; i < num_new_enemies.Length; i++)
					if (num_new_enemies[i] == 0)
						num_new_enemies[i] = num_on_lowest;
				num_on_lowest = 0;

				for (int i = 0; i < num_new_enemies.Length; i++)
				{
					if (num_on_lowest > 0 && num_new_enemies[lowest_ids[0]] > num_new_enemies[i])
						num_on_lowest = 0;
					if (num_on_lowest == 0 || num_new_enemies[lowest_ids[0]] == num_new_enemies[i])
						lowest_ids[num_on_lowest++] = i;
				}

				float[] chances = new float[num_on_lowest];
				float total = 0f;
				for (int i = 0; i < chances.Length; i++)
				{
					chances[i] = 1f / Mathf.Pow(2f, num_fights[fight_ids[sect.difficulty][lowest_ids[i]]]);
					total += chances[i];
				}
				total = 1f / total;
				for (int i = 0; i < chances.Length; i++)
				{
					chances[i] *= total;
				}
				float rng = Random.Range(0f, 1f);
				total = chances[0];
				int id = 0;
				while (rng > total)
					total += chances[++id];

				sect.fight = fights[fight_ids[sect.difficulty][lowest_ids[id]]];
				num_fights[fight_ids[sect.difficulty][lowest_ids[id]]]++;
				if (sect.sector_name.Equals(""))
					sect.sector_name = sect.fight.fight_name;

				if (Random.Range(0f, 1f) < GetSpawnWeaponChance(sect))
				{
					int[] spawnable_weapons = new int[weapons_spawned.Length];
					int num_spawnable_weapons = 0;
					for (int i = 0; i < weapons_spawned.Length; i++)
					{
						if (!GarageController.unlocked_weapons[i] &&
						    	GarageController.weapons_can_spawn[i] &&
						    	!weapons_spawned[i])
							spawnable_weapons[num_spawnable_weapons++] = i;
					}

					if (num_spawnable_weapons > 0)
					{
						sect.weapon_unlock = spawnable_weapons[Random.Range(0, num_spawnable_weapons)];
						weapons_spawned[sect.weapon_unlock] = true;
						sect.dist_to_weapon = 0;
						_GenerateDistToWeaponRecurse(sect);
					}
				}
			}
		}
	}

	void _GenerateDifficulties()
	{
		spread = new int[max_difficulty + 1];
		for (int i = 0; i < max_difficulty + 1; i++)
			spread[i] = 0;

		int min_diff = max_difficulty;
		int max_diff = 0;
		foreach (Sector s in sectors)
		{
			if (s == sectors[0]) continue;

			s.difficulty = s.dist_to_start + sectors[0].dist_to_tower * 2 - s.dist_to_tower * 2 - 1;
			if (s.difficulty > max_diff) max_diff = s.difficulty;
			if (s.difficulty < min_diff) min_diff = s.difficulty;
		}

		foreach (Sector s in sectors)
			s.difficulty -= min_diff;
		max_diff -= min_diff;

		float diff_mod = 1.0f * max_difficulty / max_diff;

		foreach (Sector s in sectors)
		{
			s.difficulty = Mathf.RoundToInt(s.difficulty * diff_mod);
			if (s.difficulty >= 0)
				spread[s.difficulty]++;
		}

		float mean = 0f;
		int total = 0, mode = 0, median = 0;
		for (int i = 0; i < spread.Length; i++)
		{
			total += spread[i];
			mean += spread[i] * i;
			if (spread[i] > spread[mode])
				mode = i;
		}
		for (int t = 0; t < total / 2; median++)
			t += spread[median];
		mean /= total;
		// Debug.Log("DIFFICULTY DATA\nMEAN: " + mean + " MEDIAN: " + median + " MODE: " + mode);
	}

	void _GenerateDistances()
	{
		max_dist_to_center = max_dist_to_start = max_dist_to_tower = 0;

		sectors[0].dist_to_start = 0;
		_GenerateDistToStartRecurse(sectors[0]);

		for (int i = 0; i < tower_fabs.Length; i++)
		{
			sectors[i + 1].dist_to_tower = 0;
			_GenerateDistToTowerRecurse(sectors[i + 1]);
		}

		blocks[map_width / 2, map_width / 2].sector.dist_to_center = 0;
		_GenerateDistToCenterRecurse(blocks[map_width / 2, map_width / 2].sector);

		foreach (Sector s in sectors)
		{
			if (s.dist_to_center > max_dist_to_center)
				max_dist_to_center = s.dist_to_center;
			if (s.dist_to_start > max_dist_to_start)
				max_dist_to_start = s.dist_to_start;
			if (s.dist_to_tower > max_dist_to_tower)
				max_dist_to_tower = s.dist_to_tower;
		}
	}

	void _GenerateDistToWeaponRecurse(Sector sect)
	{
		foreach (Sector s in sect.adj_sectors)
			if (s.dist_to_weapon > sect.dist_to_weapon + 1)
		{
			s.dist_to_weapon = sect.dist_to_weapon + 1;
			_GenerateDistToWeaponRecurse(s);
		}
	}

	void _GenerateDistToCenterRecurse(Sector sect)
	{
		foreach (Sector s in sect.adj_sectors)
			if (s.dist_to_center > sect.dist_to_center + 1 || s.dist_to_center == -1)
		{
			s.dist_to_center = sect.dist_to_center + 1;
			_GenerateDistToCenterRecurse(s);
		}
	}

	void _GenerateDistToStartRecurse(Sector sect)
	{
		foreach (Sector s in sect.adj_sectors)
			if (s.dist_to_start > sect.dist_to_start + 1 || s.dist_to_start == -1)
		{
			s.dist_to_start = sect.dist_to_start + 1;
			_GenerateDistToStartRecurse(s);
		}
	}

	void _GenerateDistToTowerRecurse(Sector sect)
	{
		foreach (Sector s in sect.adj_sectors)
			if (s.dist_to_tower > sect.dist_to_tower + 1 || s.dist_to_tower == -1)
		{
			s.dist_to_tower = sect.dist_to_tower + 1;
			_GenerateDistToTowerRecurse(s);
		}
	}

	void _GenerateTowersAndStart()
	{
		bool[] corners_filled = { false, false, false, false };
		
		// Tower
		for (int i = 0; i < tower_fabs.Length; i++)
		{
			// Make sector
			sectors[i + 1].color = (Sector.BlockColor)(i + 1);
			if (i == 0)
				sectors[i + 1].sector_name = "Red Tower";
			else if (i == 1)
				sectors[i + 1].sector_name = "Green Tower";
			else if (i == 2)
				sectors[i + 1].sector_name = "Blue Tower";
			sectors[i + 1].sector_type = Sector.SectorType.Tower;
			
			// Find corner
			int tower_corner = Random.Range(0, 4);
			for (int j = 0; j < 4 && corners_filled[tower_corner]; j++)
			{
				tower_corner++;
				if (tower_corner >= 4)
					tower_corner = 0;
			}
			
			corners_filled[tower_corner] = true;
			int x = tower_corner % 2 == 0 ? 1 : map_width - 2;
			int y = tower_corner / 2 == 0 ? 1 : map_width - 2;
			
			for (int xm = -1; xm < 2; xm++)
				for (int ym = -1; ym < 2; ym++)
				{
					if (xm == 0 && ym == 0) continue;

					blocks[x + xm, y + ym] = NewMapBlock(x + xm, y + ym, empty_block_fab);
					AddBlockToSector(sectors[i + 1], blocks[x + xm, y + ym]);
				}
			
			blocks[x, y] = NewMapBlock(x, y, tower_fabs[i]);
			AddBlockToSector(sectors[i + 1], blocks[x, y]);
		}
		
		//Player
		// Make sector
		sectors[0].color = Sector.BlockColor.Cyan;
		sectors[0].name = "Home Sweet Home";
		sectors[0].sector_type = Sector.SectorType.Player;
		
		// Find corner
		int player_corner = Random.Range(0, 4);
		for (int j = 0; j < 4 && corners_filled[player_corner]; j++)
		{
			player_corner++;
			if (player_corner >= 4)
				player_corner = 0;
		}
		
		int px = player_corner % 2 == 0 ? 0 : map_width - 1;
		int py = player_corner / 2 == 0 ? 0 : map_width - 1;
			
		blocks[px, py] = NewMapBlock(px, py);
		AddBlockToSector(sectors[0], blocks[px, py]);
	}
	
	void _GenerateBlocks()
	{
		// Set base values
		for (int x = 0; x < map_width; x++)
			for (int y = 0; y < map_width; y++)
			{
				if (blocks[x, y] != null) continue;
				
				blocks[x, y] = NewMapBlock(x, y);
			}
	}
	
	void _SectorFirstBlocks()
	{
		// One block per sector
		for (int i = tower_fabs.Length + 1; i < sectors.Length; i++)
		{
			MapBlock b;
			do
			{
				b = blocks[Random.Range(0, map_width), Random.Range(0, map_width)];
				
				if (b.SectorType != Sector.SectorType.None)
				{
					b = null;
					continue;
				}
				
				// This skips any blocks with neighbors that have a sector
				if (GetAdjBlock(b, ~Sector.SectorType.None) != null)
				{
					b = null;
					continue;
				}
			}
			while (b == null);
			
			AddBlockToSector(sectors[i], b);
		}
	}
	
	void _SectorExpansion()
	{
		
		// if we successfully expanded, recurse
		bool expanded = false;
		
		for (int i = tower_fabs.Length + 1; i < sectors.Length; i++)
		{
			int id = Random.Range(0, sectors[i].blocks.Length);
			int id_mod = 0;
			MapBlock a;
			
			// Get a random block that has no sector
			// adjacent to a random block in this sector
			do
			{
				a = GetAdjBlock(sectors[i].blocks[id + id_mod], Sector.SectorType.None);
				
				id_mod++;
				if (id + id_mod >= sectors[i].blocks.Length)
					id_mod = -id;
			}
			while (a == null && id_mod != 0);
			
			// If one is found, convert it
			if (a != null)
			{
				AddBlockToSector(sectors[i], a);
				
				expanded = true;
			}
		}
		
		if (expanded)
			_SectorExpansion();
	}
	
	void _GenerateNeighborhoods()
	{
		for (int i = 0; i < sectors.Length; i++)
		{
			for (int j = 0; j < sectors[i].blocks.Length; j++)
			{
				MapBlock a, b, c, d;
				a = WorldToBlock(sectors[i].blocks[j].transform.position + new Vector3(-1f, 0f, 0f));
				b = WorldToBlock(sectors[i].blocks[j].transform.position + new Vector3(1f, 0f, 0f));
				c = WorldToBlock(sectors[i].blocks[j].transform.position + new Vector3(0f, 0f, 1f));
				d = WorldToBlock(sectors[i].blocks[j].transform.position + new Vector3(0f, 0f, -1f));
				
				if (a != null) AddNeighborToSector(sectors[i], a.sector);
				if (b != null) AddNeighborToSector(sectors[i], b.sector);
				if (c != null) AddNeighborToSector(sectors[i], c.sector);
				if (d != null) AddNeighborToSector(sectors[i], d.sector);
			}
		}
	}
	
	void _GenerateWalls()
	{
		foreach (MapBlock mapblock in blocks)
		{
			if (mapblock.sector == null) continue;
			if (mapblock.walls != null)
				Destroy(mapblock.walls);
			
			bool walls_active = mapblock.walls != null && mapblock.walls.activeSelf;
			
			// Walllllllzzzz
			MapBlock mb_bottom = WorldToBlock(mapblock.transform.position + new Vector3(0f, 0f, -1f));
			MapBlock mb_left = WorldToBlock(mapblock.transform.position + new Vector3(1f, 0f, 0f));
			MapBlock mb_right = WorldToBlock(mapblock.transform.position + new Vector3(-1f, 0f, 0f));
			MapBlock mb_top = WorldToBlock(mapblock.transform.position + new Vector3(0f, 0f, 1f));
			
			MapBlock mb_bottomleft = WorldToBlock(mapblock.transform.position + new Vector3(1f, 0f, -1f));
			MapBlock mb_topleft = WorldToBlock(mapblock.transform.position + new Vector3(1f, 0f, 1f));
			MapBlock mb_bottomright = WorldToBlock(mapblock.transform.position + new Vector3(-1f, 0f, -1f));
			MapBlock mb_topright = WorldToBlock(mapblock.transform.position + new Vector3(-1f, 0f, 1f));
			
			bool bottom, left, right, top;
			bool topleft, topright, bottomleft, bottomright;
			if (mapblock.sector.color == Sector.BlockColor.Cyan)
			{
				bottom = mb_bottom == null || mb_bottom.sector.color != Sector.BlockColor.Cyan;
				left = mb_left == null || mb_left.sector.color != Sector.BlockColor.Cyan;
				right = mb_right == null || mb_right.sector.color != Sector.BlockColor.Cyan;
				top = mb_top == null || mb_top.sector.color != Sector.BlockColor.Cyan;
				
				bottomleft = mb_bottomleft == null || mb_bottomleft.sector.color != Sector.BlockColor.Cyan;
				bottomright = mb_bottomright == null || mb_bottomright.sector.color != Sector.BlockColor.Cyan;
				topleft = mb_topleft == null || mb_topleft.sector.color != Sector.BlockColor.Cyan;
				topright = mb_topright == null || mb_topright.sector.color != Sector.BlockColor.Cyan;
			}
			else
			{
				bottom = mb_bottom == null || mb_bottom.sector != mapblock.sector;
				left = mb_left == null || mb_left.sector != mapblock.sector;
				right = mb_right == null || mb_right.sector != mapblock.sector;
				top = mb_top == null || mb_top.sector != mapblock.sector;
				
				bottomleft = mb_bottomleft == null || mb_bottomleft.sector != mapblock.sector;
				bottomright = mb_bottomright == null || mb_bottomright.sector != mapblock.sector;
				topleft = mb_topleft == null || mb_topleft.sector != mapblock.sector;
				topright = mb_topright == null || mb_topright.sector != mapblock.sector;
			}
			
			GameObject walls = new GameObject("quadwalls");
			GameObject cur_wall;
			
			// Wall fabs
			if (bottom)
			{
				cur_wall = (GameObject)Instantiate(quadwalls[0]);
				
				cur_wall.transform.parent = walls.transform;
			}
			else
			{
				if (left || bottomleft)
				{
					cur_wall = (GameObject)Instantiate(quadwalls[4]);
				
					cur_wall.transform.parent = walls.transform;
				}
				if (right || bottomright)
				{
					cur_wall = (GameObject)Instantiate(quadwalls[6]);
				
					cur_wall.transform.parent = walls.transform;
				}
			}
			
			if (left)
			{
				cur_wall = (GameObject)Instantiate(quadwalls[1]);
				
				cur_wall.transform.parent = walls.transform;
			}
			else
			{
				if (bottom || bottomleft)
				{
					cur_wall = (GameObject)Instantiate(quadwalls[5]);
				
					cur_wall.transform.parent = walls.transform;
				}
				if (top || topleft)
				{
					cur_wall = (GameObject)Instantiate(quadwalls[8]);
				
					cur_wall.transform.parent = walls.transform;
				}
			}
			
			if (right)
			{
				cur_wall = (GameObject)Instantiate(quadwalls[2]);
				
				cur_wall.transform.parent = walls.transform;
			}
			else
			{
				if (bottom || bottomright)
				{
					cur_wall = (GameObject)Instantiate(quadwalls[7]);
				
					cur_wall.transform.parent = walls.transform;
				}
				if (top || topright)
				{
					cur_wall = (GameObject)Instantiate(quadwalls[10]);
				
					cur_wall.transform.parent = walls.transform;
				}
			}
			
			if (top)
			{
				cur_wall = (GameObject)Instantiate(quadwalls[3]);
				
				cur_wall.transform.parent = walls.transform;
			}
			else
			{
				if (left || topleft)
				{
					cur_wall = (GameObject)Instantiate(quadwalls[9]);
				
					cur_wall.transform.parent = walls.transform;
				}
				if (right || topright)
				{
					cur_wall = (GameObject)Instantiate(quadwalls[11]);
				
					cur_wall.transform.parent = walls.transform;
				}
			}
			
			cur_wall = (GameObject)Instantiate(quadwalls[12]);
			cur_wall.transform.parent = walls.transform;
			
			mapblock.walls = walls;
			walls.transform.parent = mapblock.transform;
			walls.transform.localPosition = Vector3.zero;
			
			walls.SetActive(walls_active);
		}
		
		foreach (Sector s in sectors)
			s.Recolor(s.color);
	}
}