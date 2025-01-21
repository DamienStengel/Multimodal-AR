using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;


public class MenuManager : MonoBehaviour
{
    public string csv = "students";
    List<Student> students = new List<Student>();  // Liste pour stocker les étudiants
    List<Student> filteredStudents = new List<Student>();  // Liste pour stocker les étudiants

    public TMP_Dropdown yearDropdown;
    public TMP_Dropdown specializationDropdown;
    public TMP_Dropdown transportDropdown;

    public List<int> uniqueYears;
    public List<string> uniqueSpecializations;
    public List<string> uniqueTransports;


    public GameObject studentCellPrefab; // Le prefab StudentCell
    public Transform parentContainer;   // Le parent pour les cellules


    public RectTransform content;           // Référence au RectTransform du Content
    public GridLayoutGroup gridLayoutGroup; // Référence au GridLayoutGroup

    public GameObject menuContainer; // Référence à l'objet MenuContainer
    private bool isMenuVisible = false; // Indique si le menu est visible ou non



    // Start is called before the first frame update
    void Start()
    {
        LoadCSV();

        // Créer des listes uniques pour chaque filtre
        uniqueYears = GetUniqueYears();
        uniqueSpecializations = GetUniqueSpecializations();
        uniqueTransports = GetUniqueTransports();

        PopulateDropdowns();
        PopulateStudentList(students);
        yearDropdown.onValueChanged.AddListener(OnYearDropdownChanged);
        specializationDropdown.onValueChanged.AddListener(OnSpecializationDropdownChanged);
        transportDropdown.onValueChanged.AddListener(OnTransportDropdownChanged);
        menuContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadCSV()
    {
        // Charger le fichier CSV depuis Resources
        TextAsset csvData = Resources.Load<TextAsset>(csv);
        
        // Vérifier si le fichier a été trouvé
        if (csvData == null)
        {
            Debug.LogError("CSV file not found at " + csv);
            return;
        }

        // Lire chaque ligne du fichier CSV
        StringReader reader = new StringReader(csvData.text);
        List<string[]> rows = new List<string[]>();  // Liste pour stocker les lignes du CSV
        bool firstLine = true;  // Indicateur pour ignorer la première ligne si c'est l'entête

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            string[] fields = line.Split(',');  // Séparer les valeurs sur les virgules
            if (firstLine)
            {
                firstLine = false;  // Ignorer la première ligne si c'est l'entête
                continue;
            }

            rows.Add(fields);  // Ajouter les valeurs de chaque ligne à la liste
            students.Add(new Student(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], fields[6], fields[7], int.Parse(fields[8]), fields[9], fields[10], int.Parse(fields[11]), fields[12]));  // Créer un nouvel étudiant avec les valeurs de la ligne
        }

