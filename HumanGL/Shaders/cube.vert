#version 410 core

// Transforme chaque sommet pour l'affichage.

// Position du sommet fournie par CubeRenderer.
layout (location = 0) in vec3 position;

// Transformations envoyees par le C# : objet, camera et perspective.
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    // Applique dans l'ordre : objet, camera, perspective.
    gl_Position = projection * view * model * vec4(position, 1.0);
}
