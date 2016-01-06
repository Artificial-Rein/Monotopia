using UnityEngine;
using System.Collections;
using System;

public abstract class Squad : ShidouGameObject
{
	public delegate void ChangeSquadMemberEventHandler(Enemy member, Squad squad);
	public event ChangeSquadMemberEventHandler NewSquadMember;
	public event ChangeSquadMemberEventHandler LoseSquadMember;
	
	public delegate void SquadDestroyedEvent(Squad squad);
	public event SquadDestroyedEvent SquadDestroyed;
	
	bool spawned = false;
	public bool Spawned { get { return spawned; } }
	
	public int total_units;
	int num_members;
	int gaps;
	public int NumMembers { get { return num_members; } }

	protected bool spawning;
	public bool Spawning { get { return spawning; } }
	
	protected Enemy[] members;
	
	public IEnumerator GetEnumerator()
	{
		for (int i = 0; i < members.Length; i++)
		{
			if (members[i] == null) continue;
			else yield return members[i];
		}
	}
	
	const int padding = 5;
	
	public virtual void Add(Enemy right)
	{
		right.squad = this;
		
		right._Initialize();
		
		if (NewSquadMember != null)
			NewSquadMember(right, this);
		
		if (members.Length == num_members)
		{
			Enemy[] temp = new Enemy[num_members + padding];
			for (int i = 0; i < num_members; i++)
				temp[i] = members[i];
			members = temp;
		}
		
		if (gaps == 0)
			members[num_members] = right;
		else
		{
			for (int i = 0; i < members.Length; i++)
				if (members[i] == null)
				{
					members[i] = right;
					gaps--;
					break;
				}
		}
		num_members++;
		right.Death += Remove;
	}
	
	public virtual void Remove(Enemy member)
	{	
		for (int i = 0; i < members.Length; i++)
		{
			if (members[i] == member)
			{
				if (i < num_members + gaps - 1)
					gaps++;
				members[i] = null;
				num_members--;
				
				if (LoseSquadMember != null)
					LoseSquadMember(member, this);
				
				total_units--;
				if (total_units <= 0)
				{
					if (SquadDestroyed != null)
						SquadDestroyed(this);
					Destroy(gameObject);
				}
				return;
			}
		}
	}
	
	protected virtual void Start()
	{
		spawned = false;

		spawning = false;
		
		members = new Enemy[0];
	}
	
	public void Spawn()
	{
		spawned = true;
		_Spawn();
	}
	public abstract void _Spawn();
}
