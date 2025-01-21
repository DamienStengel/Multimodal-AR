using UnityEngine;
using TMPro; // Nécessaire pour TextMeshPro

public class StudentCell : MonoBehaviour
{
    public TextMeshProUGUI studentNameText; // Référence pour le nom
    public TextMeshProUGUI informationsText; // Référence pour les informations

    // Méthode pour définir les valeurs des textes
    public void SetStudentInfo(string name, string info)
    {
        studentNameText.text = name;
        informationsText.text = info;
    }
}