using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class QRCodeManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject cubePrefab;
    
    private Dictionary<string, GameObject> trackedImages = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Debug.Log("QRCodeManager: Awake");
        if (trackedImageManager == null)
        {
            Debug.LogError("QRCodeManager: trackedImageManager non assigné!");
        }
    }

    private void OnEnable()
    {
        Debug.Log("QRCodeManager: OnEnable - Ajout du listener");
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        Debug.Log("QRCodeManager: OnDisable - Retrait du listener");
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void Start()
    {
        Debug.Log($"QRCodeManager: Start - Référence Library contient {trackedImageManager.referenceLibrary.count} images");
        
        // Vérification détaillée des images de référence
        for (int i = 0; i < trackedImageManager.referenceLibrary.count; i++)
        {
            XRReferenceImage refImage = trackedImageManager.referenceLibrary[i];
            if (refImage.size.x <= 0 || refImage.size.y <= 0)
            {
                Debug.LogError($"ERREUR: L'image {refImage.name} a une taille invalide: {refImage.size} mètres.");
            }
            else
            {
                Debug.Log($"Image de référence {i}: {refImage.name}" +
                         $"\nTaille: {refImage.size} mètres" +
                         $"\nTexture: {(refImage.texture != null ? "OK" : "MANQUANTE")}" +
                         $"\nGUID: {refImage.guid}"); // Ajouter le GUID pour debug
            }
        }

        // Vérification de la configuration AR
        Debug.Log($"Configuration AR:" +
                  $"\nMax Moving Images: {trackedImageManager.maxNumberOfMovingImages}" +
                  $"\nTracking Enabled: {trackedImageManager.enabled}" +
                  $"\nRequest ID: {trackedImageManager.requestedMaxNumberOfMovingImages}" +
                  $"\nSubsystem Running: {trackedImageManager.subsystem?.running}");
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            if (trackedImages.ContainsKey(newImage.referenceImage.guid.ToString()))
            {
                continue;
            }

            // Calculer la position avec le cube au-dessus du QR code
            Vector3 position = newImage.transform.position;
            Vector2 imageSize = newImage.size;
            float cubeHeight = 0.001f; // Hauteur du cube
            position.y += cubeHeight / 2; // Positionner le centre du cube à la moitié de sa hauteur

            GameObject cube = Instantiate(cubePrefab, position, newImage.transform.rotation);
            
            // Définir l'échelle du cube
            cube.transform.localScale = new Vector3(imageSize.x, cubeHeight, imageSize.y);
            
            // Créer le TextMesh pour afficher le nom de la pièce
            GameObject textObj = new GameObject("RoomText");
            textObj.transform.SetParent(cube.transform);
            
            // Positionner le texte juste au-dessus du cube
            textObj.transform.localPosition = new Vector3(0, 1f, 0); // Le y est relatif à l'échelle du cube
            textObj.transform.localRotation = Quaternion.Euler(90, 0, 0); // Rotation pour que le texte soit lisible du dessus
            
            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = newImage.referenceImage.name;
            Debug.Log($"Création TextMesh pour la salle: {newImage.referenceImage.name}"); // Debug
            textMesh.fontSize = 100; // Ajustez selon vos besoins
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;
            
            // Ajuster l'échelle du texte en fonction de la taille du cube
            float textScale = Mathf.Min(imageSize.x, imageSize.y) * 0.5f;
            textObj.transform.localScale = new Vector3(textScale, textScale, textScale);

            // Activer le script Lean Touch
            var leanTouch = cube.GetComponent<MonoBehaviour>();
            if (leanTouch != null)
            {
                leanTouch.enabled = true;
            }

            // Changer la couleur du cube pour le rendre plus visible
            var renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
                // S'assurer que le matériau est opaque
                renderer.material.SetFloat("_Mode", 0); // 0 = Opaque
                renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                renderer.material.SetInt("_ZWrite", 1);
                renderer.material.DisableKeyword("_ALPHATEST_ON");
                renderer.material.DisableKeyword("_ALPHABLEND_ON");
                renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                renderer.material.renderQueue = -1;
            }

            cube.name = $"Cube_{newImage.referenceImage.name}";
            trackedImages[newImage.referenceImage.guid.ToString()] = cube;

            // Ajuster la taille du collider pour qu'il corresponde au cube
            BoxCollider boxCollider = cube.AddComponent<BoxCollider>();
            boxCollider.size = Vector3.one; // Le collider aura la même taille que l'échelle locale
            
            var selector = cube.AddComponent<CubeSelector>();
            
            // Log uniquement à la création initiale
            Debug.Log($"Nouveau cube créé: {cube.name}");
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if (trackedImages.TryGetValue(updatedImage.referenceImage.guid.ToString(), out GameObject cube))
            {
                Vector3 position = updatedImage.transform.position;
                float cubeHeight = cube.transform.localScale.y;
                position.y += cubeHeight / 2;

                if (updatedImage.trackingState == TrackingState.Tracking)
                {
                    cube.SetActive(true);
                    cube.transform.position = position;
                    cube.transform.rotation = updatedImage.transform.rotation;
                    
                    // Mettre à jour la position du chemin si ce cube fait partie du chemin
                    PathManager.Instance.UpdatePathPosition();
                }
                else
                {
                    cube.SetActive(false);
                }
            }
        }

        foreach (var removedImage in eventArgs.removed)
        {
            if (trackedImages.TryGetValue(removedImage.referenceImage.guid.ToString(), out GameObject cube))
            {
                Debug.Log($"Cube supprimé: {cube.name}");
                Destroy(cube);
                trackedImages.Remove(removedImage.referenceImage.guid.ToString());
            }
        }
    }
} 