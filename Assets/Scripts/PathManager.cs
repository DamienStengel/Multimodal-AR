using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation; // Ajout de la référence AR Foundation

public class PathManager : MonoBehaviour
{
    private static PathManager instance;
    public static PathManager Instance => instance;

    [SerializeField] private LineRenderer pathPrefab;
    [SerializeField] private float walkingSpeed = 1.4f; // vitesse moyenne de marche en m/s
    [SerializeField] private float heightPenalty = 1.5f; // multiplicateur pour les déplacements verticaux
    [SerializeField] private PathUI pathUI; // Référence à l'UI
    [SerializeField] private Material pathMaterial; // Ajoutez cette référence
    [SerializeField] private ARSessionOrigin arSessionOrigin; // Référence à l'AR Session Origin
    
    private GameObject startCube;
    private GameObject endCube;
    private LineRenderer currentPath;
    private Dictionary<string, Vector3> lastKnownPositions = new Dictionary<string, Vector3>();
    private GameObject pathParent; // Parent fixe pour tous les chemins
    private List<GameObject> pathLines = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (pathUI != null)
        {
            pathUI.HideClearButton(); // Cacher le bouton au démarrage
        }

        // Créer un parent fixe pour tous les chemins
        pathParent = new GameObject("PathsParent");
        pathParent.transform.SetParent(arSessionOrigin.transform);
        pathParent.transform.localPosition = Vector3.zero;
        pathParent.transform.localRotation = Quaternion.identity;
    }

    public void RegisterCubePosition(string cubeId, Vector3 position)
    {
        lastKnownPositions[cubeId] = position;
    }

    public void SetStartPoint(GameObject cube)
    {
        startCube = cube;
        UpdateUI();
        if (endCube != null)
        {
            DrawPath();
        }
    }

    public void SetEndPoint(GameObject cube)
    {
        if (cube == startCube) return;
        
        endCube = cube;
        UpdateUI();
        if (startCube != null)
        {
            DrawPath();
        }
    }

    private void DrawPath()
    {
        if (currentPath != null)
        {
            Destroy(currentPath.gameObject);
        }

        if (startCube == null || endCube == null) return;

        currentPath = Instantiate(pathPrefab, pathParent.transform);
        currentPath.positionCount = 3; // Utiliser 3 points pour créer un chemin en "escalier"
        currentPath.useWorldSpace = true;

        // Configurer le LineRenderer
        currentPath.startWidth = 0.01f;
        currentPath.endWidth = 0.01f;
        currentPath.material = pathMaterial;
        
        // Désactiver les ombres
        currentPath.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        currentPath.receiveShadows = false;

        // Calculer les positions
        Vector3 startPos = startCube.transform.position;
        Vector3 endPos = endCube.transform.position;
        
        // Calculer le point intermédiaire pour créer un chemin en "escalier"
        Vector3 midPos = new Vector3(
            startPos.x,
            Mathf.Max(startPos.y, endPos.y) + 0.02f,
            endPos.z
        );

        currentPath.SetPosition(0, startPos);
        currentPath.SetPosition(1, midPos);
        currentPath.SetPosition(2, endPos);

        // Calculer la distance en prenant en compte les déplacements verticaux
        float horizontalDistance = Vector2.Distance(
            new Vector2(startPos.x, startPos.z),
            new Vector2(endPos.x, endPos.z)
        );

        float verticalDistance = Mathf.Abs(endPos.y - startPos.y) * heightPenalty;
        float totalDistance = horizontalDistance + verticalDistance;

        // Convertir la distance en mètres (si nécessaire, selon l'échelle de votre scène)
        float distanceInMeters = totalDistance * 100; // Ajustez ce multiplicateur selon votre échelle

        // Calculer le temps en secondes
        float timeInSeconds = distanceInMeters / walkingSpeed;
        
        Debug.Log($"Distance horizontale: {horizontalDistance}m, " +
                  $"Distance verticale: {verticalDistance}m, " +
                  $"Distance totale: {distanceInMeters}m, " +
                  $"Temps estimé: {timeInSeconds}s");

        ShowPathInfo(timeInSeconds);
    }

    private void UpdateUI()
    {
        if (pathUI != null)
        {
            string startRoom = "---";
            string endRoom = "---";

            if (startCube != null)
            {
                TextMesh startText = startCube.GetComponentInChildren<TextMesh>();
                if (startText != null)
                {
                    startRoom = startText.text.Trim(); // Enlever les espaces
                    Debug.Log($"Nom salle départ trouvé: '{startRoom}'");
                }
            }

            if (endCube != null)
            {
                TextMesh endText = endCube.GetComponentInChildren<TextMesh>();
                if (endText != null)
                {
                    endRoom = endText.text.Trim(); // Enlever les espaces
                    Debug.Log($"Nom salle arrivée trouvé: '{endRoom}'");
                }
            }

            // Construire le texte avec des espaces explicites
            string displayText = string.Format("De : {0}\nVers : {1}", 
                string.IsNullOrEmpty(startRoom) ? "---" : startRoom,
                string.IsNullOrEmpty(endRoom) ? "---" : endRoom);

            Debug.Log($"Texte final envoyé à l'UI: '{displayText}'");
            pathUI.UpdateSelectionText(displayText);

            if (startCube != null || endCube != null)
            {
                pathUI.ShowClearButton();
            }
            else
            {
                pathUI.HideClearButton();
            }
        }
    }

    private void ShowPathInfo(float timeInSeconds)
    {
        if (timeInSeconds < 60)
        {
            // Moins d'une minute
            if (pathUI != null)
            {
                pathUI.UpdateTimeText($"{Mathf.CeilToInt(timeInSeconds)} secondes");
            }
        }
        else
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60);
            int seconds = Mathf.CeilToInt(timeInSeconds % 60);
            
            if (pathUI != null)
            {
                if (seconds > 0)
                {
                    pathUI.UpdateTimeText($"{minutes}min {seconds}s");
                }
                else
                {
                    pathUI.UpdateTimeText($"{minutes} minutes");
                }
            }
        }
    }

    public void ClearPath()
    {
        // Réinitialiser les cubes sélectionnés
        if (startCube != null)
        {
            var startSelector = startCube.GetComponent<CubeSelector>();
            if (startSelector != null)
                startSelector.Deselect();
        }
        if (endCube != null)
        {
            var endSelector = endCube.GetComponent<CubeSelector>();
            if (endSelector != null)
                endSelector.Deselect();
        }

        // Réinitialiser les références
        startCube = null;
        endCube = null;

        // Effacer le chemin visuel
        if (currentPath != null)
            Destroy(currentPath.gameObject);

        // Désactiver le mode navigation pour tous les cubes
        var cubeSelectors = FindObjectsOfType<CubeSelector>();
        foreach (var selector in cubeSelectors)
        {
            selector.SetNavigationMode(false);
        }

        UpdateUI();
        ClearPathLines();
    }

    public GameObject GetStartPoint()
    {
        return startCube;
    }

    public GameObject GetEndPoint()
    {
        return endCube;
    }

    public bool IsCubeSelected(GameObject cube)
    {
        return cube == startCube || cube == endCube;
    }

    // Méthode pour mettre à jour la position du chemin si nécessaire
    public void UpdatePathPosition()
    {
        if (currentPath != null && startCube != null && endCube != null)
        {
            Vector3 startPos = startCube.transform.position;
            Vector3 endPos = endCube.transform.position;
            
            Vector3 midPos = new Vector3(
                startPos.x,
                Mathf.Max(startPos.y, endPos.y) + 0.02f,
                endPos.z
            );

            currentPath.SetPosition(0, startPos);
            currentPath.SetPosition(1, midPos);
            currentPath.SetPosition(2, endPos);
        }
    }

    public List<GameObject> FindPath(GameObject start, GameObject end)
    {
        List<GameObject> path = new List<GameObject>();
        
        // Pour l'instant, on fait un chemin direct
        path.Add(start);
        path.Add(end);

        // Dessiner le chemin
        DrawPath(path);
        
        return path;
    }

    public float GetPathDistance(List<GameObject> path)
    {
        float distance = 0f;
        
        for (int i = 0; i < path.Count - 1; i++)
        {
            distance += Vector3.Distance(path[i].transform.position, path[i + 1].transform.position);
        }
        
        return distance;
    }

    public void DrawPath(List<GameObject> path)
    {
        // Effacer l'ancien chemin
        ClearPath();

        // Dessiner le nouveau chemin
        for (int i = 0; i < path.Count - 1; i++)
        {
            DrawLine(path[i].transform.position, path[i + 1].transform.position);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("PathLine");
        line.transform.parent = transform;
        
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.blue;
        lr.endColor = Color.blue;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        
        pathLines.Add(line);
    }

    private void ClearPathLines()
    {
        foreach (var line in pathLines)
        {
            if (line != null)
                Destroy(line);
        }
        pathLines.Clear();
    }
} 