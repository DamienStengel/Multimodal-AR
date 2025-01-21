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
        Debug.Log($"OnTrackedImagesChanged - Added: {eventArgs.added.Count}, Updated: {eventArgs.updated.Count}, Removed: {eventArgs.removed.Count}");

        foreach (var newImage in eventArgs.added)
        {
            // Vérification supplémentaire pour la détection
            Debug.Log($"Nouvelle image détectée:" +
                     $"\nNom: {newImage.referenceImage.name}" +
                     $"\nGUID: {newImage.referenceImage.guid}" +
                     $"\nTaille physique: {newImage.size} mètres" +
                     $"\nTaille texture: {newImage.referenceImage.texture.width}x{newImage.referenceImage.texture.height}");

            // Vérifier si l'image est déjà trackée
            if (trackedImages.ContainsKey(newImage.referenceImage.guid.ToString()))
            {
                Debug.LogWarning($"Image déjà trackée! GUID: {newImage.referenceImage.guid}");
                continue;
            }

            Vector3 position = newImage.transform.position;
            position.y += 0.0005f;
            GameObject cube = Instantiate(cubePrefab, position, newImage.transform.rotation);
            
            Vector2 imageSize = newImage.size;
            cube.transform.localScale = new Vector3(imageSize.x, 0.001f, imageSize.y);
            
            // Créer le TextMesh pour afficher le nom de la pièce
            GameObject textObj = new GameObject("RoomText");
            textObj.transform.SetParent(cube.transform);
            
            // Positionner le texte juste au-dessus du cube
            textObj.transform.localPosition = new Vector3(0, 1f, 0); // Le y est relatif à l'échelle du cube
            textObj.transform.localRotation = Quaternion.Euler(90, 0, 0); // Rotation pour que le texte soit lisible du dessus
            
            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = newImage.referenceImage.name;
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

            // Utiliser le GUID comme clé au lieu du nom
            cube.name = $"Cube_{newImage.referenceImage.guid}";
            trackedImages[newImage.referenceImage.guid.ToString()] = cube;

            Debug.Log($"Cube créé avec texte:" +
                     $"\nPosition: {cube.transform.position}" +
                     $"\nScale: {cube.transform.localScale}" +
                     $"\nTaille image: {imageSize}" +
                     $"\nNom de la pièce: {newImage.referenceImage.name}" +
                     $"\nActive: {cube.activeSelf}" +
                     $"\nRenderer enabled: {renderer?.enabled}");
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Utiliser le GUID comme clé
            if (trackedImages.TryGetValue(updatedImage.referenceImage.guid.ToString(), out GameObject cube))
            {
                Vector3 position = updatedImage.transform.position;
                position.y += 0.0005f;

                Debug.Log($"Mise à jour cube:" +
                         $"\nPosition image: {updatedImage.transform.position}" +
                         $"\nPosition cube: {position}" +
                         $"\nTracking state: {updatedImage.trackingState}");

                if (updatedImage.trackingState == TrackingState.Tracking)
                {
                    cube.SetActive(true);
                    cube.transform.position = position;
                    cube.transform.rotation = updatedImage.transform.rotation;
                }
                else
                {
                    cube.SetActive(false);
                }
            }
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Utiliser le GUID comme clé
            if (trackedImages.TryGetValue(removedImage.referenceImage.guid.ToString(), out GameObject cube))
            {
                Destroy(cube);
                trackedImages.Remove(removedImage.referenceImage.guid.ToString());
            }
        }
    }
} 