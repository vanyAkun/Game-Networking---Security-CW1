using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;

public class MultiplayerBulletController : MonoBehaviourPunCallbacks
{
    public int damage = 50;
    Rigidbody rigidBody;

    public float bulletSpeed = 15f;

    public AudioClip BulletHitAudio;

    public GameObject bulletImpactEffect;
   
  

    [HideInInspector]
    public Photon.Realtime.Player owner;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        if (rigidBody != null)
            print("Rigidbody is found!");
        else
            print("Rigidbody isn't found!");
    }

    public void InitializeBullet(Vector3 originalDirection, Photon.Realtime.Player givenPlayer) 
    {
        print(originalDirection);
        transform.forward = originalDirection;
        rigidBody.velocity = transform.forward * bulletSpeed;

        owner= givenPlayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position);

        VFXManager.instance.PlayVFX(bulletImpactEffect, transform.position);

        Destroy(gameObject);
    }
}
