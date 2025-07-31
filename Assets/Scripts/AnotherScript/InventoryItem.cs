using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR    //this will run only if it detect unity editor otherwise if in phone it won't work(to save from error)

using UnityEditor;

#endif

public class InventoryItem : MonoBehaviour
{
    private enum ItemType {background, decoration, food}

    [Header("Present")]
    [SerializeField] private ItemType itemType;

    List<GameObject> background = new List<GameObject>();
    //etc

    [SerializeField] private string itemName;
    [SerializeField] private int cost;
    [SerializeField] private float happiness;

#if UNITY_EDITOR
    [CustomEditor(typeof(InventoryItem))]
    public class InventoryItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            InventoryItem iItem = (InventoryItem)target;
            
            // Draw the default inspector for non-custom fields
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Details", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Item Name:", GUILayout.Width(80));
            iItem.itemName = EditorGUILayout.TextField(iItem.itemName);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Cost:", GUILayout.Width(80));
            iItem.cost = EditorGUILayout.IntField(iItem.cost);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Happiness:", GUILayout.Width(80));
            iItem.happiness = EditorGUILayout.FloatField(iItem.happiness);
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
