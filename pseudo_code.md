
# STACKING DE MATRICES
Un membre parent hérite de la matrice de son parent:

Example: le trose bouge de deux, le bras bouge de 45 degrés mais herite du mouvement du torse 
    -> donc il bouge de 2 et il rotate de 45 deg. 

Pour avoir du coup le mouvement du bras on a multiplier sa matrice avec celle du torse on a 'push', 
maintenant pour bouger la jambe on veut se défaire du mouvement du bras donc on 'pop'

Terme technique: Matrix Stack -> litteralement avec des push et de pop
In c#: System.Collections.Generic


# PRINCIPE DE RENDER:
De base on donne au GPU seulement un cue 1 X 1 X 1 ce qui veut dire qu'il prends 1 unité de haut, 1 de large et 1 de profondeur. 
ensuite tout ce qu'on va donner au GPU vont être des Matrices qui vont être appliquée à ce cube. 
La matrice en l'occurence est calculée par rapport au coordonates 


# STRUCTURE DE LA DONNEE & FONCTIONS

## - le cube

type cube { // en gors un des membres
    positions:  8 points [int, int],
    <!-- matrix: 4[int, int, int, int], -->
    mouvement: X (b example 2m, 45 deg.)
}

## - la stack 

type stack {
    cube.mouvement [ ]
}



// updateAnnimation 
func ( )


Fonction DessinerNoeud(noeud: Node, pile: MatrixStack):
    pile.Push()[cite: 1]

    // On récupère la matrice qui contient la rotation recalculée avec sin / -sin
    MatriceMiseAJour = ObtenirMatriceLocale(noeud)

    // Fusion avec la matrice du parent
    pile.Multiply(MatriceMiseAJour)

    // On envoie la matrice au Shader : l'angle devient une transformation de sommets 3D
    Shader.SetUniform("u_ModelMatrix", pile.GetCurrent())
    DrawCube()[cite: 1]

    Pour chaque enfant dans noeud.enfants:
        DessinerNoeud(enfant, pile)

    pile.Pop()[cite: 1]