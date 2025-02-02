using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLasers : MonoBehaviour
{
    public Transform bossCenter;           // The boss's central position
    public Transform[] lasers;             // Array of laser emitters (front, back, left, right)
    public ParticleSystem[] laserParticles; // Particle effects for the lasers
    public float rotationSpeed = 20f;      // Speed at which the lasers rotate around the boss
    public float maxLaserLength = 10f;     // Maximum length of the laser
    public float laserGrowthSpeed = 2f;    // Speed at which the laser grows
    public int laserDamage = 10;           // Damage dealt to the player by the lasers

    [Header("Debug Settings")]
    public bool showDebugLines = true;     // Toggle to show debug lines
    public float debugLineLength = 15f;    // Adjustable length of debug lines in the Scene view

    private float currentLaserLength = 0f; // Current length of the laser

    private void OnEnable()
    {
        FindAnyObjectByType<AudioManager>().Play("boost");
    }
    private void Update()
    {
        // Rotate the laser system around the boss
        transform.RotateAround(bossCenter.position, Vector3.up, rotationSpeed * Time.deltaTime);

        // Grow the laser's length gradually
        currentLaserLength = Mathf.Min(currentLaserLength + laserGrowthSpeed * Time.deltaTime, maxLaserLength);

        // Update each laser's visuals
        for (int i = 0; i < lasers.Length; i++)
        {
            Transform laser = lasers[i];
            ParticleSystem laserParticle = laserParticles[i];

            // Update the particle visuals to match the laser length
            UpdateParticleEffect(laserParticle, currentLaserLength);

            // Update the collider size to match the laser length
            UpdateColliderSize(laser, currentLaserLength);
        }
    }

    private void UpdateParticleEffect(ParticleSystem particleSystem, float length)
    {
        // Scale the Z-axis of the particle's shape to match the laser length
        var shape = particleSystem.shape;
        shape.scale = new Vector3(1, 1, length);
    }

    private void UpdateColliderSize(Transform laser, float length)
    {
        BoxCollider collider = laser.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.center = new Vector3(0, 0, length / 2); // Move the collider forward
            collider.size = new Vector3(collider.size.x, collider.size.y, length); // Update the length
        }
    }
    private void OnDisable()
    {
        FindAnyObjectByType<AudioManager>().Stop("boost");
    }
}