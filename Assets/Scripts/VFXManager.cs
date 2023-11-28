using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else 
            Destroy(instance);
    }

    public void PlayVFX(GameObject effectObject, Vector3 effectPosition) 
    {
        GameObject vfxObject = Instantiate(effectObject, effectPosition, Quaternion.identity);

        ParticleSystem[] particleSystems = vfxObject.GetComponentsInChildren<ParticleSystem>();

        float maxLength = 0f;
        foreach (ParticleSystem individualParticleSystem in particleSystems) 
        {
            float currentKnownMaxLength = individualParticleSystem.main.duration 
                + individualParticleSystem.main.startLifetime.constantMax;

            if(currentKnownMaxLength > maxLength)
                maxLength = currentKnownMaxLength;
        }

        Destroy(vfxObject, maxLength);
    }
}
