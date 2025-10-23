# 🏭 Factory Frenzy

> **Projet final CESI — Unity Multijoueur & VR**  
> Conçu dans le cadre du module Unity/VR pour démontrer les fonctionnalités clés de **Factory Frenzy**, un jeu inspiré de *Fall Guys* mêlant **course multijoueur PC** et **éditeur de niveaux en réalité virtuelle**.

---

## 🌐 À propos du projet

**Factory Frenzy** est un jeu où plusieurs joueurs s’affrontent sur un parcours semé d’obstacles, composé de plateformes et de pièges dynamiques.  
Le projet se décline en **deux applications distinctes mais interconnectées** :

| Application | Description |
|--------------|-------------|
| 🖥️ **PC Multijoueur** | Permet aux joueurs de participer à une course compétitive à la troisième personne. |
| 🕶️ **Éditeur VR** | Permet de **créer**, **placer**, et **exporter** des niveaux complets au format JSON. Ces niveaux peuvent ensuite être joués sur PC. |

Le lien entre ces deux mondes est le fichier **`Level.json`**, généré en VR et importé dans la version PC.

---

## 🧩 Fonctionnalités présentes

### 🕶️ Fonctionnalités principales
| ID | Fonctionnalité | États |
|----|----------------|-------|
| **VR-FP1-1** | Téléportation de l'avatar RV | Fonctionnelle ✅ |
| **VR-FP1-2** | Séléctionner et créer des éléments dans une UI | Fonctionnelle ✅ |
| **VR-FP1-3** | Attraper et placer des objets | Fonctionnelle ✅ |
| **VR-FP1-4** | Supprimer des éléments du niveau | Fonctionnelle ✅ |
| **VR-FP1-5** | Export JSON du niveau | Fonctionnelle ✅ |
| **VR-FP1-6** | Sons & vibrations | Fonctionnelle ✅ |

---

### 🕶️ Fonctionnalités secondaires
| ID | Fonctionnalité | États |
|----|----------------|-------|
| **VR-FP2-1** | Placement et snap | En cours 💭 |
| **VR-FP2-2** | Verouiller un éléments | Non disponible ❌ |
| **VR-FP2-3** | Paramétrage des éléments de la plateforme mobile | Non disponible ❌ |

---

## ⚙️ Technologies utilisées

| Catégorie | Outils |
|------------|--------|
| 🎮 Moteur | Unity 2022.3.8f1 |
| 💻 Langage | C# |
| 🌐 Réseau | Unity Netcode for GameObjects |
| 📦 Sérialisation | JsonUtility |
| 🧠 FSM | Finite State Machine (pièges & logique de tir) |
| 🕶️ VR | XR Interaction Toolkit (Meta QUEST 3) |
| 🧰 IDE | Visual Studio Community |

---

## 🧩 Ressources et installation

### 🔧 Logiciels requis
- Unity **2022.3.1 à 2022.3.8**
- Visual Studio Community (avec support C# / Unity)
- Unity Hub  


### 🕹️ Installation
- Télécharger la dernière release sur le Github et dé-zipper le fichier. 
- Lancer l'exécutable **unity-project.exe**
