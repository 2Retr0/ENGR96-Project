using UnityEngine;

public class LightCurveAnimator : MonoBehaviour {

    private Light LightComp;
    public AnimationCurve LightFactor;
    public Renderer LightMaterial;
    private float ActualLightFactor = 0.0f;
    private float ActualLightMaterialFactor = 0.0f;
    public float FactorSpeed = 1.0f;
    public bool active = true;

	// Use this for initialization
    private void Start () {
        if (GetComponent<Light>())
        {
            LightComp = GetComponent<Light>();
            ActualLightFactor = LightComp.intensity;
            if (LightMaterial)
                ActualLightMaterialFactor = LightMaterial.material.GetFloat("_EmissionFactor");
        }
           
        else
            DestroyComp();
   }
	
	// Update is called once per frame
    private void Update () {
        if (LightComp && active)
        {
            LightComp.intensity = ActualLightFactor * LightFactor.Evaluate(Time.time * FactorSpeed);

            if (LightMaterial)
                LightMaterial.material.SetFloat("_EmissionFactor", ActualLightMaterialFactor * LightFactor.Evaluate(Time.time * FactorSpeed));
        }
    }

    private void DestroyComp()
    {
        Debug.LogWarning("Light curve animator script can´t find light component on gameobject : " + gameObject.name + "Destroying Component...");
        Destroy(this);
    }
}
