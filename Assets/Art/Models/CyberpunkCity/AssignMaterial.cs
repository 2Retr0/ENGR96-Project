using UnityEditor;
using UnityEngine;

public class AssignMaterial : ScriptableWizard {

    public Material materialToApply;

    void OnWizardUpdate ()
    {
        helpString = "Select Game Objects";
        isValid = ( materialToApply != null );
    }

    void OnWizardCreate ()
    {
        GameObject [] gos = Selection.gameObjects;
        foreach( GameObject go in gos )
        {
            Material[] materials = go.GetComponent<Renderer> ().sharedMaterials;
            for ( int i = 0 ; i < materials.Length ; i++ )
                materials [ i ] = materialToApply;
            go.GetComponent<Renderer> ().sharedMaterials = materials;

            materials = go.GetComponent<Renderer> ().materials;
            for ( int i = 0 ; i < materials.Length ; i++ )
                materials [ i ] = materialToApply;
            go.GetComponent<Renderer> ().materials = materials;


        }

    }

    [MenuItem("GameObject/Assign Material", false, 4)]
    static void CreateWindow ()
    {
        ScriptableWizard.DisplayWizard ("Assign Material", typeof(AssignMaterial), "Assign");
    }
}