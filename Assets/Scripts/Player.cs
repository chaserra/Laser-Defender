using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    //Config Parameters
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    [Header("Player Stats")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 0.5f;
    [SerializeField] int health = 200;
    [SerializeField] GameObject playerShield;
    [SerializeField] int numberOfShields = 0;
    [SerializeField] float shieldDuration = 2.5f;
    [SerializeField] bool shieldActive = false; //Serialized for debugging
    [SerializeField] bool invincible = false; //Serialized for debugging
    int maxHealth = 1000;

    [Header("Projectile")]
    [SerializeField] GameObject playerLaser;
    [SerializeField] int laserLevel = 1;
    int maxLaserLevel = 5;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.5f;
    [SerializeField] float maxFireRate = .2f;

    [Header("VFX/SFX")]
    [SerializeField] GameObject explosionParticle;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip laserSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip getPointSFX;
    [SerializeField] AudioClip laserUpSFX;
    [SerializeField] AudioClip healSFX;
    [SerializeField] [Range(0, 1)] float laserSoundVolume = 0.2f;
    [SerializeField] [Range(0, 5)] float deathSoundVolume = 5f;
    [SerializeField] [Range(0, 1)] float hitsSoundVolume = 0.8f;
    [SerializeField] [Range(0, 1)] float getPointVolume = .5f;
    [SerializeField] [Range(0, 1)] float laserUpgradeVolume = 0.8f;
    [SerializeField] [Range(0, 1)] float healSoundVolume = .8f;

    Coroutine firingCoroutine;


    //Cached References
    AudioSource myAudioSource;
    GameObject laser;
    GameObject shield;
    GameSession gameSession;

    //State Variables
    private bool isFiring = false;


    // Use this for initialization
    void Start () {
        SetUpMoveBoundaries();
        gameSession = FindObjectOfType<GameSession>();
    }

    // Update is called once per frame
    void Update() {
        Move();
        Fire();
        ActivateShield();
        if(shieldActive) {
            MoveShieldWithPlayer();
        }
    }

    //Movement
    private void Move() {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries() {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    //Firing
    private void Fire() {
        if(Input.GetButtonDown("Fire1") && !(isFiring)) {
            isFiring = true;
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if(Input.GetButtonUp("Fire1")) {
            StopCoroutine(firingCoroutine);
            isFiring = false;
        }
    }

    IEnumerator FireContinuously() {
        Vector3[] offsetLaser = new[] {
            new Vector3(0f, 0f, 0f),
            new Vector3(-.4f, .2f, 0),
            new Vector3(.4f, .2f, 0),
            new Vector3(-.2f, .2f, 0),
            new Vector3(.2f, .2f, 0) };

        while (true) {
            for (int counter = 0; counter < laserLevel; counter++) {
                ShootLasers(offsetLaser[counter]);
            }
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void ShootLasers(Vector3 offset) {
        laser = Instantiate(
            playerLaser,
            transform.position + offset,
            Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
        PlaySFX(laserSFX, laserSoundVolume);
    }

    //Activate Shield
    private void ActivateShield() {
        if(Input.GetButtonUp("Fire2")) {
            if(numberOfShields > 0 && !(shieldActive)) {
                shieldActive = true;
                invincible = true;
                shield = Instantiate(
                    playerShield,
                    transform.position,
                    Quaternion.identity) as GameObject;
                numberOfShields--;
                StartCoroutine(DeactivateShield(shieldDuration));
            }
        }
    }

    private void MoveShieldWithPlayer() {
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
        shield.transform.position = playerPos;
    }

    IEnumerator DeactivateShield(float duration) {
        yield return new WaitForSeconds(duration);
        Destroy(shield.gameObject);
        shieldActive = false;
        invincible = false;
    }

    //Damage Controller, Power-ups
    private void OnTriggerEnter2D(Collider2D other) {
        PowerUps powerUp = other.gameObject.GetComponent<PowerUps>();
        if(other.tag == "Laser Powerup") {
            if (laserLevel < maxLaserLevel) {
                if (maxFireRate < projectileFiringPeriod - .01f) {
                    powerUp.ObtainedPowerUp();
                    projectileFiringPeriod -= .1f;
                    PlaySFX(laserUpSFX, laserUpgradeVolume);
                    return;
                } else {
                    laserLevel++;
                    projectileFiringPeriod = .5f;
                    PlaySFX(laserUpSFX, laserUpgradeVolume);
                }
            } else if (maxFireRate < projectileFiringPeriod - .01f) {
                powerUp.ObtainedPowerUp();
                projectileFiringPeriod -= .1f;
                PlaySFX(laserUpSFX, laserUpgradeVolume);
                return;
            } else {
                PowerUpAddScore(powerUp);
                PlaySFX(getPointSFX, getPointVolume);
            }
            powerUp.ObtainedPowerUp();
        } 

        if(other.tag == "Health Pack") {
            if (health < maxHealth) {
                health = health + powerUp.Heal();
                PlaySFX(healSFX, healSoundVolume);
                if(health > maxHealth) {
                    health = maxHealth;
                }
            } else {
                PowerUpAddScore(powerUp);
                PlaySFX(getPointSFX, getPointVolume);
            }
            powerUp.ObtainedPowerUp();
        }

        if (other.tag == "Shield Pack") {
            numberOfShields++;
            PlaySFX(getPointSFX, getPointVolume);
            powerUp.ObtainedPowerUp();
        } 
        
        else {
            if(!invincible) {
                DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
                if (!damageDealer) { return; }
                ProcessHit(damageDealer);
            }
        }
    }
    //TODO Try to refactor powerups (much better if a new class is added)

    private void PowerUpAddScore(PowerUps powerUp) {
        int pickupScore = powerUp.AddToScoreOnPickup();
        gameSession.AddToScore(pickupScore);
    }

    //Damage and Death
    private void ProcessHit(DamageDealer damageDealer) {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        invincible = true;
        if (health <= 0) {
            PlaySFX(deathSFX, deathSoundVolume);
            Die();
        } else {
            PlaySFX(hitSFX, hitsSoundVolume);
            Invoke("resetInvulnerability", 2f);
        }
    }

    private void Die() {
        Destroy(gameObject);
        GameObject explosion = Instantiate(
            explosionParticle,
            transform.position,
            Quaternion.identity) as GameObject;
        Destroy(explosion, durationOfExplosion);
        FindObjectOfType<Level>().LoadGameOver();
    }

    private void resetInvulnerability() {
        invincible = false;
    }

    private void PlaySFX(AudioClip sfx, float vol) {
        AudioSource.PlayClipAtPoint(sfx, Camera.main.transform.position, vol);
    }

    public int GetHealth() {
        return health;
    }

    public int GetNumberOfShields() {
        return numberOfShields;
    }

    public int GetLaserLevel() {
        return laserLevel;
    }
}
