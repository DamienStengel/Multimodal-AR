# Campus Navigator AR

## Description
Campus Navigator AR est une application de réalité augmentée développée sous Unity qui permet aux utilisateurs de naviguer et d'obtenir des informations sur les salles d'un établissement d'enseignement en utilisant des QR codes. L'application fonctionne avec une carte physique de l'établissement sur laquelle sont positionnés des QR codes pour chaque salle.

## Fonctionnalités
- Scan de QR codes pour identifier les salles
- Affichage d'informations détaillées sur chaque salle :
  - Nom et capacité
  - Liste des équipements
  - Type de salle (cours, informatique, laboratoire, etc.)
- Visualisation des étudiants présents dans la salle aux différentes heures (13h-17h)
- Système de navigation entre les salles avec :
  - Calcul de chemin
  - Estimation du temps de parcours
  - Visualisation du trajet en réalité augmentée
- Visualisation de la liste des étudiants que l'ont peu filtrer avec l'année/filière/moyen de locomotion
## Prérequis
- Unity 2021.3.3f1
- AR Foundation (version compatible avec Unity 2021.3.3f1)
- Un appareil mobile compatible AR (Android ou iOS)
- La carte physique avec les QR codes des salles

## Installation

1. Cloner le projet
2. Ouvrir le projet dans Unity 2021.3.3f1
3. S'assurer que les packages suivants sont installés via le Package Manager :
   - AR Foundation
   - ARCore XR Plugin (pour Android)
   - ARKit XR Plugin (pour iOS)
4. Vérifier que tous les fichiers nécessaires sont présents dans le dossier Resources :
   - rooms_data.json
   - students.csv
   - select_sound.wav (ou .mp3)

## Structure du projet

### Scripts principaux
- `QRCodeManager.cs` : Gère la détection et le traitement des QR codes
- `RoomDataManager.cs` : Gère les données des salles et des étudiants
- `RoomInfoPanel.cs` : Interface utilisateur pour l'affichage des informations des salles
- `PathManager.cs` : Gère le système de navigation et de calcul des chemins

### Données
- `rooms_data.json` : Contient les informations sur toutes les salles
- `students.csv` : Contient les emplois du temps des étudiants

### Modèles de données
- `RoomData.cs` : Structure des données des salles
- `StudentData.cs` : Structure des données des étudiants

## Utilisation

1. Lancer l'application sur un appareil mobile
2. Pointer la caméra vers la carte de l'établissement
3. Les QR codes sont automatiquement détectés et des cubes 3D apparaissent au-dessus des salles
4. Toucher un cube pour afficher les informations de la salle correspondante
5. Pour la navigation :
   - Sélectionner une salle de départ
   - Choisir une salle d'arrivée
   - Le chemin et le temps estimé s'affichent en réalité augmentée

## Carte et QR Codes

L'application fonctionne avec une carte spécifique de l'établissement où les QR codes sont placés sur les différentes salles. La carte doit inclure :
- Les salles de cours (Room01, Room02, etc.)
- Les espaces communs (bibliothèque, cafétéria, etc.)
- Les entrées et sorties
- Les QR codes correspondant à chaque salle
![TIM-Map_Polytech](https://github.com/user-attachments/assets/8db7b93b-faa3-4ce6-a114-c918a980eb0d)

## Notes techniques

- Les QR codes doivent être enregistrés dans la bibliothèque de référence d'AR Foundation
- La taille physique des QR codes doit correspondre aux paramètres définis dans l'application
- Les données des salles peuvent être modifiées dans le fichier rooms_data.json
- Les emplois du temps des étudiants peuvent être mis à jour via le fichier students.csv
