using UnityEngine;
using UnityEngine.UI;

public class PathUI : MonoBehaviour
{
    [SerializeField] private Text timeText;
    [SerializeField] private Text selectionText;
    [SerializeField] private Button clearButton;
    
    private void Start()
    {
        if (timeText == null || selectionText == null)
        {
            Debug.LogError("PathUI: Text components non assignés!");
            return;
        }

        clearButton.onClick.AddListener(() => {
            PathManager.Instance.ClearPath();
        });
        
        // Initialiser les textes
        UpdateTimeText("");
        UpdateSelectionText("Sélectionnez une salle de départ");
        HideClearButton();
    }

    public void UpdateTimeText(string time)
    {
        if (timeText != null)
        {
            timeText.text = time != "" ? $"Temps estimé: {time}" : "";
            Debug.Log($"Mise à jour du temps: '{timeText.text}'");
        }
        else
        {
            Debug.LogError("TimeText est null!");
        }
    }

    public void UpdateSelectionText(string text)
    {
        if (selectionText != null)
        {
            // S'assurer que le texte n'est pas null
            text = text ?? "Sélectionnez une salle";
            
            // Mettre à jour le texte
            selectionText.text = text;
            
            // Forcer une mise à jour du layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(selectionText.rectTransform);
            
            Debug.Log($"Text UI mis à jour avec: '{selectionText.text}'");
        }
        else
        {
            Debug.LogError("SelectionText est null!");
        }
    }

    public void ShowClearButton()
    {
        if (clearButton != null)
        {
            clearButton.gameObject.SetActive(true);
        }
    }

    public void HideClearButton()
    {
        if (clearButton != null)
        {
            clearButton.gameObject.SetActive(false);
        }
    }
} 