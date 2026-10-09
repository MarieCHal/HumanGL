# Comment HumanGL dessine le personnage

## À l'ouverture : préparer les shaders

```text
CubeRenderer
     |
     v
Crée ShaderProgram
     |
     v
Lit les fichiers cube.vert et cube.frag
     |
     v
Compile les deux shaders
  - ShaderType.VertexShader   : cube.vert
  - ShaderType.FragmentShader : cube.frag
     |
     v
Réunit les shaders dans un programme OpenGL
     |
     v
Le programme est prêt pour les dessins
```

## À chaque dessin : afficher un membre

```text
HumanGLWindow
Calcule la matrice du membre
     |
     v
CubeRenderer.Draw(model, view, projection, color)
     |
     +--> Utilise ShaderProgram
     |      - Use() : active les shaders
     |      - SetMatrix4() : envoie les matrices
     |      - SetVector3() : envoie la couleur
     |
     v
Sélectionne les sommets du cube avec GL.BindVertexArray
     |
     v
GL.DrawArrays : demande le dessin des 36 sommets
     |
     v
CARTE GRAPHIQUE
     |
     v
cube.vert : calcule la position de chaque sommet
     |
     v
OpenGL forme les triangles et leurs fragments
     |
     v
cube.frag : calcule la couleur des fragments
     |
     v
Résultat dans l'image en cours de dessin
     |
     v
Après le dessin de tous les membres : SwapBuffers()
     |
     v
Image affichée dans la fenêtre
```

## Le rôle des matrices

```text
Sommet du cube
     |
     v
model      : place et dimensionne le membre
     |
     v
view       : exprime sa position par rapport à la caméra
     |
     v
projection : prépare l'affichage avec la perspective
     |
     v
gl_Position dans cube.vert
```

## Exemple : dessiner un bras

`HumanGLWindow` calcule la matrice du bras, puis appelle `CubeRenderer.Draw`.
Celui-ci transmet les matrices et la couleur via `ShaderProgram`, puis lance le dessin.
La carte graphique utilise les deux shaders pour afficher le cube à la bonne place,
avec la bonne taille et la bonne couleur.

Le même cube et les mêmes shaders servent pour tous les membres.
Les matrices et la couleur changent à chaque dessin.
