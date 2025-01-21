using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RoomInfoPanel : UIPanel
{
    private static RoomInfoPanel instance;
    public static RoomInfoPanel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RoomInfoPanel>(true); // Le paramètre true permet de trouver même les objets inactifs
                if (instance == null)
                {
                    Debug.LogError("Aucun RoomInfoPanel trouvé dans la scène!");
                }
            }
            return instance;
        }
    }

    [Header("Room Info")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI nbPlaceText;

    [Header("Lists")]
    [SerializeField] private Transform objectListContent;
    [SerializeField] private Transform studentListContent;
    [SerializeField] private TextMeshProUGUI listItemPrefab;
    [SerializeField] private TMP_Dropdown timeSelector;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button startNavigationButton;

    [Header("Navigation")]
    [SerializeField] private Canvas navigationCanvas; // Référence au Canvas de navigation

    private string currentRoomId;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> spawnedStudents = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeLayoutComponents();
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLayoutComponents()
    {
        // Configuration du objectListContent
        if (objectListContent != null)
        {
            // Ajouter VerticalLayoutGroup si manquant
            var layoutGroup = objectListContent.GetComponent<VerticalLayoutGroup>();
            if (layoutGroup == null)
            {
                layoutGroup = objectListContent.gameObject.AddComponent<VerticalLayoutGroup>();
                Debug.Log("VerticalLayoutGroup ajouté à objectListContent");
            }
            
            // Configurer le LayoutGroup
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 5;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);

            // Ajouter ContentSizeFitter si manquant
            var contentSizeFitter = objectListContent.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = objectListContent.gameObject.AddComponent<ContentSizeFitter>();
                Debug.Log("ContentSizeFitter ajouté à objectListContent");
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else
        {
            Debug.LogError("objectListContent n'est pas assigné dans l'inspecteur");
        }

        // Configuration similaire pour studentListContent
        if (studentListContent != null)
        {
            var layoutGroup = studentListContent.GetComponent<VerticalLayoutGroup>();
            if (layoutGroup == null)
            {
                layoutGroup = studentListContent.gameObject.AddComponent<VerticalLayoutGroup>();
                Debug.Log("VerticalLayoutGroup ajouté à studentListContent");
            }
            
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 5;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);

            var contentSizeFitter = studentListContent.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = studentListContent.gameObject.AddComponent<ContentSizeFitter>();
                Debug.Log("ContentSizeFitter ajouté à studentListContent");
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else
        {
            Debug.LogError("studentListContent n'est pas assigné dans l'inspecteur");
        }
    }

    private void Start()
    {
        InitializeTimeSelector();
        InitializeButtons();
        // Suppression de Hide() ici car le panel est déjà désactivé dans Awake
    }

    private void InitializeTimeSelector()
    {
        timeSelector.ClearOptions();
        timeSelector.AddOptions(new List<string> { "13h", "14h", "15h", "16h", "17h" });
        timeSelector.onValueChanged.AddListener(OnTimeSelectorChanged);
    }

    private void InitializeButtons()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => {
                Hide();
            });
        }

        if (startNavigationButton != null)
        {
            startNavigationButton.onClick.AddListener(() => {
                StartNavigation();
            });
        }
    }

    private void StartNavigation()
    {
        var cube = GameObject.Find($"Cube_{currentRoomId}");
        if (cube != null)
        {
            // Définir le point de départ
            PathManager.Instance.SetStartPoint(cube);
            
            // Activer le mode navigation pour tous les cubes
            var cubeSelectors = FindObjectsOfType<CubeSelector>();
            foreach (var selector in cubeSelectors)
            {
                selector.SetNavigationMode(true);
            }

            // Activer l'UI de navigation
            if (navigationCanvas != null)
            {
                navigationCanvas.gameObject.SetActive(true);
            }

            Hide(); // Cacher le panel d'info
        }
    }

    public void ShowRoomInfo(string roomId)
    {
        Debug.Log($"Tentative d'affichage des informations pour la salle: {roomId}");
        
        currentRoomId = roomId;
        var roomInfo = RoomDataManager.Instance.GetRoomInfo(roomId);
        
        if (roomInfo != null)
        {
            Debug.Log($"Mise à jour des informations de la salle: {roomInfo.name}");
            roomNameText.text = roomInfo.name;
            nbPlaceText.text = $"Capacité: {roomInfo.capacity} places";

            Debug.Log($"Mise à jour de la liste d'équipements ({roomInfo.equipment.Length} items)");
            UpdateObjectsList(roomInfo.equipment);
            OnTimeSelectorChanged(timeSelector.value);

            gameObject.SetActive(true);
            
            Debug.Log($"Panel activé pour la salle: {roomId}");
        }
        else
        {
            Debug.LogWarning($"Aucune information trouvée pour la salle: {roomId}");
        }
    }

    private void UpdateObjectsList(string[] equipment)
    {
        if (objectListContent == null || listItemPrefab == null)
        {
            Debug.LogError("objectListContent ou listItemPrefab est null");
            return;
        }

        Debug.Log("Début de la mise à jour de la liste d'objets");

        ClearList(spawnedObjects);

        foreach (var item in equipment)
        {
            var objectItem = Instantiate(listItemPrefab, objectListContent);
            Debug.Log($"Item créé: {item}");

            // Configuration du RectTransform
            var rectTransform = objectItem.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.sizeDelta = new Vector2(0, 30);
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Configuration du texte
            objectItem.text = $"• {item}";
            objectItem.color = Color.black;
            objectItem.fontSize = 24;
            
            spawnedObjects.Add(objectItem.gameObject);
            
            Debug.Log($"Item configuré - Position: {rectTransform.anchoredPosition}, Size: {rectTransform.sizeDelta}, Text: {objectItem.text}");
        }

        // Forcer la mise à jour du layout
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(objectListContent as RectTransform);
    }

    private void OnTimeSelectorChanged(int hourIndex)
    {
        int hour = 13 + hourIndex;
        var students = RoomDataManager.Instance.GetStudentsInRoom(currentRoomId, hour);
        UpdateStudentsList(students);
    }

    private void UpdateStudentsList(List<string> students)
    {
        ClearList(spawnedStudents);

        foreach (var student in students)
        {
            var studentItem = Instantiate(listItemPrefab, studentListContent);
            studentItem.text = $"• {student}";
            spawnedStudents.Add(studentItem.gameObject);
        }
    }

    private void ClearList(List<GameObject> list)
    {
        foreach (var item in list)
        {
            if (item != null)
                Destroy(item);
        }
        list.Clear();
    }

    public override void Hide()
    {
        // Désélectionner le cube actuel
        var cube = GameObject.Find($"Cube_{currentRoomId}");
        if (cube != null)
        {
            var selector = cube.GetComponent<CubeSelector>();
            if (selector != null)
            {
                selector.Deselect();
            }
        }
        
        base.Hide();
    }
} 