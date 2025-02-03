# 🎯 Campus Navigator AR

## 📝 Description
Campus Navigator AR est une application de réalité augmentée conçue pour aider les nouveaux étudiants à s'orienter dans les locaux de Polytech. En utilisant la reconnaissance de QR codes placés sur une carte du campus, l'application fournit des informations détaillées sur les salles et permet une navigation intuitive.

## 👨‍💻 Parties implémentées

### Damien STENGEL
#### Core Functionalities
* Système de reconnaissance des QR codes
* Affichage des informations des salles
* Interface utilisateur principale
* Visualisation en réalité augmentée
#### Fonctionnalités additionnelles
* (**) Design a gesture that will allow the user to select any two locations, and have a visualization of the time of the trajectory using only foot
  * Système de sélection de points de départ/arrivée
  * Calcul et affichage du temps de trajet
  * Visualisation du chemin en RA

### Toa Foloka
* (**) Design of a menu that shows the whereabouts of students of interest by filtering by student year or specialization (IHM, MAM, AL, WD, etc…)
  * Interface de filtrage des étudiants
  * Système de tri par année/spécialisation/moyen de transport
  * Affichage dynamique des résultats
* Support des core fonctionnalités
  * Amélioration de l'interface utilisateur
  * Optimisation de l'expérience utilisateur

## ✅ Prérequis
Avant de commencer, assurez-vous d'avoir les éléments suivants installés :
* 🎮 **Unity Hub** avec Unity version 2021.3.3f1
* 📱 **Android Build Support** ou **iOS Build Support**
* 📦 **AR Foundation** package
* 📦 **ARCore XR Plugin** (Android) ou **ARKit XR Plugin** (iOS)

## 📂 Structure du projet
```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── QRCodeManager.cs
│   │   ├── RoomDataManager.cs
│   │   └── StudentData.cs
│   ├── UI/
│   │   ├── RoomInfoPanel.cs
│   │   └── PathUI.cs
│   └── Navigation/
│       ├── PathManager.cs
│       └── CubeSelector.cs
├── Resources/
│   ├── rooms_data.json
│   ├── students.csv
│   └── Sounds/
└── Scenes/
    └── Main.unity
```

## ⚙️ Configuration du projet
1. Cloner le projet
2. Ouvrir avec Unity 2021.3.3f1
3. Installer les packages requis via le Package Manager :
   * AR Foundation
   * ARCore XR Plugin (Android)
   * ARKit XR Plugin (iOS)

## 🚀 Utilisation
1. Imprimer la carte avec les QR codes
2. Lancer l'application sur un appareil mobile
3. Scanner un QR code pour voir les informations de la salle
4. Pour la navigation :
   * Toucher une première salle pour définir le point de départ
   * Toucher une seconde salle pour l'arrivée
   * Le chemin et le temps de trajet s'affichent automatiquement

## 📱 Build et déploiement
1. Ouvrir `File > Build Settings`
2. Sélectionner la plateforme (Android/iOS)
3. Vérifier que la scène principale est incluse
4. Cliquer sur `Build And Run`

## 🗺️ Ressources requises
* Carte du campus avec QR codes
* Fichier rooms_data.json dans le dossier Resources
* Fichier students.csv dans le dossier Resources

## 🔧 Dépannage
* Vérifier que les QR codes sont bien éclairés
* S'assurer que la caméra est stable lors de la détection
* Vérifier que tous les fichiers de données sont présents dans Resources
