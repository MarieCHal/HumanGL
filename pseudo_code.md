# Structure de la donnée

## Vecteur 3D

Pour les positions, rotations et dimensions.

```
Structure Vector3:
    x: Float
    y: Float
    z: Float
```

## Matrice 4x4

Pour représenter les transformations spatiales.

```
Structure Matrix4x4:
    m: Tableau[4][4] de Float
```

## Pile de matrices

Structure LIFO pour gérer l'héritage parent/enfant.

```
Structure MatrixStack:
    pile: Liste/Stack de Matrix4x4

    Fonction Push():
        // Duplique la matrice au sommet et l'empile

    Fonction Pop():
        // Supprime la matrice au sommet pour revenir au parent

    Fonction GetCurrent() -> Matrix4x4:
        // Retourne la matrice actuellement au sommet
```

## Nœud du personnage

Chaque membre du corps.

```
Structure Node:
    nom: String                          // ex: "BrasGauche"
    positionLocale: Vector3              // Offset d'articulation fixe par rapport au parent
    rotationLocale: Vector3              // Angles (X, Y, Z) modifiés par la boucle d'animation
    scaleLocale: Vector3                 // Taille du membre (Scale X, Y, Z)
    enfants: Liste de Node               // Membres rattachés directement sous ce nœud
```

### Rôle de chaque composant

| Champ            | Rôle |
|------------------|------|
| `positionLocale` | Placé une fois pour toutes à l'initialisation (ex: l'épaule à `X = -0.5`, `Y = 0.5` du torse) |
| `rotationLocale` | Seule variable modifiée dynamiquement par la boucle de marche ou de saut |
| `scaleLocale`    | Redimensionne n'importe quelle partie du corps sans casser les articulations des enfants |
| `enfants`        | Permet la traversée récursive pour le rendu (quand le torse bouge, la boucle descend dans les bras et les jambes) |

---

# Instanciation de la donnée

## Helper de création

```
Function CreateNode(name: String, pos: Vector3, rot: Vector3, scale: Vector3) -> Node:
    node = New Node
    node.name = name
    node.localPosition = pos
    node.localRotation = rot
    node.localScale = scale
    node.children = EmptyList()
    Return node
```

## Instanciation du squelette

Chaque membre est décalé par rapport au cube originel `1x1x1`.  
Tous les membres découlent du parent `"Torso"`.

```
Function InitCharacter() -> Node:
    // 1. Torse (racine principale, forme rectangle)
    torso = CreateNode("Torso", Vector3(0.0, 0.0, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(1.5, 2.0, 1.0))

    // 2. Tête (décalée vers le haut du torse)
    head = CreateNode("Head", Vector3(0.0, 1.3, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.8, 0.8, 0.8))
    torso.children.Add(head)

    // 3. Bras gauche (épaule -> avant-bras)
    leftUpperArm = CreateNode("LeftUpperArm", Vector3(-1.0, 0.8, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.4, 1.0, 0.4))
    leftForearm  = CreateNode("LeftForearm",  Vector3(0.0, -1.0, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.35, 0.9, 0.35))
    leftUpperArm.children.Add(leftForearm)
    torso.children.Add(leftUpperArm)

    // 4. Bras droit (épaule -> avant-bras)
    rightUpperArm = CreateNode("RightUpperArm", Vector3(1.0, 0.8, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.4, 1.0, 0.4))
    rightForearm  = CreateNode("RightForearm",  Vector3(0.0, -1.0, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.35, 0.9, 0.35))
    rightUpperArm.children.Add(rightForearm)
    torso.children.Add(rightUpperArm)

    // 5. Jambe gauche (hanche -> mollet)
    leftThigh = CreateNode("LeftThigh", Vector3(-0.5, -1.2, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.5, 1.2, 0.5))
    leftCalf  = CreateNode("LeftCalf",  Vector3(0.0, -1.2, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.45, 1.1, 0.45))
    leftThigh.children.Add(leftCalf)
    torso.children.Add(leftThigh)

    // 6. Jambe droite (hanche -> mollet)
    rightThigh = CreateNode("RightThigh", Vector3(0.5, -1.2, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.5, 1.2, 0.5))
    rightCalf  = CreateNode("RightCalf",  Vector3(0.0, -1.2, 0.0), Vector3(0.0, 0.0, 0.0), Vector3(0.45, 1.1, 0.45))
    rightThigh.children.Add(rightCalf)
    torso.children.Add(rightThigh)

    // On retourne uniquement le nœud racine : il donne accès à tout l'arbre
    Return torso
```

---

# Rendu

## Matrice locale TRS

Calcule l'offset du membre à partir de son parent :  
`Translation * Rotation * Scale`.

```
Function GetLocalMatrix(node: Node) -> Matrix4x4:
    translationMat = Matrix4x4.Translation(node.localPosition)
    rotationMat    = Matrix4x4.RotationXYZ(node.localRotation)  // Combine RotX, RotY, RotZ
    scaleMat       = Matrix4x4.Scale(node.localScale)

    Return translationMat * rotationMat * scaleMat
```

## Mise à jour de l'animation

Mise à jour des angles selon l'état.

