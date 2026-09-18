#version 410 core

// Vertex shader : programme execute par la carte graphique pour chaque sommet.
// Il transforme la position du sommet et transmet sa couleur au fragment shader.
// Avec le mode Core utilise par ce projet, les deux shaders sont necessaires.

// Position (x, y, z) et couleur (rouge, vert, bleu) fournies par CubeRenderer.
// Les locations correspondent aux attributs configures avec GL.VertexAttribPointer.
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 vertexColor;

// Matrices envoyees depuis le code C#, communes a tous les sommets du dessin.
// model : place l'objet dans la scene.
// view : exprime la scene par rapport a la camera.
// projection : applique la perspective.
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

// Couleur transmise a l'etape suivante, puis interpolee entre les sommets.
out vec3 faceColor;

void main()
{
    // Les transformations s'appliquent de droite a gauche : model, view, projection.
    // Le 1.0 ajoute la quatrieme coordonnee necessaire aux matrices 4x4.
    // gl_Position recoit la position transformee ; OpenGL calcule ensuite sa place a l'ecran.
    gl_Position = projection * view * model * vec4(position, 1.0);
    faceColor = vertexColor;
}
