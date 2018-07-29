using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

// This will be used throughout our script to determine what behavior our AI is in:
public enum Behaviors {Idle, Guard, Combat, Flee};

public class AI_Agent : MonoBehaviour 
{
	public Behaviors m_aiBehaviors = Behaviors.Idle;

	public bool isSuspicious = false;
	public bool isInRange = false;
	public bool FightsRanged = false;
	public List<KeyValuePair<string, int>> Stats = new List<KeyValuePair<string, int>>();
	public GameObject Projectile;
	public Transform[] Waypoints;
	public int curWaypoint = 0;
	bool ReversePath = false;
	UnityEngine.AI.NavMeshAgent navAgent;
	Vector3 Destination;
	float Distance;
	
	void RunBehaviors() 
	{
		switch(m_aiBehaviors) 
		{
		case Behaviors.Idle:
			RunIdleNode();
			break;
		case Behaviors.Guard:
			RunGuardNode();
			break;
		case Behaviors.Combat:
			RunCombatNode();
			break;
		case Behaviors.Flee:
			RunFleeNode();
			break;
		}
	}

	void ChangeBehavior(Behaviors newBehavior) 
	{
		m_aiBehaviors = newBehavior;
		RunBehaviors();
	}

	void RunIdleNode() 
	{
		Idle();
	}
	
	void RunGuardNode() 
	{
		Guard();
	}
	
	void RunCombatNode() 
	{
		if (FightsRanged) 
		{
			RangedAttack();
		}
		else
		{
			MeleeAttack();
		}
	}
	
	void RunFleeNode() 
	{
		Flee();
	}

	void Idle() 
	{
	}
	
	void Guard() 
	{
		if(isSuspicious) 
		{
			SearchForTarget();
		} 
		else 
		{
			Patrol();
		}
	}
	
	void Combat() 
	{
		if(isInRange) 
		{
			if(FightsRanged) 
			{
				RangedAttack();
			} 
			else 
			{
				MeleeAttack();
			}
		} 
		else 
		{
			SearchForTarget();
		}
	}

	void Flee() 
	{
		for(int fleePoint = 0; fleePoint < Waypoints.Length; fleePoint++) 
		{
			Distance = Vector3.Distance(gameObject.transform.position, Waypoints[fleePoint].position);
			if(Distance > 10.00f) 
			{
				Destination = Waypoints[curWaypoint].position;
				navAgent.SetDestination(Destination);
				break;
			} 
			else if (Distance < 2.00f) 
			{
				ChangeBehavior(Behaviors.Idle);
			}
		}
	}
	
	void SearchForTarget() 
	{
		Destination = GameObject.FindGameObjectWithTag("Player").transform.position;
		navAgent.SetDestination(Destination);
		Distance = Vector3.Distance(gameObject.transform.position, Destination);
		if(Distance < 10)
		{
			ChangeBehavior(Behaviors.Combat);
		}
	}
	
	void Patrol()
	{
		Distance = Vector3.Distance(gameObject.transform.position, Waypoints[curWaypoint].position);
		if(Distance > 2.00f) 
		{
			Destination = Waypoints[curWaypoint].position;
			navAgent.SetDestination(Destination);
		} 
		else 
		{
			if(ReversePath == true) 
			{
				if(curWaypoint <= 0) 
				{
					ReversePath = false;
				} 
				else 
				{
					curWaypoint--;
					Destination = Waypoints[curWaypoint].position;
				}
			}
			else 
			{
				if(curWaypoint >= (Waypoints.Length - 1)) 
				{
					ReversePath = true;
				}
				else 
				{
					curWaypoint++;
					Destination = Waypoints[curWaypoint].position;
				}
			}
		}
	}
		
	void RangedAttack() 
	{
		GameObject newProjectile;
		// projectile will run its own script
		newProjectile = Instantiate(Projectile, transform.position, Quaternion.identity) as GameObject;
	}
	
	void MeleeAttack() 
	{
	}

	void ChangeHealth(int Amount)
	{
		if(Amount < 0)
		{
			if(!isSuspicious)
			{
				isSuspicious = true;
				ChangeBehavior(Behaviors.Guard);
			}
		}
		
		for(int i = 0; i < Stats.Capacity; i++)
		{
			if(Stats[i].Key == "Health")
			{
				int tempValue = Stats[i].Value;
				Stats[i] = new KeyValuePair<string, int>(Stats[i].Key, tempValue += Amount);
				if(Stats[i].Value <= 0)
				{
					Destroy(gameObject);
				} 
				else if(Stats[i].Value < 25)
				{
					isSuspicious = false;
					ChangeBehavior(Behaviors.Flee);
				}
				break;
			}
		}
	}
	
	void ModifyStat(string Stat, int Amount)
	{
		for(int i = 0; i < Stats.Capacity; i++)
		{
			if(Stats[i].Key == Stat)
			{
				int tempValue = Stats[i].Value;
				Stats[i] = new KeyValuePair<string, int>(Stats[i].Key, tempValue += Amount);
				break;
			}
		}
		
		if(Amount < 0)
		{
			if(!isSuspicious)
			{
				isSuspicious = true;
				ChangeBehavior(Behaviors.Guard);
			}
		}
	}

}
