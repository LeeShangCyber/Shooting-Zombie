﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HandGunScript : MonoBehaviour
{

    Animator animator;

    bool isOutOfAmmo = false;
    bool isWalking = false;
    bool isAiming = false;
    bool isReloading = false;
    string FireButton = "Fire1";
    int currentAmmo = 6;
    Light LightBullet;

    public AudioSource shootAudioSource;
    public AudioSource mainAudioSource;
    [Header("Gun camera")]
    public Camera fpsCam;

    [Header("Gun attribute")]
    [SerializeField]
    float damage = 10f;
    // [SerializeField]
    //float range = 100f;
    [SerializeField]
    int ammo = 6;

    [Header("Canvas Information Gun")]
    public Canvas UIGunInformation;
    public Text ammoQuantity;
    public Text GunName;

    [Header("Particle Effect")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem SparkFlash;

    [Header("Bullet & its attributes")]
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject SpawnBulletPoint;
    [SerializeField]
    GameObject casingPrefab;
    [SerializeField]
    GameObject SpawnCasingPoint;
    [SerializeField]
    GameObject LightPoint;

    [System.Serializable]
	public class SoundClips
	{
        [Header("Audio Clips")]
		public AudioClip shootSound;
		public AudioClip silencerShootSound;
		// public AudioClip takeOutSound;
		// public AudioClip holsterSound;
		public AudioClip reloadSoundOutOfAmmo;
		public AudioClip reloadSoundAmmoLeft;
		// public AudioClip aimSound;
	}
	public SoundClips soundClips;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentAmmo = ammo;
        LightBullet = LightPoint.GetComponent<Light>();
    }

    void Update()
    {

        if (GameManager.Instance.gameState != GameState.Playing)
            return;

        AnimationCheck();

        if (Input.GetButtonDown(FireButton) && !isReloading)
        {
            Shoot();
        }
        else
        {
            LightBullet.enabled = false;
            if (Input.GetKey(KeyCode.Q))
            {
                animator.Play("GrenadeThrow");
            }
        }

        if (Input.GetMouseButton(1) && !isReloading)
        {
            isAiming = true;
            fpsCam.fieldOfView = Mathf.Lerp (fpsCam.fieldOfView, 25.0f, 4f * Time.deltaTime);
            animator.SetBool("Aim", true);
        }
        else
        {
            isAiming = false;
            fpsCam.fieldOfView = Mathf.Lerp (fpsCam.fieldOfView, 40.0f, 4f * Time.deltaTime);
            animator.SetBool("Aim", false);
        }

		//Walking when pressing down WASD keys
		if (Input.GetKey (KeyCode.W) || 
			Input.GetKey (KeyCode.A) || 
			Input.GetKey (KeyCode.S) || 
			Input.GetKey (KeyCode.D)) 
		{
            isWalking  = true;
			animator.SetBool ("Walk", true);
		} else {
			animator.SetBool ("Walk", false);
            isWalking = false;
        }

        if (Input.GetKey(KeyCode.R) && !isReloading)
        {
            isReloading = true;
            Reload();

        }

    }

    void Shoot()
    {

        if (!isOutOfAmmo)
        {
            LightBullet.enabled = true;
            shootAudioSource.clip = soundClips.shootSound;
            shootAudioSource.Play();
            currentAmmo--;
            ammoQuantity.text = currentAmmo.ToString();
             GameObject bullet = Instantiate(bulletPrefab, SpawnBulletPoint.transform.position, SpawnBulletPoint.transform.rotation);
            bullet.transform.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 1000;
            
            Instantiate(casingPrefab, SpawnCasingPoint.transform.position, SpawnCasingPoint.transform.rotation);

            if (!isAiming)
            {
                animator.Play ("Fire", 0, 0f);
            }

            else
            {
                animator.Play ("Aim Fire", 0, 0f);
            }

            if (!isWalking)
            {
                muzzleFlash.Emit(1);
                SparkFlash.Emit(Random.Range(1, 6));
            }
        }
        else
        {
            LightBullet.enabled = false;
        }

        if (currentAmmo <= 0)
            isOutOfAmmo = true;

    }

    void Reload()
    {
        if (isOutOfAmmo)
        {
            animator.Play ("Reload Out Of Ammo", 0, 0f);
            mainAudioSource.clip = soundClips.reloadSoundOutOfAmmo;
            mainAudioSource.Play();
        }

        else 
		{
			//Play diff anim if ammo left
			animator.Play ("Reload Ammo Left", 0, 0f);
            mainAudioSource.clip = soundClips.reloadSoundAmmoLeft;
            mainAudioSource.Play();
        }

		currentAmmo = ammo;
        ammoQuantity.text = currentAmmo.ToString();
        isOutOfAmmo = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(" collider " + other);

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("GrenadeThrow"))
        {
            GetDamage dame = other.GetComponent<GetDamage>();
            if (dame != null)
            {
                dame.ProcessDame(5);
            }
        }

    }

    private void AnimationCheck () 
	{
		//Check if reloading
		//Check both animations
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Reload Out Of Ammo") || 
			animator.GetCurrentAnimatorStateInfo (0).IsName ("Reload Ammo Left")) 
		{
			isReloading = true;
		} 
		else 
		{
			isReloading = false;
		}

		//Check if inspecting weapon
		// if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Inspect")) 
		// {
		// 	isInspecting = true;
		// } 
		// else 
		// {
		// 	isInspecting = false;
		// }
    }

}
