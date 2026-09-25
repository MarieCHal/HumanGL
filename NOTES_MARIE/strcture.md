# Structure du projet

## Périmètre Mathématiques & Logique ( Marie )

| Fichier           | Rôle |
|-------------------|------|
| `Matrix4x4.cs`    | Calculs matriciels purs (prévoir un fichier `Vector3.cs` adjacent ou intégré) |
| `MatrixStack.cs`  | Pile LIFO pour la propagation des transformations |
| `Limb.cs`         | Membre de l'arbre hiérarchique avec calcul de la matrice locale `T × R × S` |
| `Animator.cs`     | Machine à états et équations de mouvement (`sin`, gravité) |

## Périmètre Pipeline Graphique

| Fichier                      | Rôle |
|------------------------------|------|
| `CubeRenderer.cs`            | Allocation du VBO/VAO du cube `1 × 1 × 1` et émission du `glDrawArrays` |
| `Shader.cs`                  | Compilation GLSL et injection de la matrice `u_ModelMatrix` |
| `vertex.glsl` / `fragment.glsl` | Shaders exécutés sur le GPU |

## Point d'entrée & orchestration

| Fichier       | Rôle |
|---------------|------|
| `Program.cs`  | Création de la fenêtre, gestion des événements clavier et boucle `while` principale |
