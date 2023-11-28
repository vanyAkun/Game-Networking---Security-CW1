using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    Rigidbody rigidBody;

    public float bulletSpeed = 15f;

    public AudioClip BulletHitAudio;

    public GameObject bulletImpactEffect;

    public int damage = 10;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        if (rigidBody != null)
            print("Rigidbody is found!");
        else
            print("Rigidbody isn't found!");
    }

    public void InitializeBullet(Vector3 originalDirection) 
    {
        print(originalDirection);
        transform.forward = originalDirection;
        rigidBody.velocity = transform.forward * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position);

        VFXManager.instance.PlayVFX(bulletImpactEffect, transform.position);

        Destroy(gameObject);
    }
}
