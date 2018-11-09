using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    //Config Parameters
    [Header("Stats")]
    [SerializeField] float health = 100f;
    [SerializeField] int scorePerKill = 100;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] float projectileSpeed = 5f;

    [Header("Drops")]
    [SerializeField] List<GameObject> powerUpDrop;
    [SerializeField] float powerUpDropSpeed = 3.5f;
    [SerializeField] [Range(0, 100)] float dropRate = 90f;

    [Header("VFX/SFX")]
    [SerializeField] GameObject explosionParticle;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip laserSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] [Range(0,1)] float laserSoundVolume = 0.2f;
    [SerializeField] [Range(0,1)] float deathSoundVolume = 0.7f;
    [SerializeField] [Range(0,1)] float hitsSoundVolume = 0.4f;

    //Cached References
    GameSession gameSession;

    // Use this for initialization
    void Start () {
        gameSession = FindObjectOfType<GameSession>();
        ResetShotCounter();
    }

    private void ResetShotCounter() {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update () {
        CountDownAndShoot();
	}


    //Firing
    private void CountDownAndShoot() {
        shotCounter -= Time.deltaTime;
        if(shotCounter <= 0f) {
            Fire();
            ResetShotCounter();
        }
    }

    private void Fire() {
        GameObject laser = Instantiate(
            projectile, 
            transform.position, 
            Quaternion.Euler(180,0,0)) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
        PlaySFX(laserSFX, laserSoundVolume);
    }

    //Hitbox and Health
    private void OnTriggerEnter2D(Collider2D other) {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer) {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0) {
            PlaySFX(deathSFX, deathSoundVolume);
            DropPowerUp();
            Die();
        } else {
            PlaySFX(hitSFX, hitsSoundVolume);
        }
    }

    private void DropPowerUp() {
        float dropModifier = Random.Range(0f, 100f);
        if(powerUpDrop.Count > 0 && dropModifier < dropRate) {
            GameObject droppedItem = Instantiate(
                powerUpDrop[Random.Range(0, powerUpDrop.Count)],
                transform.position,
                Quaternion.identity) as GameObject;
            droppedItem.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -powerUpDropSpeed);
        }
    }

    private void Die() {
        gameSession.AddToScore(scorePerKill);
        Destroy(gameObject);
        GameObject explosion = Instantiate(
                        explosionParticle,
                        transform.position,
                        Quaternion.identity) as GameObject;
        Destroy(explosion, durationOfExplosion);
    }

    private void PlaySFX(AudioClip sfx, float vol) {
        AudioSource.PlayClipAtPoint(sfx, Camera.main.transform.position, vol);
    }
}
