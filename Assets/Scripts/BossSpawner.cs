using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour {

    [SerializeField] List<WaveConfig> waveConfigs;
    int startingWave = 0;
    [SerializeField] float spawnDelay = 5f;
    [SerializeField] int activatesAtLevel = 5;
    [SerializeField] bool bossSpawned = false;

    //Cached References
    Player player;
    MusicPlayer musicPlayer;

    // Use this for initialization
    private void Start() {
        player = FindObjectOfType<Player>();
        musicPlayer = FindObjectOfType<MusicPlayer>();
    }

    private void Update() {
        if (player.GetLaserLevel() >= activatesAtLevel) {
            if(!bossSpawned) {
                bossSpawned = true;
                musicPlayer.ChangeMusic(1);
                SpawnBoss();
            }
        }
    }

    private void SpawnBoss() {
        StartCoroutine(SpawnAllWaves());
    }

    private IEnumerator SpawnAllWaves() {
        Debug.Log(gameObject.name + " Activated!");
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++) {
            var currentWave = waveConfigs[waveIndex];
            yield return new WaitForSeconds(spawnDelay);
            yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            Debug.Log(gameObject.name + " Deactivated!");
            Destroy(gameObject);
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig) {
        for (int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++) {
            var newEnemy = Instantiate(
                waveConfig.GetEnemyPrefab(),
                waveConfig.GetWaypoints()[0].transform.position,
                Quaternion.identity);

            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
    }
}
