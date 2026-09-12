**HumanGL**, projet de l'école 42, est un moteur d'animation 3D hiérarchique en C# et OpenGL 4.0+ modélisant un personnage articulé à partir d'un unique cube 1x1x1.

**Points clés**

* **Mathématiques custom :** Moteur matriciel 4x4 (ordre $T \times R \times S$) et pile de matrices (`MatrixStack`) développés sans librairies tierces.
* **Squelette hiérarchique :** Arbre de nœuds (parent-enfant) pour propager automatiquement les mouvements le long du corps.
* **Maillage unique :** Réutilisation d'un seul VBO/VAO (cube 1x1x1) pour l'intégralité du rendu 3D.
* **Animations temps réel :** Machine à états (marche, saut parabolique, repos) et redimensionnement dynamique des membres au clavier.

**Tech Stack :** C# | OpenGL 4.0+ | GLSL | Algèbre Linéaire Custom

## Lancer le projet

### Prérequis

- Installer le **SDK .NET 8** (le runtime seul ne suffit pas).
- Disposer d’un environnement graphique prenant en charge **OpenGL 4.1**.
- Avoir une connexion Internet au premier lancement pour télécharger les dépendances.

Pour vérifier que le SDK est installé, ouvrez un terminal et tapez :

```sh
dotnet --list-sdks
```

Une version `8.0.xxx` doit apparaître. Si la commande `dotnet` est introuvable, installez le SDK puis rouvrez le terminal.

### Démarrer l’application

1. Ouvrez le dossier du projet dans votre éditeur.
2. Ouvrez un terminal à la **racine du dépôt**, dans le dossier qui contient ce `README.md` et le dossier `HumanGL`.
3. Lancez la commande suivante :

   ```sh
   dotnet run --project HumanGL/HumanGL.csproj
   ```

Cette commande télécharge les dépendances si nécessaire, compile le projet puis ouvre la fenêtre. Il n’est pas nécessaire d’installer OpenTK manuellement.

Si votre terminal est déjà dans le sous-dossier `HumanGL`, utilisez simplement :

```sh
dotnet run
```

### Résultat attendu

Une fenêtre intitulée **HumanGL**, de **800 × 600**, s’ouvre avec un fond sombre. Elle est vide pour le moment : le personnage sera ajouté plus tard.

Pour quitter, appuyez sur **Échap** lorsque la fenêtre est active, ou utilisez son bouton de fermeture.

### Compiler sans ouvrir la fenêtre

Depuis la racine du dépôt :

```sh
dotnet build HumanGL/HumanGL.csproj
```

## Point d’entrée

Le point d’entrée et la boucle d’affichage sont dans [`HumanGL/Program.cs`](HumanGL/Program.cs).
Le futur dessin du personnage sera ajouté dans `RenderFrame`.
Les fichiers de brouillon `Exemple.cs` et `Exemple2.cs` sont exclus de la compilation pour le moment.
