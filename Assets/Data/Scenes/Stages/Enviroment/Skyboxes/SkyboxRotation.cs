using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkyboxRotation : MonoBehaviour {

    // Speed multiplier
    public float speedMultiplier;
    public float blendMultiplier;
    public float exposureMultiplier;
    public float exposureSpeed;

    float exposure;
    float rotation;
    float blend;

    void Awake()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }
    // Update is called once per frame
    void Update()
    {
        exposure = (float)(0.77 + (Math.Abs(Math.Sin(Time.time * exposureSpeed) * exposureMultiplier)));
        rotation = Time.time * speedMultiplier;
        blend = (float)Math.Sin(Time.time * blendMultiplier);


        //Sets the float value of "_Rotation", adjust it by Time.time and a multiplier.
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
        RenderSettings.skybox.SetFloat("_SkyBlend", blend);
        RenderSettings.skybox.SetFloat("_Exposure", exposure);
    }
}