```
Function UpdateAnimation(root: Node, time: Float, state: String):
    If state == "WALK":
        // Angle oscillant entre -30° et +30°
        walkAngle = Sin(time * 5.0) * 30.0

        FindNode(root, "LeftUpperArm").localRotation.x  = walkAngle
        FindNode(root, "RightUpperArm").localRotation.x = -walkAngle
        FindNode(root, "LeftThigh").localRotation.x     = -walkAngle
        FindNode(root, "RightThigh").localRotation.x    = walkAngle

        // Légère flexion des coudes / genoux
        FindNode(root, "LeftForearm").localRotation.x  = Abs(walkAngle) * 0.5
        FindNode(root, "RightForearm").localRotation.x = Abs(-walkAngle) * 0.5

    Else If state == "JUMP":
        jumpHeight = Abs(Sin(time * 3.0)) * 1.5
        root.localPosition.y = jumpHeight

        // Repli des jambes pendant le saut
        FindNode(root, "LeftThigh").localRotation.x  = -45.0
        FindNode(root, "RightThigh").localRotation.x = -45.0

    Else If state == "IDLE":
        ResetPose(root)
```

## Parcours récursif et rendu

Utilisation de la `MatrixStack`.

```
Function RenderNode(node: Node, stack: MatrixStack, shader: ShaderProgram):
    // 1. Empiler pour isoler la branche
    stack.Push()

    // 2. Multiplier la matrice du parent par la matrice locale
    localMatrix = GetLocalMatrix(node)
    stack.Multiply(localMatrix)

    // 3. Transmettre la matrice monde au GPU et dessiner le cube 1x1x1
    shader.SetUniformMatrix4("u_ModelMatrix", stack.GetCurrent())
    DrawCube()

    // 4. Dessiner récursivement tous les enfants
    For Each child In node.children:
        RenderNode(child, stack, shader)

    // 5. Dépiler pour restaurer l'état du parent
    stack.Pop()
```

## Boucle principale

```
Function MainLoop(rootNode: Node, stack: MatrixStack, shader: ShaderProgram):
    While WindowIsOpen():
        time = GetTimeInSeconds()

        UpdateAnimation(rootNode, time, currentState)
        ClearScreen()
        RenderNode(rootNode, stack, shader)
        SwapBuffers()
```

---

# Fonctions fondamentales 4x4

## Identité

Ne modifie aucun point.

```
Function Matrix4x4_Identity() -> Matrix4x4:
    mat = New Matrix4x4
    // 1.0 sur la diagonale, 0.0 partout ailleurs
    mat.m[0][0] = 1.0
    mat.m[1][1] = 1.0
    mat.m[2][2] = 1.0
    mat.m[3][3] = 1.0
    Return mat
```

## Translation

```
Function Matrix4x4_Translation(pos: Vector3) -> Matrix4x4:
    mat = Matrix4x4_Identity()
    mat.m[0][3] = pos.x
    mat.m[1][3] = pos.y
    mat.m[2][3] = pos.z
    Return mat
```

## Échelle (Scale)

```
Function Matrix4x4_Scale(scale: Vector3) -> Matrix4x4:
    mat = Matrix4x4_Identity()
    mat.m[0][0] = scale.x
    mat.m[1][1] = scale.y
    mat.m[2][2] = scale.z
    Return mat
```

## Produit matriciel

Ligne × colonne.

```
Function Matrix4x4_Multiply(A: Matrix4x4, B: Matrix4x4) -> Matrix4x4:
    result = New Matrix4x4
    For row From 0 To 3:
        For col From 0 To 3:
            result.m[row][col] = A.m[row][0] * B.m[0][col] +
                                A.m[row][1] * B.m[1][col] +
                                A.m[row][2] * B.m[2][col] +
                                A.m[row][3] * B.m[3][col]
    Return result
```

## Implémentation de la pile

La pile gère le stockage et la multiplication en mémoire au fil de la récursion.

```
Structure MatrixStack:
    stackList: List of Matrix4x4

    Function Init():
        stackList.Clear()
        stackList.Add(Matrix4x4_Identity())

    Function Push():
        current = GetCurrent()
        stackList.Add(current)

    Function Pop():
        If stackList.Count > 1:
            stackList.RemoveAt(stackList.Count - 1)

    Function Multiply(localMat: Matrix4x4):
        topIndex = stackList.Count - 1
        stackList[topIndex] = Matrix4x4_Multiply(stackList[topIndex], localMat)

    Function GetCurrent() -> Matrix4x4:
        Return stackList[stackList.Count - 1]
```

---

# Parcours d'arbre et utilitaires

## Recherche d'un nœud

```
Function FindNode(currentNode: Node, nameToFind: String) -> Node:
    If currentNode.name == nameToFind:
        Return currentNode

    For Each child In currentNode.children:
        found = FindNode(child, nameToFind)
        If found != Null:
            Return found

    Return Null
```

## Remise à zéro de la pose

```
Function ResetPose(node: Node):
    node.localRotation = Vector3(0.0, 0.0, 0.0)
    If node.name == "Torso":
        node.localPosition = Vector3(0.0, 0.0, 0.0)

    For Each child In node.children:
        ResetPose(child)
```

---

# Gestion des entrées

Redimensionnement dynamique et changement d'état.

```
Function HandleInput(rootNode: Node, key: String):
    If key == "KEY_W":
        currentState = "WALK"

    Else If key == "KEY_SPACE":
        currentState = "JUMP"
        jumpStartTime = GetTimeInSeconds()

    Else If key == "KEY_I":
        currentState = "IDLE"

    Else If key == "KEY_UP":
        // Agrandir l'avant-bras gauche
        forearm = FindNode(rootNode, "LeftForearm")
        If forearm != Null:
            forearm.localScale.y = forearm.localScale.y + 0.1

    Else If key == "KEY_DOWN":
        // Réduire l'avant-bras gauche
        forearm = FindNode(rootNode, "LeftForearm")
        If forearm != Null:
            forearm.localScale.y = Max(0.1, forearm.localScale.y - 0.1)
```
