**HumanGL**, projet de l'école 42, est un moteur d'animation 3D hiérarchique en C# et OpenGL 4.0+ modélisant un personnage articulé à partir d'un unique cube 1x1x1.

**Points clés**

* **Mathématiques custom :** Moteur matriciel 4x4 (ordre $T \times R \times S$) et pile de matrices (`MatrixStack`) développés sans librairies tierces.
* **Squelette hiérarchique :** Arbre de nœuds (parent-enfant) pour propager automatiquement les mouvements le long du corps.
* **Maillage unique :** Réutilisation d'un seul VBO/VAO (cube 1x1x1) pour l'intégralité du rendu 3D.
* **Animations temps réel :** Machine à états (marche, saut parabolique, repos) et redimensionnement dynamique des membres au clavier.

**Tech Stack :** C# | OpenGL 4.0+ | GLSL | Algèbre Linéaire Custom
