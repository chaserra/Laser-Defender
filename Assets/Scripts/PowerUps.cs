using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour {
    int scoreOnPickup = 500;
    int addHealth = 50;

    public int AddToScoreOnPickup() {
        return scoreOnPickup;
    }

    public int Heal() {
        return addHealth;
    }

	public void ObtainedPowerUp() {
        Destroy(gameObject);
    }
}
