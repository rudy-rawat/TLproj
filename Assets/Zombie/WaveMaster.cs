using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ZombieWave
{
    public int[] zombieTypeCounts;
}




public class WaveMaster : MonoBehaviour
{
    [SerializeField] AnimationCurve HealthMultipliyer;
    [SerializeField] AnimationCurve StrengthMultipliyer;
    [SerializeField] AnimationCurve SpeedMultipliyer;
    int WaveNumber;
    public GameObject[] MonsterPrefabs;
    [SerializeField] Transform[] SpawnPoints;
    [SerializeField] int SpawnPointRandomRange;
    int zombieTypeCount;

    public List<ZombieWave> waves = new List<ZombieWave>();

    public int CurrentWave;

    // Optional: auto-fill each wave with 0s when new ones are added
    private void OnValidate()
    {
        if (MonsterPrefabs != null)
        {
            zombieTypeCount = MonsterPrefabs.Length;
        }

    }

    private void Start()
    {
        InstantiateNextWave();
    }


    void InstantiateNextWave()
    {
        int ZombieTypeIndex = 0;
        foreach (int Number in waves[CurrentWave].zombieTypeCounts)
        {
            SpawnSpecificZombieType(Number, ZombieTypeIndex);
            ZombieTypeIndex++;
        }
        ZombieTypeIndex = 0;
    }


    void SpawnSpecificZombieType(int NumberOfZombies, int TypeIndex)
    {
        for (int i=0; i<NumberOfZombies; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-SpawnPointRandomRange, SpawnPointRandomRange),
                Random.Range(0, SpawnPointRandomRange),
                Random.Range(-SpawnPointRandomRange, SpawnPointRandomRange)
            );

            Vector3 spawnBase = SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;
            Vector3 spawnPosition = spawnBase + randomOffset;

            Instantiate(MonsterPrefabs[TypeIndex], spawnPosition, Quaternion.identity);
        }
        CurrentWave++;
    }



 
    

    }




