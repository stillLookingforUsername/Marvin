using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "LevelCodeConfig", menuName = "Marvin/Level Code Config",order = 0)]
public class LevelCodeConfig : ScriptableObject
{
    [Serializable]
    public class DoorCode
    {
        public string sceneName;    //Optional: Limit by scene
        public string doorId;   //Optional: identify a specific door/panel
        public string code = "1122";
        public int codeLength = 4;
    }

    [SerializeField] private List<DoorCode> codes = new List<DoorCode>();

    public bool TryGetCode(string sceneName, string doorId, out string code, out int length)
    {
        for (int i = 0; i < codes.Count; i++)
        {
            DoorCode entry = codes[i];
            bool sceneMatch = string.IsNullOrEmpty(entry.sceneName) || string.Equals(entry.sceneName, sceneName, StringComparison.Ordinal);
            bool doorMatch = string.IsNullOrEmpty(entry.doorId) || string.Equals(entry.doorId, doorId, StringComparison.Ordinal);
            if (sceneMatch && doorMatch)
            {
                code = entry.code;
                length = Mathf.Max(1, entry.codeLength);
                return true;
            }
        }

        code = string.Empty;
        length = 4;
        return false;
    }
}