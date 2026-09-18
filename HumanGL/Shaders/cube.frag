#version 410 core

// Fragment shader : programme execute par la carte graphique pour calculer
// la couleur d'un fragment (un morceau de triangle pouvant contribuer a un pixel).

// Couleur RGB du membre, envoyee depuis le C# avant chaque dessin.
// Toutes les faces de ce membre utilisent cette meme couleur.
uniform vec3 objectColor;

// Couleur de sortie : rouge, vert, bleu et alpha (opacite).
out vec4 color;

void main()
{
    // Applique la couleur du membre, avec une opacite de 1.0.
    color = vec4(objectColor, 1.0);
}
