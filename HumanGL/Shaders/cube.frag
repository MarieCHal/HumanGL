#version 410 core

// Calcule la couleur de chaque fragment.

// Couleur du membre envoyee par le C#.
uniform vec3 objectColor;

// Couleur finale (RGB et opacite).
out vec4 color;

void main()
{
    // Opacite maximale : 1.0.
    color = vec4(objectColor, 1.0);
}