        // Afficher les lignes pour vérification
        foreach (var student in students)
        {
            Debug.Log("Nom: " + student.name + ", Genre: " + student.gender + ", 13h: " + student.thirteen + ", 14h: " + student.fourteen + ", 15h: " + student.fifteen + ", 16h: " + student.sixteen + ", 17h: " + student.seventeen + ", Cheveux: " + student.hair + ", Taille: " + student.height + ", Transport: " + student.transport + ", Vêtements: " + student.clothing + ", Année: " + student.year + ", Spécialisation: " + student.specialization);
        }
        Debug.Log("Taille de la liste students : " + students.Count);
    }

    // Fonction appelée par le bouton
    public void ToggleMenu()
    {
        isMenuVisible = !isMenuVisible; // Inverser l'état de visibilité
        menuContainer.SetActive(isMenuVisible); // Afficher ou masquer l'objet
    }

    void PopulateDropdowns()
    {
        yearDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> yearOptions = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Toutes les années")
        };
        yearOptions.AddRange(uniqueYears.Select(year => new TMP_Dropdown.OptionData(year.ToString())));
        yearDropdown.AddOptions(yearOptions);

        specializationDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> specOptions = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Toutes les spécialisations")
        };
        specOptions.AddRange(uniqueSpecializations.Select(spec => new TMP_Dropdown.OptionData(spec)));
        specializationDropdown.AddOptions(specOptions);

        transportDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> transportOptions = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Tous les transports")
        };
        transportOptions.AddRange(uniqueTransports.Select(transport => new TMP_Dropdown.OptionData(transport)));
        transportDropdown.AddOptions(transportOptions);
    }

    public void PopulateStudentList(List<Student> students)
    {
        foreach (var student in students)
        {
            // Instancier une cellule
            GameObject cell = Instantiate(studentCellPrefab, parentContainer);

            // Configurer les informations de la cellule
            StudentCell cellScript = cell.GetComponent<StudentCell>();
            Debug.Log(cellScript);
            cellScript.SetStudentInfo(student.name, $"Année: {student.year}\nSpécialisation: {student.specialization}");
        }
    }

    public void ClearStudentList()
    {
        Debug.Log("Parent " + parentContainer.childCount);
        foreach (Transform child in parentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // Appelée à chaque fois qu'un nouvel élément est ajouté
    public void AdjustContentHeight()
    {
        // Récupérer le nombre d'éléments enfants
        int itemCount = content.childCount;

        // Calculez la hauteur totale en fonction du nombre d'éléments
        float totalHeight = gridLayoutGroup.cellSize.y * itemCount + gridLayoutGroup.spacing.y * (itemCount - 1);

        // Ajuste la taille du Content (seulement pour l'axe vertical)
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    void ApplyFilters()
    {
        // Partir de la liste complète des étudiants
        filteredStudents = students;

        // Appliquer le filtre d'année
        int selectedYearIndex = yearDropdown.value;
        if (selectedYearIndex != 0) // 0 correspond à "Toutes les années"
        {
            int selectedYear = uniqueYears[selectedYearIndex - 1];
            filteredStudents = filteredStudents.Where(s => s.year == selectedYear).ToList();
        }

        // Appliquer le filtre de spécialisation
        int selectedSpecializationIndex = specializationDropdown.value;
        if (selectedSpecializationIndex != 0) // 0 correspond à "Toutes les spécialisations"
        {
            string selectedSpecialization = uniqueSpecializations[selectedSpecializationIndex - 1];
            filteredStudents = filteredStudents.Where(s => s.specialization == selectedSpecialization).ToList();
        }

        // Appliquer le filtre de transport
        int selectedTransportIndex = transportDropdown.value;
        if (selectedTransportIndex != 0) // 0 correspond à "Tous les transports"
        {
            string selectedTransport = uniqueTransports[selectedTransportIndex - 1];
            filteredStudents = filteredStudents.Where(s => s.transport == selectedTransport).ToList();
        }

        // Mettre à jour l'affichage
        ClearStudentList();
        PopulateStudentList(filteredStudents);
    }


    public void OnYearDropdownChanged(int index)
    {
        ApplyFilters();
    }

    public void OnSpecializationDropdownChanged(int index)
    {
        ApplyFilters();
    }

    public void OnTransportDropdownChanged(int index)
    {
        ApplyFilters();
    }

    // Obtenir toutes les années uniques
    List<int> GetUniqueYears()
    {
        List<int> uniqueYears = students.Select(student => student.year).Distinct().ToList();
        uniqueYears = uniqueYears.OrderBy(year => year).ToList();
        return uniqueYears;
    }

    // Obtenir toutes les spécialisations uniques
    List<string> GetUniqueSpecializations()
    {
        List<string> uniqueSpecializations = students.Select(student => student.specialization).Distinct().ToList();
        uniqueSpecializations = uniqueSpecializations.OrderBy(specialization => specialization).ToList();
        return uniqueSpecializations;
    }

    // Obtenir tous les modes de transport uniques
    List<string> GetUniqueTransports()
    {
        List<string> uniqueTransports = students.Select(student => student.transport).Distinct().ToList();
        uniqueTransports = uniqueTransports.OrderBy(transport => transport).ToList();
        return uniqueTransports;
    }

    void FilterByYear(int year)
    {
        var filteredStudents = students.Where(s => s.year == year).ToList();
        Debug.Log($"Étudiants en année {year}: {filteredStudents.Count}");
        foreach (var student in filteredStudents)
        {
            Debug.Log($"Nom: {student.name}, Année: {student.year}");
        }
        ClearStudentList();
        PopulateStudentList(filteredStudents);
    }

    void FilterBySpecialization(string specialization)
    {
        var filteredStudents = students.Where(s => s.specialization == specialization).ToList();
        Debug.Log($"Étudiants en spécialisation {specialization}: {filteredStudents.Count}");
        foreach (var student in filteredStudents)
        {
            Debug.Log($"Nom: {student.name}, Spécialisation: {student.specialization}");
        }
        ClearStudentList();
        PopulateStudentList(filteredStudents);
    }

    void FilterByTransport(string transport)
    {
        var filteredStudents = students.Where(s => s.transport == transport).ToList();
        Debug.Log($"Étudiants utilisant {transport} comme transport: {filteredStudents.Count}");
        foreach (var student in filteredStudents)
        {
            Debug.Log($"Nom: {student.name}, Transport: {student.transport}");
        }
        ClearStudentList();
        PopulateStudentList(filteredStudents);
    }
}