using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SavePrefab : MonoBehaviour
{
    public bool saveBool = false;
    public GameObject popcorn; 
    bool saved = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(saveBool == true && popcorn !=null && saved == false){
            // Create folder Prefabs and set the path as within the Prefabs folder,
            // and name it as the GameObject's name with the .Prefab format
            if (!Directory.Exists("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            string localPath = "Assets/Prefabs/" + popcorn.name + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab and log whether Prefab was saved successfully.
            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAsset(popcorn, localPath, out prefabSuccess);
            if (prefabSuccess == true){
                Debug.Log("Prefab was saved successfully");
                saved = true;
            }
            else
                Debug.Log("Prefab failed to save" + prefabSuccess);


            if (popcorn != null)
            {
                SaveMeshFilter(popcorn);
            }
            else
            {
                Debug.LogError("Please assign a target GameObject.");
            }
        }

    }
    

    void SaveMeshFilter(GameObject gameObject)
{
    MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

    if (meshFilter != null)
    {
        Mesh mesh = meshFilter.sharedMesh;

        if (mesh != null)
        {
            // You can save the mesh to an asset file using AssetDatabase
            // Note: Make sure to include UnityEditor namespace for AssetDatabase
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Prefabs/SavedMesh.asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
#endif
            Debug.Log("Mesh saved successfully.");
        }
        else
        {
            Debug.LogError("Mesh is null.");
        }
    }
    else
    {
        Debug.LogError("MeshFilter component not found on the specified GameObject.");
    }
}
}