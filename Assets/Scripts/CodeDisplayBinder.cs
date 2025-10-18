using UnityEngine;
using TMPro;

public class CodeDisplayBinder : MonoBehaviour
{
	[Header("Bindings")]
	[SerializeField] private TMP_Text[] _targets; // Texts to display the code in

	[Header("Code Configuration")]
	[SerializeField] private LevelCodeConfig _config;
	[SerializeField] private string _doorId = "";
	[SerializeField] private string _fallbackCode = "1122";
	[SerializeField] private string _format = "{0}"; // e.g., "Code: {0}"

	private void OnEnable()
	{
		ApplyCodeToTargets();
	}

	public void ApplyCodeToTargets()
	{
		string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		string codeToShow = _fallbackCode;

		if (_config != null && _config.TryGetCode(sceneName, _doorId, out string configCode, out int _))
		{
			codeToShow = configCode;
			Debug.Log($"CodeDisplayBinder: Using code '{codeToShow}' from config (scene='{sceneName}', door='{_doorId}')");
		}
		else
		{
			Debug.LogWarning($"CodeDisplayBinder: No config match (scene='{sceneName}', door='{_doorId}'). Using fallback '{_fallbackCode}'");
		}

		string finalText = string.Format(_format, codeToShow);

		if (_targets == null || _targets.Length == 0)
		{
			// Try find any TMP_Text under this object as a fallback
			TMP_Text anyText = GetComponentInChildren<TMP_Text>(true);
			if (anyText != null)
			{
				anyText.text = finalText;
			}
			return;
		}

		for (int i = 0; i < _targets.Length; i++)
		{
			if (_targets[i] != null)
			{
				_targets[i].text = finalText;
			}
		}
	}
}