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
        
        // Vérification des images de référence
        for (int i = 0; i < trackedImageManager.referenceLibrary.count; i++)
        {
            XRReferenceImage refImage = trackedImageManager.referenceLibrary[i];
            if (refImage.size.x <= 0 || refImage.size.y <= 0)
            {
                Debug.LogError($"ERREUR: L'image {refImage.name} a une taille invalide: {refImage.size} mètres. " +
                             "Veuillez définir une taille physique correcte dans la Reference Image Library.");
            }
            else
            {
                Debug.Log($"Image de référence {i}: {refImage.name}" +
                         $"\nTaille: {refImage.size} mètres" +
                         $"\nTexture: {(refImage.texture != null ? "OK" : "MANQUANTE")}");
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
        Debug.Log($"OnTrackedImagesChanged - Added: {eventArgs.added.Count}, Updated: {eventArgs.updated.Count}, Removed: {eventArgs.removed.Count}");

        foreach (var newImage in eventArgs.added)
        {
            Debug.Log($"Nouvelle image détectée: {newImage.referenceImage.name}");

            // Créer le cube juste au-dessus de l'image
            Vector3 position = newImage.transform.position;
            position.y += 0.05f; // 5cm au-dessus de l'image

            GameObject cube = Instantiate(cubePrefab, position, newImage.transform.rotation);
            
            // Ajuster la taille du cube à 5cm
            cube.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            
            // Activer le script Lean Touch
            var leanTouch = cube.GetComponent<MonoBehaviour>(); // Remplacez MonoBehaviour par le type exact de votre script
            if (leanTouch != null)
            {
                leanTouch.enabled = true;
            }

            // Changer la couleur du cube pour le rendre plus visible
            var renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red; // ou une autre couleur vive
            }

            cube.name = $"Cube_{newImage.referenceImage.name}";
            trackedImages[newImage.referenceImage.name] = cube;

            Debug.Log($"Cube créé:" +
                     $"\nPosition: {cube.transform.position}" +
                     $"\nScale: {cube.transform.localScale}" +
                     $"\nActive: {cube.activeSelf}" +
                     $"\nRenderer enabled: {renderer?.enabled}" +
                     $"\nLean Touch enabled: {leanTouch?.enabled}");
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if (trackedImages.TryGetValue(updatedImage.referenceImage.name, out GameObject cube))
            {
                Vector3 position = updatedImage.transform.position;
                position.y += 0.1f; // Maintenir le décalage

                Debug.Log($"Mise à jour cube:" +
                         $"\nPosition image: {updatedImage.transform.position}" +
                         $"\nPosition cube: {position}" +
                         $"\nTracking state: {updatedImage.trackingState}" +
                         $"\nCube actif: {cube.activeSelf}");

                cube.SetActive(updatedImage.trackingState == TrackingState.Tracking);
                if (updatedImage.trackingState == TrackingState.Tracking)
                {
                    cube.transform.position = position;
                    cube.transform.rotation = updatedImage.transform.rotation;
                }
            }
        }

        foreach (var removedImage in eventArgs.removed)
        {
            Debug.Log($"Image perdue: {removedImage.referenceImage.name}");
            if (trackedImages.TryGetValue(removedImage.referenceImage.name, out GameObject cube))
            {
                Destroy(cube);
                trackedImages.Remove(removedImage.referenceImage.name);
            }
        }
    }
} 