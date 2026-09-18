**HumanGL**, projet de l'école 42, est un moteur d'animation 3D hiérarchique en C# et OpenGL 4.0+ modélisant un personnage articulé à partir d'un unique cube 1x1x1.

**Objectifs du projet**

Ces fonctionnalités décrivent la cible finale. À ce stade, le rendu affiche un cube 3D avec une caméra contrôlée par les flèches ; la bibliothèque mathématique, le squelette et les animations restent à intégrer.

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

Une fenêtre intitulée **HumanGL**, de **800 × 600**, s’ouvre avec un fond sombre et un **cube coloré au centre**, vu de dessus et de côté. Ses six faces sont formées de douze triangles, dessinés en un seul appel OpenGL. Le test de profondeur masque les faces cachées et la projection suit le redimensionnement de la fenêtre. Le personnage sera ajouté plus tard.

Cliquez sur la fenêtre pour lui donner le focus, puis maintenez les flèches :

- **Gauche / droite** : tourner autour du cube horizontalement.
- **Haut / bas** : monter ou descendre autour du cube pour voir le dessus ou le dessous.
- **Échap** : quitter (ou utiliser le bouton de fermeture).

Le cube reste immobile et centré : c'est la caméra qui se déplace, à une distance constante de 3 unités. L'inclinaison est limitée à ±80° pour éviter de retourner la vue. La vitesse est de 90° par seconde, indépendamment du nombre d'images par seconde.

### Compiler sans ouvrir la fenêtre

Depuis la racine du dépôt :

```sh
dotnet build HumanGL/HumanGL.csproj
```

## Organisation du code

| Fichier | Rôle |
| --- | --- |
| [`Program.cs`](HumanGL/Program.cs) | Crée la fenêtre et lance l’application. |
| [`HumanGLWindow.cs`](HumanGL/HumanGLWindow.cs) | Gère la fenêtre, Échap, le redimensionnement et la boucle d’affichage. |
| [`Rendering/OrbitCamera.cs`](HumanGL/Rendering/OrbitCamera.cs) | Calcule la vue autour du cube à partir des mouvements demandés avec les flèches. |
| [`Rendering/CubeRenderer.cs`](HumanGL/Rendering/CubeRenderer.cs) | Contient les 36 sommets du cube unitaire et gère ses ressources et son dessin. |
| [`Rendering/ShaderProgram.cs`](HumanGL/Rendering/ShaderProgram.cs) | Charge, compile et active les shaders. |
| [`Shaders/cube.vert`](HumanGL/Shaders/cube.vert) | Transforme les sommets avec les matrices modèle, vue et projection. |
| [`Shaders/cube.frag`](HumanGL/Shaders/cube.frag) | Affiche la couleur de chaque face. |

Dans `HumanGLWindow`, `OnLoad` crée le rendu du cube, `OnRenderFrame` appelle son dessin à chaque image et `OnUnload` libère les ressources graphiques.
Les fichiers de shaders sont automatiquement copiés à côté de l’application lors de la compilation.

Les fichiers de brouillon `Exemple.cs` et `Exemple2.cs` sont exclus de la compilation pour le moment.

## Raccorder la partie mathématique

`CubeRenderer.Draw(model, view, projection)` attend trois tableaux de 16 `float`, rangés **colonne par colonne**. `ShaderProgram.SetMatrix4` les transmet à OpenGL sans transposition. Le vertex shader utilise des vecteurs colonnes :

```glsl
gl_Position = projection * view * model * vec4(position, 1.0);
```

Les positions du cube restent toujours entre `-0.5` et `+0.5`. La matrice `model` déterminera la taille, l'orientation et la position de chaque membre, sans recréer le maillage.

Pour afficher le cube dès maintenant, `HumanGLWindow` contient une matrice modèle identité et une projection de démonstration (champ vertical de 60°, plans proche/lointain à 0.1 et 10). `OrbitCamera` calcule la matrice de vue selon les angles horizontal et vertical, en radians. Ces calculs seront raccordés à votre bibliothèque maison. Aucune matrice OpenTK n'est utilisée ; son `Vector2i` sert uniquement à définir la taille de la fenêtre.
