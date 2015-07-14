using UnityEngine;
using System.Collections;
using FMOD.Studio;

namespace CompleteProject
{
    public class EnemyMovement : MonoBehaviour
    {
		public int maxAudibleEnemies;				// The number of enemies closest to the player whose footsteps will make sound
		public string audioPath;					// Path to the FMOD footsteps event		

		Transform player;               			// Reference to the player's position.
        PlayerHealth playerHealth;      			// Reference to the player's health.
        EnemyHealth enemyHealth;        			// Reference to this enemy's health.
        NavMeshAgent nav;               			// Reference to the nav mesh agent.

		bool isMoving;								// Whether or not the enemy is moving.
		EnemyManager[] enemyManagers;				// Reference to all enemy managers
		FMOD.Studio.EventInstance footStepsAudio;	// The footsteps audio event.

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealth = GetComponent<EnemyHealth>();
            nav = GetComponent<NavMeshAgent>();

			isMoving = false;
			footStepsAudio = FMOD_StudioSystem.instance.GetEvent(audioPath); 

			enemyManagers = GameObject.Find("/EnemyManager").GetComponents<EnemyManager>();
        }
		
        void FixedUpdate()
        {
            // If the enemy and the player have health left...
            if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
            {
                // ... set the destination of the nav mesh agent to the player.
                nav.SetDestination (player.position);
				isMoving = true;
            }
            // Otherwise...
            else
            {
                // ... disable the nav mesh agent.
                nav.enabled = false;
				isMoving = false;
            }

			// JD
			bool audioConditionsMet = isMoving && IsNearest();
			AudioManager.startStopLoop(footStepsAudio, audioConditionsMet, gameObject);
		}

		// JD
		// Checks to see if the enemy is one of the n nearest to the player
		bool IsNearest()
		{
			ArrayList enemyDistances = new ArrayList();
			float distFromPlayer = Vector3.Distance(transform.position, player.transform.position);
			int enemyCount = 0;

			foreach(EnemyManager m in enemyManagers)
			{
				ArrayList enemies = m.enemies;

				if(enemies != null && enemies.Count > 0)
				{
					enemyCount += enemies.Count;

					foreach(GameObject e in enemies)
					{
						float distance = Vector3.Distance(e.transform.position, player.transform.position);
						enemyDistances.Add(distance);
					}
				}
			}

			if(enemyCount > maxAudibleEnemies)
			{	
				enemyDistances.Sort();
				return distFromPlayer <= (float) enemyDistances[maxAudibleEnemies - 1];
			}
			else
			{
				return true;
			}
		}
    }
}