using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] List<WaveConfig> waveConfigs;
    [SerializeField] int startingWave = 0; //Serialized for debugging
    [SerializeField] float spawnDelay = 1.5f;
    [SerializeField] int activatesAtLevel = 1;
    [SerializeField] bool looping = false;

    //Cached References
    Player player;

	// Use this for initialization
	IEnumerator Start() {
        player = FindObjectOfType<Player>();
        do {
            yield return StartCoroutine(SpawnAllWaves());
        } while (looping);
    }

    private IEnumerator SpawnAllWaves() {
        if (player.GetLaserLevel() >= activatesAtLevel) {
            yield return new WaitForSeconds(2f);
            Debug.Log(gameObject.name + " Activated!");
            for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++) {
                var currentWave = waveConfigs[waveIndex];
                yield return new WaitForSeconds(spawnDelay);
                yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            }
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig) {
        for(int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++) {
            var newEnemy = Instantiate(
                waveConfig.GetEnemyPrefab(),
                waveConfig.GetWaypoints()[0].transform.position,
                Quaternion.identity);

            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
    }

    //TODO rework waves. balance more
    //TODO Final boss at level 5.
    //removed GIT Test
}
