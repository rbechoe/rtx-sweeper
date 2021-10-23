using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Runics : MonoBehaviour, ITriggerable
{
    private bool triggered;
    private HDAdditionalLightData babyLight;
    private DecalProjector decalProjector;

    private float increaser = 0.1f;
    private float speed = 15f;

    void Start()
    {
        babyLight = gameObject.transform.GetChild(0).GetComponent<HDAdditionalLightData>();
        decalProjector = gameObject.GetComponent<DecalProjector>();
        decalProjector.fadeFactor = 0;
        babyLight.intensity = 0;
    }

    public void Activate()
    {
        if (!triggered)
        {
            triggered = true;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    private void Update()
    {
        if (triggered)
        {
            increaser += Time.deltaTime * speed;
            babyLight.intensity = increaser;
            decalProjector.fadeFactor = increaser / 15f;

            if (increaser >= 15)
            {
                decalProjector.fadeFactor = 1;
                babyLight.intensity = 15;
                this.enabled = false;
            }
        }
    }
}
