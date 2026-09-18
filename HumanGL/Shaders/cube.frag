#version 410 core

// Fragment shader : programme execute par la carte graphique pour calculer
// la couleur d'un fragment (un morceau de triangle pouvant contribuer a un pixel).

// Couleur recue du vertex shader, interpolee entre les sommets du triangle.
// Dans notre cube, les sommets d'une face ont la meme couleur : elle reste uniforme.
in vec3 faceColor;

// Couleur de sortie : rouge, vert, bleu et alpha (opacite).
out vec4 color;

void main()
{
    // Conserve la couleur recue et fixe l'alpha a 1.0 : completement opaque.
    color = vec4(faceColor, 1.0);
}
