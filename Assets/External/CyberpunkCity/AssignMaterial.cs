using UnityEditor;
using UnityEngine;
using System.Collections;

public class AssignMaterial : ScriptableWizard {

    public Material material_to_apply;

    void OnWizardUpdate ()
    {
        helpString = "Select Game Objects";
        isValid = ( material_to_apply != null );
    }

    void OnWizardCreate ()
    {
        GameObject [] gos = Selection.gameObjects;
        foreach( GameObject go in gos )
        {
            Material[] materials = go.GetComponent<Renderer> ().sharedMaterials;
            for ( int i = 0 ; i < materials.Length ; i++ )
                materials [ i ] = material_to_apply;
            go.GetComponent<Renderer> ().sharedMaterials = materials;

            materials = go.GetComponent<Renderer> ().materials;
            for ( int i = 0 ; i < materials.Length ; i++ )
                materials [ i ] = material_to_apply;
            go.GetComponent<Renderer> ().materials = materials;


        }

    }

    [MenuItem("GameObject/Assign Material", false, 4)]
    static void CreateWindow ()
    {
        ScriptableWizard.DisplayWizard ("Assign Material", typeof(AssignMaterial), "Assign");
    }
}