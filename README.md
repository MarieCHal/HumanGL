# HumanGL

HumanGL est un projet de l’école 42 réalisé en **C#**, avec **OpenGL 4.1**, **OpenTK** et **GLSL**. Il affiche un personnage articulé en 3D, construit à partir d’un cube unitaire réutilisé pour chaque partie du corps.

Le projet comprend une bibliothèque mathématique maison, un squelette hiérarchique, des animations de repos, de marche et de saut, ainsi qu’une caméra contrôlable au clavier. La taille des membres peut être modifiée pendant l’exécution.

## Prérequis

- **SDK .NET 8**, installé automatiquement par `make install` s’il manque.
- **curl** et **bash** pour installer automatiquement le SDK (macOS/Linux).
- **make** pour utiliser les commandes du Makefile.
- Un environnement graphique compatible avec **OpenGL 4.1**.
- Une connexion Internet pour télécharger les dépendances lorsqu’elles ne sont pas déjà en cache.

Pour vérifier la présence du SDK :

```sh
dotnet --list-sdks
```

Une version `8.0.xxx` doit apparaître.

## Installation et lancement

Depuis la racine du dépôt, dans le dossier contenant le Makefile :

```sh
make install
make
```

`make install` installe le SDK .NET 8 s’il manque, puis restaure les packages NuGet, dont OpenTK et ses dépendances. Le SDK est téléchargé avec le [script officiel Microsoft](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script) dans `.dotnet/`, sans droits administrateur. Les commandes du Makefile utilisent automatiquement ce SDK local lorsqu’il est présent.

`make` compile le projet puis ouvre une fenêtre **HumanGL** de **800 × 600**, avec le personnage sur un fond sombre. Les dépendances sont également restaurées au lancement si nécessaire.

| Commande | Action |
| --- | --- |
| `make` ou `make run` | Compiler et lancer l’application. |
| `make install` | Installer le SDK .NET 8 si nécessaire et restaurer les dépendances NuGet. |
| `make build` | Compiler sans ouvrir la fenêtre. |
| `make clean` | Nettoyer les fichiers générés par la compilation. |

Sans Makefile, le lancement depuis la racine reste possible :

```sh
dotnet run --project HumanGL/HumanGL.csproj
```

## Contrôles

Cliquez dans la fenêtre pour lui donner le focus.

| Touche | Action |
| --- | --- |
| `I` | Revenir à la pose de repos. |
| `W` | Activer l’animation de marche sur place. |
| `Espace` | Déclencher un saut, suivi d’un retour au repos. |
| `T` | Sélectionner la partie du corps suivante à redimensionner. |
| `R` | Agrandir la partie sélectionnée. |
| `F` | Réduire la partie sélectionnée. |
| Flèches gauche / droite | Tourner la caméra horizontalement. |
| Flèches haut / bas | Tourner la caméra verticalement. |
| `Échap` | Quitter l’application. |

La sélection initiale est le bras supérieur gauche. La touche `T` parcourt la tête, le torse, le bras et l’avant-bras gauches, la cuisse et le mollet gauches, puis le bras et l’avant-bras droits. La partie sélectionnée est indiquée dans le titre de la fenêtre et dans le terminal après un changement de sélection.

## Fonctionnement

Le squelette est organisé en arbre : les transformations d’un parent se transmettent à ses enfants. Une pile de matrices permet de parcourir cette hiérarchie et de dessiner chaque membre avec le même maillage de cube.

Les transformations utilisent les classes `Vector3`, `Matrix4x4` et `MatrixStack` du projet, dans l’ordre translation, rotation, puis mise à l’échelle. Les matrices sont transmises aux shaders colonne par colonne.

Le test de profondeur masque les surfaces cachées. La projection s’adapte au redimensionnement de la fenêtre, et les fichiers GLSL sont copiés automatiquement à côté de l’application lors de la compilation.

## Organisation du code

| Fichier | Rôle |
| --- | --- |
| [Program.cs](HumanGL/Program.cs) | Point d’entrée de l’application. |
| [HumanGLWindow.cs](HumanGL/HumanGLWindow.cs) | Fenêtre, clavier, boucle de rendu et parcours du squelette. |
| [Models/Limb.cs](HumanGL/Models/Limb.cs) | Construction du personnage, hiérarchie et redimensionnement des membres. |
| [Animation/Animator.cs](HumanGL/Animation/Animator.cs) | Animations de repos, de marche et de saut. |
| [Mathematics/Vector3.cs](HumanGL/Mathematics/Vector3.cs) | Vecteur à trois composantes. |
| [Mathematics/Matrix4x4.cs](HumanGL/Mathematics/Matrix4x4.cs) | Matrices et transformations 3D. |
| [Mathematics/MatrixStack.cs](HumanGL/Mathematics/MatrixStack.cs) | Pile de matrices pour le rendu hiérarchique. |
| [Rendering/OrbitCamera.cs](HumanGL/Rendering/OrbitCamera.cs) | Caméra orbitale contrôlée par les flèches. |
| [Rendering/CubeRenderer.cs](HumanGL/Rendering/CubeRenderer.cs) | Maillage du cube et rendu OpenGL. |
| [Rendering/ShaderProgram.cs](HumanGL/Rendering/ShaderProgram.cs) | Chargement et gestion des shaders. |
| [Shaders/cube.vert](HumanGL/Shaders/cube.vert) | Transformation des sommets. |
| [Shaders/cube.frag](HumanGL/Shaders/cube.frag) | Couleur des surfaces. |
