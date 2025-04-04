using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float countdown;

    public Wave[] waves;

    [SerializeField] private GameObject spawnPoint;

    private int currentWaveIndex = 0;

    private void Update()
    {
         countdown -= Time.deltaTime;

        if(countdown <= 0)
        {
            StartCoroutine(SpawnWave() );
        }

    }

    private IEnumerator SpawnWave()
    {
        for(int i =0; i < waves[currentWaveIndex].enemy.Length; i++)
        {
            Instantiate(waves[currentWaveIndex].enemy[i], spawnPoint.transform);
            Debug.Log("Enemy is spawned");

            yield return new WaitForSeconds(waves[currentWaveIndex].timeToNextZombie);
        }
    }
}

[System.Serializable]

public class Wave
{
    //public Zombie[] zombies;
    public Enemy[] enemy;
    public float timeToNextZombie;
}