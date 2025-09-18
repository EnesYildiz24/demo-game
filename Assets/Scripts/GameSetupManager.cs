using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [Header("Setup Settings")]
    public bool autoSetup = true;
    public Material defaultMaterial;
    public Material crystalMaterial;

    [Header("Kenney Prefabs (optional)")]
    public GameObject kenneyWallPrefab;           // e.g. Assets/KenneyBuildingKit/Models/FBX format/wall.fbx
    public GameObject kenneyDoorFramePrefab;      // e.g. Assets/KenneyBuildingKit/Models/FBX format/wall-doorway-square.fbx
    public GameObject kenneyDoorPrefab;           // e.g. Assets/KenneyBuildingKit/Models/FBX format/door-rotate-square-a.fbx
    
    void Start()
    {
        if (autoSetup)
        {
            // Kleine Verzögerung um Transform-System Fehler zu vermeiden
            StartCoroutine(DelayedSetup());
        }
    }

    private System.Collections.IEnumerator DelayedSetup()
    {
        // Mehrere Frames warten für bessere Stabilität
        for (int i = 0; i < 3; i++)
        {
            yield return null;
        }

        SetupCompleteGame();
    }
    
    [ContextMenu("Setup Complete Game")]
    public void SetupCompleteGame()
    {
        Debug.Log("Setting up complete Quantum Puzzle Game...");

        // Starte Setup als Coroutine für bessere Stabilität
        StartCoroutine(SetupCoroutine());
    }

    private System.Collections.IEnumerator SetupCoroutine()
    {
        // Phase 1: Core Managers
        CreateCoreManagers();
        yield return null;

        // Phase 2: Player
        CreatePlayer();
        yield return null;

        // Phase 3: AudioManager
        CreateAudioManager();
        yield return null;

        // Phase 4: Environment
        CreateEnvironment();
        yield return null;

        // Phase 5: Puzzle Level
        CreateFirstPuzzleLevel();
        yield return null;

        // Phase 6: Physics Zones
        CreatePhysicsZones();
        yield return null;

        // Phase 7: Teleporters
        CreateTeleporters();
        yield return null;

        // Phase 8: UI
        CreateUI();
        yield return null;

        // Phase 8: Logo
        CreateSetupHintLogo();

        Debug.Log("Complete game setup finished!");
    }
    
    private void CreateCoreManagers()
    {
        // GameManager
        if (GameObject.Find("GameManager") == null)
        {
            GameObject gameManagerGO = new GameObject("GameManager");
            gameManagerGO.AddComponent<GameManager>();
        }
        
        // SaveSystem
        if (GameObject.Find("SaveSystem") == null)
        {
            GameObject saveSystemGO = new GameObject("SaveSystem");
            saveSystemGO.AddComponent<SaveSystem>();
        }
        
        // LevelManager
        if (GameObject.Find("LevelManager") == null)
        {
            GameObject levelManagerGO = new GameObject("LevelManager");
            levelManagerGO.AddComponent<LevelManager>();
        }
        
        // UIManager
        if (GameObject.Find("UIManager") == null)
        {
            GameObject uiManagerGO = new GameObject("UIManager");
            uiManagerGO.AddComponent<UIManager>();
        }
        
    }
    
    private void CreateAudioManager()
    {
        // AudioManager (created after player to avoid null reference)
        if (GameObject.Find("AudioManager") == null)
        {
            GameObject audioManagerGO = new GameObject("AudioManager");
            audioManagerGO.AddComponent<AudioManager>();
        }
    }
    
    private void CreatePlayer()
    {
        GameObject playerGO = GameObject.Find("Player");
        if (playerGO == null)
        {
            playerGO = new GameObject("Player");
            playerGO.tag = "Player";
            
            // Add CharacterController
            CharacterController controller = playerGO.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 1, 0);
            
            // Add PlayerController
            playerGO.AddComponent<PlayerController>();
            
            // Add GrabController
            playerGO.AddComponent<GrabController>();
            
            // Add InteractionController
            playerGO.AddComponent<InteractionController>();
            
            // Create Camera
            GameObject cameraGO = new GameObject("PlayerCamera");
            cameraGO.transform.SetParent(playerGO.transform);
            cameraGO.transform.localPosition = new Vector3(0, 1.7f, 0); // Höher positioniert
            
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.fieldOfView = 75f; // Etwas breiteres Sichtfeld
            
            // Add PostProcessingManager to camera
            cameraGO.AddComponent<PostProcessingManager>();
            
            // Set position - sicherere Methode
            playerGO.transform.SetPositionAndRotation(new Vector3(0, 1, 0), Quaternion.identity);
        }
    }
    
    private void CreateEnvironment()
    {
        // Ground
        GameObject groundGO = GameObject.Find("Ground");
        if (groundGO == null)
        {
            groundGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundGO.name = "Ground";
            // Transform-Änderungen in separaten Zeilen für bessere Stabilität
            groundGO.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
            groundGO.transform.localScale = new Vector3(20, 1, 20);
            groundGO.layer = LayerMask.NameToLayer("Default");

            // Professioneller Boden-Design mit Grid-Pattern
            Renderer renderer = groundGO.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Erstelle ein neues Material für den Boden
                Material groundMaterial = new Material(Shader.Find("Standard"));
                groundMaterial.color = new Color(0.85f, 0.85f, 0.9f); // Hellgrau mit leichtem Blaustich
                groundMaterial.SetFloat("_Metallic", 0.1f);
                groundMaterial.SetFloat("_Smoothness", 0.2f);

                // Erstelle ein einfaches Grid-Texture programmatisch
                Texture2D gridTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
                Color gridColor = new Color(0.7f, 0.7f, 0.8f); // Dunklere Grid-Linien
                Color bgColor = new Color(0.9f, 0.9f, 0.95f); // Hellerer Hintergrund

                for (int x = 0; x < 128; x++)
                {
                    for (int y = 0; y < 128; y++)
                    {
                        // Erstelle Grid-Linien alle 16 Pixel
                        if (x % 16 == 0 || y % 16 == 0)
                        {
                            gridTexture.SetPixel(x, y, gridColor);
                        }
                        else
                        {
                            gridTexture.SetPixel(x, y, bgColor);
                        }
                    }
                }
                gridTexture.Apply();

                groundMaterial.mainTexture = gridTexture;
                groundMaterial.mainTextureScale = new Vector2(10, 10); // Grid skalieren

                renderer.material = groundMaterial;
            }

            // Entferne den MeshCollider und füge einen BoxCollider hinzu
            MeshCollider meshCol = groundGO.GetComponent<MeshCollider>();
            if (meshCol != null)
            {
                DestroyImmediate(meshCol);
            }

            // Füge einen BoxCollider hinzu für bessere Kollision
            BoxCollider boxCol = groundGO.AddComponent<BoxCollider>();
            boxCol.size = new Vector3(20, 0.1f, 20); // Dünner Collider
            boxCol.center = new Vector3(0, -0.05f, 0); // Etwas unter der Oberfläche
        }

        // Walls mit Kenney-Modellen (größer und sichtbarer)
        CreateKenneyWall("Wall_North", new Vector3(0, 2.5f, 20), new Vector3(40, 5, 2), new Color(0.7f, 0.6f, 0.5f));
        CreateKenneyWall("Wall_South", new Vector3(0, 2.5f, -20), new Vector3(40, 5, 2), new Color(0.7f, 0.6f, 0.5f));
        CreateKenneyWall("Wall_East", new Vector3(20, 2.5f, 0), new Vector3(2, 5, 40), new Color(0.7f, 0.6f, 0.5f));
        CreateKenneyWall("Wall_West", new Vector3(-20, 2.5f, 0), new Vector3(2, 5, 40), new Color(0.7f, 0.6f, 0.5f));
    }
    
    private void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wallGO = GameObject.Find(name);
        if (wallGO == null)
        {
            wallGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallGO.name = name;
            // Sicherere Transform-Änderungen
            wallGO.transform.SetPositionAndRotation(position, Quaternion.identity);
            wallGO.transform.localScale = scale;
        }
    }

    private void CreateStyledWall(string name, Vector3 position, Vector3 scale, Color wallColor)
    {
        GameObject wallGO = GameObject.Find(name);
        if (wallGO == null)
        {
            wallGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallGO.name = name;
            // Sicherere Transform-Änderungen
            wallGO.transform.SetPositionAndRotation(position, Quaternion.identity);
            wallGO.transform.localScale = scale;

            // Verbessertes Material für Wände
            Renderer renderer = wallGO.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material wallMaterial = new Material(Shader.Find("Standard"));
                wallMaterial.color = wallColor;
                wallMaterial.SetFloat("_Metallic", 0.0f);
                wallMaterial.SetFloat("_Smoothness", 0.1f);
                renderer.material = wallMaterial;
            }
        }
    }

    private void CreateKenneyWall(string name, Vector3 position, Vector3 scale, Color wallColor)
    {
        GameObject wallGO = GameObject.Find(name);
        if (wallGO == null)
        {
            // Prefer assigned Kenney prefab if available
            if (kenneyWallPrefab != null)
            {
                wallGO = Instantiate(kenneyWallPrefab);
                wallGO.name = name;
                wallGO.transform.position = position;
                wallGO.transform.localScale = scale;
            }
            else
            {
                // Create wall with better design (no Kenney for now)
                wallGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallGO.name = name;
                wallGO.transform.position = position;
                wallGO.transform.localScale = scale;
                
                // Add wall details
                CreateWallDetails(wallGO);
            }
            
            // Apply wall material
            Renderer renderer = wallGO.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material wallMaterial = new Material(Shader.Find("Standard"));
                wallMaterial.color = wallColor;
                wallMaterial.SetFloat("_Metallic", 0.1f);
                wallMaterial.SetFloat("_Smoothness", 0.2f);
                renderer.material = wallMaterial;
                Debug.Log($"Wall material applied successfully for {name}!");
            }
        }
    }
    
    private void CreateDoorFrameDetails(GameObject doorFrame)
    {
        // Add door frame corners
        for (int i = 0; i < 4; i++)
        {
            GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            corner.name = $"DoorFrameCorner_{i}";
            corner.transform.SetParent(doorFrame.transform);
            
            Vector3[] cornerPositions = {
                new Vector3(-1.2f, -2.2f, 0), // Bottom left
                new Vector3(1.2f, -2.2f, 0),  // Bottom right
                new Vector3(-1.2f, 2.2f, 0),  // Top left
                new Vector3(1.2f, 2.2f, 0)    // Top right
            };
            
            corner.transform.localPosition = cornerPositions[i];
            corner.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // Größer für bessere Sichtbarkeit
            
            // Darker corner material
            Renderer cornerRenderer = corner.GetComponent<Renderer>();
            Material cornerMaterial = new Material(Shader.Find("Standard"));
            cornerMaterial.color = new Color(0.2f, 0.1f, 0.05f); // Very dark wood
            cornerMaterial.SetFloat("_Metallic", 0.0f);
            cornerMaterial.SetFloat("_Smoothness", 0.0f);
            cornerRenderer.material = cornerMaterial;
        }
        
        // Add door frame trim
        GameObject trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trim.name = "DoorFrameTrim";
        trim.transform.SetParent(doorFrame.transform);
        trim.transform.localPosition = new Vector3(0, 0, 0.1f);
        trim.transform.localScale = new Vector3(1.1f, 1.1f, 0.1f);
        
        // Trim material
        Renderer trimRenderer = trim.GetComponent<Renderer>();
        Material trimMaterial = new Material(Shader.Find("Standard"));
        trimMaterial.color = new Color(0.1f, 0.05f, 0.02f); // Very dark trim
        trimMaterial.SetFloat("_Metallic", 0.0f);
        trimMaterial.SetFloat("_Smoothness", 0.0f);
        trimRenderer.material = trimMaterial;
    }
    
    private void CreateDoorDetails(GameObject door)
    {
        // Add door handle
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        handle.name = "DoorHandle";
        handle.transform.SetParent(door.transform);
        handle.transform.localPosition = new Vector3(0.8f, 0, 0.2f);
        handle.transform.localScale = new Vector3(0.3f, 0.3f, 0.2f); // Größer für bessere Sichtbarkeit
        
        // Metallic handle material
        Renderer handleRenderer = handle.GetComponent<Renderer>();
        Material handleMaterial = new Material(Shader.Find("Standard"));
        handleMaterial.color = new Color(0.7f, 0.7f, 0.8f); // Metallic silver
        handleMaterial.SetFloat("_Metallic", 0.8f);
        handleMaterial.SetFloat("_Smoothness", 0.9f);
        handleRenderer.material = handleMaterial;
        
        // Add door panels
        for (int i = 0; i < 3; i++)
        {
            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = $"DoorPanel_{i}";
            panel.transform.SetParent(door.transform);
            panel.transform.localPosition = new Vector3(0, (i - 1) * 1.2f, 0.1f);
            panel.transform.localScale = new Vector3(1.8f, 0.2f, 0.1f); // Größer für bessere Sichtbarkeit
            
            // Slightly darker panel material
            Renderer panelRenderer = panel.GetComponent<Renderer>();
            Material panelMaterial = new Material(Shader.Find("Standard"));
            panelMaterial.color = new Color(0.7f, 0.5f, 0.3f); // Darker wood
            panelMaterial.SetFloat("_Metallic", 0.0f);
            panelMaterial.SetFloat("_Smoothness", 0.2f);
            panelRenderer.material = panelMaterial;
        }
        
        // Add door border
        GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
        border.name = "DoorBorder";
        border.transform.SetParent(door.transform);
        border.transform.localPosition = new Vector3(0, 0, 0.05f);
        border.transform.localScale = new Vector3(1.1f, 1.1f, 0.05f);
        
        // Border material
        Renderer borderRenderer = border.GetComponent<Renderer>();
        Material borderMaterial = new Material(Shader.Find("Standard"));
        borderMaterial.color = new Color(0.4f, 0.3f, 0.2f); // Dark wood border
        borderMaterial.SetFloat("_Metallic", 0.0f);
        borderMaterial.SetFloat("_Smoothness", 0.1f);
        borderRenderer.material = borderMaterial;
    }
    
    private void CreateWallDetails(GameObject wall)
    {
        // Add wall trim
        GameObject trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trim.name = "WallTrim";
        trim.transform.SetParent(wall.transform);
        trim.transform.localPosition = new Vector3(0, 2.4f, 0);
        trim.transform.localScale = new Vector3(1.1f, 0.2f, 1.1f); // Größer für bessere Sichtbarkeit
        
        // Darker trim material
        Renderer trimRenderer = trim.GetComponent<Renderer>();
        Material trimMaterial = new Material(Shader.Find("Standard"));
        trimMaterial.color = new Color(0.4f, 0.3f, 0.2f); // Dark wood trim
        trimMaterial.SetFloat("_Metallic", 0.0f);
        trimMaterial.SetFloat("_Smoothness", 0.1f);
        trimRenderer.material = trimMaterial;
        
        // Add wall baseboard
        GameObject baseboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseboard.name = "WallBaseboard";
        baseboard.transform.SetParent(wall.transform);
        baseboard.transform.localPosition = new Vector3(0, -2.4f, 0);
        baseboard.transform.localScale = new Vector3(1.1f, 0.2f, 1.1f);
        
        // Baseboard material
        Renderer baseboardRenderer = baseboard.GetComponent<Renderer>();
        Material baseboardMaterial = new Material(Shader.Find("Standard"));
        baseboardMaterial.color = new Color(0.3f, 0.2f, 0.1f); // Darker wood baseboard
        baseboardMaterial.SetFloat("_Metallic", 0.0f);
        baseboardMaterial.SetFloat("_Smoothness", 0.1f);
        baseboardRenderer.material = baseboardMaterial;
    }
    
    private void CreateInteractiveObjects()
    {
        // Physics Objects
        for (int i = 0; i < 5; i++)
        {
            GameObject cubeGO = GameObject.Find($"PhysicsCube_{i}");
            if (cubeGO == null)
            {
                cubeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeGO.name = $"PhysicsCube_{i}";
                cubeGO.transform.position = new Vector3(i * 3 - 6, 2, 0);
                cubeGO.AddComponent<PhysicsObject>();
                cubeGO.AddComponent<Grabbable>();
            }
        }
        
        // Switches
        for (int i = 0; i < 3; i++)
        {
            GameObject switchGO = GameObject.Find($"Switch_{i}");
            if (switchGO == null)
            {
                switchGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                switchGO.name = $"Switch_{i}";
                switchGO.transform.position = new Vector3(i * 4 - 4, 1, 10);
                switchGO.transform.localScale = new Vector3(0.5f, 0.2f, 0.5f);
                switchGO.GetComponent<Renderer>().material.color = Color.red;
                switchGO.AddComponent<Switch>();
            }
        }
        
        // Doors
        for (int i = 0; i < 2; i++)
        {
            GameObject doorGO = GameObject.Find($"Door_{i}");
            if (doorGO == null)
            {
                // Create door frame
                GameObject doorFrameGO = new GameObject($"DoorFrame_{i}");
                doorFrameGO.transform.position = new Vector3(i * 8 - 4, 2.5f, 15);
                
                // Create door
                doorGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                doorGO.name = $"Door_{i}";
                doorGO.transform.SetParent(doorFrameGO.transform);
                doorGO.transform.localPosition = Vector3.zero;
                doorGO.transform.localScale = new Vector3(2, 5, 0.2f);
                doorGO.GetComponent<Renderer>().material.color = new Color(0.6f, 0.4f, 0.2f); // Brown color
                
                // Add Door component and set pivot
                Door doorScript = doorGO.AddComponent<Door>();
                doorScript.doorPivot = doorFrameGO.transform;
            }
        }
        
        // Pressure Plates
        CreatePressurePlates();
        
        // Crystals
        Vector3[] crystalPositions = {
            new Vector3(5, 2, 5),
            new Vector3(-5, 2, 5),
            new Vector3(0, 5, 10),
            new Vector3(8, 2, -5),
            new Vector3(-8, 2, -5)
        };
        
        for (int i = 0; i < crystalPositions.Length; i++)
        {
            GameObject crystalGO = GameObject.Find($"Crystal_{i}");
            if (crystalGO == null)
            {
                crystalGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                crystalGO.name = $"Crystal_{i}";
                crystalGO.transform.position = crystalPositions[i];
                crystalGO.transform.localScale = Vector3.one * 0.5f;
                
                Renderer renderer = crystalGO.GetComponent<Renderer>();
                if (crystalMaterial != null)
                {
                    renderer.material = crystalMaterial;
                }
                else
                {
                    renderer.material.color = Color.yellow;
                    renderer.material.SetFloat("_Metallic", 0.8f);
                    renderer.material.SetFloat("_Smoothness", 0.9f);
                }
                
                crystalGO.AddComponent<CrystalCollector>();
                SphereCollider collider = crystalGO.GetComponent<SphereCollider>();
                collider.isTrigger = true;
            }
        }
    }
    
    private void CreatePhysicsZones()
    {
        // Anti-Gravity Zone
        GameObject antiGravityGO = GameObject.Find("AntiGravityZone");
        if (antiGravityGO == null)
        {
            antiGravityGO = new GameObject("AntiGravityZone");
            antiGravityGO.transform.position = new Vector3(0, 1, 15);
            SphereCollider antiGravityCollider = antiGravityGO.AddComponent<SphereCollider>();
            antiGravityCollider.isTrigger = true;
            antiGravityCollider.radius = 5f;
            antiGravityGO.AddComponent<AntiGravityZone>();
        }
        
        // Time Distortion Zone
        GameObject timeDistortionGO = GameObject.Find("TimeDistortionZone");
        if (timeDistortionGO == null)
        {
            timeDistortionGO = new GameObject("TimeDistortionZone");
            timeDistortionGO.transform.position = new Vector3(10, 1, 0);
            SphereCollider timeDistortionCollider = timeDistortionGO.AddComponent<SphereCollider>();
            timeDistortionCollider.isTrigger = true;
            timeDistortionCollider.radius = 4f;
            timeDistortionGO.AddComponent<TimeDistortionZone>();
        }
        
        // Gravity Rotator
        GameObject gravityRotatorGO = GameObject.Find("GravityRotator");
        if (gravityRotatorGO == null)
        {
            gravityRotatorGO = new GameObject("GravityRotator");
            gravityRotatorGO.transform.position = new Vector3(-10, 1, 0);
            SphereCollider gravityRotatorCollider = gravityRotatorGO.AddComponent<SphereCollider>();
            gravityRotatorCollider.isTrigger = true;
            gravityRotatorCollider.radius = 6f;
            gravityRotatorGO.AddComponent<GravityRotator>();
        }
    }
    
    private void CreatePressurePlates()
    {
        // Create pressure plates
        Vector3[] platePositions = {
            new Vector3(0, 0.1f, 12), // In front of first door
            new Vector3(8, 0.1f, 12), // In front of second door
            new Vector3(-4, 0.1f, 8), // For puzzle solving
            new Vector3(4, 0.1f, 8)   // For puzzle solving
        };
        
        for (int i = 0; i < platePositions.Length; i++)
        {
            GameObject plateGO = GameObject.Find($"PressurePlate_{i}");
            if (plateGO == null)
            {
                // Create pressure plate
                plateGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                plateGO.name = $"PressurePlate_{i}";
                plateGO.transform.position = platePositions[i];
                plateGO.transform.localScale = new Vector3(2, 0.2f, 2);
                plateGO.GetComponent<Renderer>().material.color = Color.gray;
                
                // Add trigger collider
                Collider plateCollider = plateGO.GetComponent<Collider>();
                plateCollider.isTrigger = true;
                
                // Add PressurePlate component
                PressurePlate pressurePlate = plateGO.AddComponent<PressurePlate>();
                pressurePlate.activationWeight = 0.5f; // Light activation
                
                // Create indicator light
                GameObject lightGO = new GameObject("IndicatorLight");
                lightGO.transform.SetParent(plateGO.transform);
                lightGO.transform.localPosition = new Vector3(0, 0.2f, 0);
                lightGO.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
                
                GameObject lightCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lightCube.transform.SetParent(lightGO.transform);
                lightCube.transform.localPosition = Vector3.zero;
                lightCube.transform.localScale = Vector3.one;
                lightCube.GetComponent<Renderer>().material.color = Color.red;
                
                // Remove collider from light
                DestroyImmediate(lightCube.GetComponent<Collider>());
            }
        }
        
        // Create door triggers to connect plates with doors
        CreateDoorTriggers();
    }
    
    private void CreateDoorTriggers()
    {
        // Find doors and pressure plates
        Door[] doors = FindObjectsOfType<Door>();
        PressurePlate[] plates = FindObjectsOfType<PressurePlate>();
        
        // Create door triggers
        for (int i = 0; i < doors.Length && i < 2; i++)
        {
            GameObject triggerGO = GameObject.Find($"DoorTrigger_{i}");
            if (triggerGO == null)
            {
                triggerGO = new GameObject($"DoorTrigger_{i}");
                triggerGO.transform.position = doors[i].transform.position;
                
                DoorTrigger doorTrigger = triggerGO.AddComponent<DoorTrigger>();
                doorTrigger.targetDoor = doors[i];
                
                // Only assign plate if it exists
                if (i < plates.Length && plates[i] != null)
                {
                    doorTrigger.requiredPlates = new PressurePlate[] { plates[i] };
                }
                else
                {
                    doorTrigger.requiredPlates = new PressurePlate[0];
                }
                
                doorTrigger.requireAllPlates = true;
                doorTrigger.openOnActivation = true;
                doorTrigger.closeOnDeactivation = true;
            }
        }
    }
    
    private void CreateUI()
    {
        // Create Canvas
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null)
        {
            canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        
        // Create EventSystem
        GameObject eventSystemGO = GameObject.Find("EventSystem");
        if (eventSystemGO == null)
        {
            eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }
    
    private void CreateFirstPuzzleLevel()
    {
        Debug.Log("Creating first puzzle level: Tutorial Box Puzzle");
        
        // Create the puzzle room structure
        CreatePuzzleRoom();
        
        // Create the tutorial puzzle elements
        CreateTutorialPuzzle();
        
        // Add visual hints and lighting
        AddPuzzleLighting();
    }
    
    private void CreatePuzzleRoom()
    {
        // Create a dedicated puzzle room
        GameObject puzzleRoomGO = new GameObject("PuzzleRoom");
        puzzleRoomGO.transform.position = new Vector3(0, 0, 0);

        // Room floor ist jetzt der Haupt-Ground, keine separate Plane mehr
        
        // Create room walls
        CreatePuzzleWall("PuzzleWall_North", new Vector3(0, 2.5f, 10), new Vector3(20, 5, 1));
        CreatePuzzleWall("PuzzleWall_South", new Vector3(0, 2.5f, -10), new Vector3(20, 5, 1));
        CreatePuzzleWall("PuzzleWall_East", new Vector3(10, 2.5f, 0), new Vector3(1, 5, 20));
        CreatePuzzleWall("PuzzleWall_West", new Vector3(-10, 2.5f, 0), new Vector3(1, 5, 20));
    }
    
    private void CreatePuzzleWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wallGO = GameObject.Find(name);
        if (wallGO == null)
        {
            wallGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallGO.name = name;
            // Sicherere Transform-Änderungen
            wallGO.transform.SetPositionAndRotation(position, Quaternion.identity);
            wallGO.transform.localScale = scale;

            // Gleiches Material wie die äußeren Wände für Konsistenz
            Renderer renderer = wallGO.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material wallMaterial = new Material(Shader.Find("Standard"));
                wallMaterial.color = new Color(0.75f, 0.75f, 0.8f); // Leicht dunkler als äußere Wände
                wallMaterial.SetFloat("_Metallic", 0.0f);
                wallMaterial.SetFloat("_Smoothness", 0.1f);
                renderer.material = wallMaterial;
            }
        }
    }
    
    private void CreateTutorialPuzzle()
    {
        // 1. Create the puzzle box (grabbable object)
        GameObject puzzleBoxGO = GameObject.Find("PuzzleBox");
        if (puzzleBoxGO == null)
        {
            puzzleBoxGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            puzzleBoxGO.name = "PuzzleBox";
            puzzleBoxGO.transform.position = new Vector3(-5, 1, -3); // Left side of room
            puzzleBoxGO.transform.localScale = new Vector3(1, 1, 1);
            puzzleBoxGO.GetComponent<Renderer>().material.color = new Color(0.8f, 0.4f, 0.2f); // Orange box
            
            // Add physics and grab components
            Rigidbody rb = puzzleBoxGO.AddComponent<Rigidbody>();
            rb.mass = 2f; // Heavy enough to activate pressure plate
            puzzleBoxGO.AddComponent<PhysicsObject>();
            puzzleBoxGO.AddComponent<Grabbable>();
        }
        
        // 2. Create the pressure plate
        GameObject pressurePlateGO = GameObject.Find("TutorialPressurePlate");
        if (pressurePlateGO == null)
        {
            pressurePlateGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pressurePlateGO.name = "TutorialPressurePlate";
            pressurePlateGO.transform.position = new Vector3(0, 0.1f, -3); // Center of room
            pressurePlateGO.transform.localScale = new Vector3(2, 0.2f, 2); // Wide, flat plate
            pressurePlateGO.GetComponent<Renderer>().material.color = new Color(0.3f, 0.7f, 0.3f); // Green
            
            // Add pressure plate component
            PressurePlate pressurePlate = pressurePlateGO.AddComponent<PressurePlate>();
            pressurePlate.activationWeight = 1.5f; // Requires the box weight
            pressurePlate.pressDepth = 0.1f;
            pressurePlate.inactiveColor = new Color(0.3f, 0.7f, 0.3f);
            pressurePlate.activeColor = new Color(0.1f, 1f, 0.1f); // Bright green when active
            
            // Add trigger collider
            BoxCollider trigger = pressurePlateGO.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(2.5f, 1f, 2.5f);
            trigger.center = new Vector3(0, 0.5f, 0);
        }
        
        // 3. Create the door and door frame
        CreateTutorialDoor();
        
        // 4. Create the crystal behind the door
        CreateTutorialCrystal();
        
        // 5. Create door trigger system
        CreateDoorTriggerSystem();
    }
    
    private void CreateTutorialDoor()
    {
        // Create door frame using Kenney wall model
        GameObject doorFrameGO = GameObject.Find("TutorialDoorFrame");
        if (doorFrameGO == null)
        {
            // Prefer assigned Kenney prefab if available
            if (kenneyDoorFramePrefab != null)
            {
                doorFrameGO = Instantiate(kenneyDoorFramePrefab);
                doorFrameGO.name = "TutorialDoorFrame";
                doorFrameGO.transform.position = new Vector3(0, 2.5f, 5); // North wall
                doorFrameGO.transform.localScale = new Vector3(3, 5, 1f);
            }
            else
            {
                // Create door frame with better design (no Kenney for now)
                doorFrameGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                doorFrameGO.name = "TutorialDoorFrame";
                doorFrameGO.transform.position = new Vector3(0, 2.5f, 5); // North wall
                doorFrameGO.transform.localScale = new Vector3(3, 5, 0.5f);
                
                // Add door frame details
                CreateDoorFrameDetails(doorFrameGO);
            }
            
            // Apply better material for door frame
            Renderer frameRenderer = doorFrameGO.GetComponent<Renderer>();
            if (frameRenderer != null)
            {
                Material frameMaterial = new Material(Shader.Find("Standard"));
                frameMaterial.color = new Color(0.3f, 0.2f, 0.1f); // Dark wood frame
                frameMaterial.SetFloat("_Metallic", 0.0f);
                frameMaterial.SetFloat("_Smoothness", 0.1f);
                frameRenderer.material = frameMaterial;
                Debug.Log("Door frame material applied successfully!");
            }
        }
        
        // Create the actual door using Kenney door model
        GameObject doorGO = GameObject.Find("TutorialDoor");
        if (doorGO == null)
        {
            // Prefer assigned Kenney prefab if available
            if (kenneyDoorPrefab != null)
            {
                doorGO = Instantiate(kenneyDoorPrefab);
                doorGO.name = "TutorialDoor";
                doorGO.transform.position = new Vector3(0, 2.5f, 5.1f); // Slightly in front of frame
                doorGO.transform.localScale = new Vector3(2.5f, 4.5f, 1f);
            }
            else
            {
                // Create door with better design (no Kenney for now)
                doorGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                doorGO.name = "TutorialDoor";
                doorGO.transform.position = new Vector3(0, 2.5f, 5.1f); // Slightly in front of frame
                doorGO.transform.localScale = new Vector3(2.5f, 4.5f, 0.3f);
                
                // Add door details
                CreateDoorDetails(doorGO);
            }
            
            // Apply better material for door
            Renderer doorRenderer = doorGO.GetComponent<Renderer>();
            if (doorRenderer != null)
            {
                Material doorMaterial = new Material(Shader.Find("Standard"));
                doorMaterial.color = new Color(0.8f, 0.6f, 0.4f); // Light wood door
                doorMaterial.SetFloat("_Metallic", 0.1f);
                doorMaterial.SetFloat("_Smoothness", 0.3f);
                doorRenderer.material = doorMaterial;
                Debug.Log("Door material applied successfully!");
            }

            // Add door component
            Door door = doorGO.AddComponent<Door>();
            door.openAngle = 90f; // Rotates 90 degrees when open
            door.closeAngle = 0f; // Normal position when closed
            door.doorSpeed = 8f; // Schnellere Tür-Öffnung

            // Win-Bedingung: Wenn diese Tür sich öffnet, hat der Spieler gewonnen
            // Event-Registrierung mit sicherer Initialisierung
            StartCoroutine(RegisterDoorWinEvent(door));
        }
    }
    
    private void CreateTutorialCrystal()
    {
        GameObject crystalGO = GameObject.Find("TutorialCrystal");
        if (crystalGO == null)
        {
            crystalGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            crystalGO.name = "TutorialCrystal";
            crystalGO.transform.position = new Vector3(0, 2f, 8); // Behind the door
            crystalGO.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            
            // Make it look like a crystal
            Renderer renderer = crystalGO.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.8f, 0.2f); // Golden crystal
            renderer.material.SetFloat("_Metallic", 0.8f);
            renderer.material.SetFloat("_Smoothness", 0.9f);
            
            // Add crystal collector component
            CrystalCollector crystalCollector = crystalGO.AddComponent<CrystalCollector>();
            crystalCollector.crystalValue = 1;
            
            // Add floating animation
            crystalGO.AddComponent<FloatingCrystal>();
        }
    }
    
    private void CreateDoorTriggerSystem()
    {
        // Create door trigger
        GameObject doorTriggerGO = GameObject.Find("TutorialDoorTrigger");
        if (doorTriggerGO == null)
        {
            doorTriggerGO = new GameObject("TutorialDoorTrigger");
            doorTriggerGO.transform.position = new Vector3(0, 1f, 0);
            
            // Add door trigger component
            DoorTrigger doorTrigger = doorTriggerGO.AddComponent<DoorTrigger>();
            
            // Find the door and pressure plate
            Door door = GameObject.Find("TutorialDoor").GetComponent<Door>();
            PressurePlate pressurePlate = GameObject.Find("TutorialPressurePlate").GetComponent<PressurePlate>();
            
            // Configure the trigger
            doorTrigger.targetDoor = door;
            doorTrigger.openOnActivation = true;
            doorTrigger.closeOnDeactivation = true;
            doorTrigger.requiredPlates = new PressurePlate[] { pressurePlate };
            doorTrigger.requireAllPlates = true;
        }
    }
    
    private void AddPuzzleLighting()
    {
        // Add spotlight over the pressure plate
        GameObject pressurePlateLightGO = new GameObject("PressurePlateLight");
        pressurePlateLightGO.transform.position = new Vector3(0, 8, -3);
        pressurePlateLightGO.transform.rotation = Quaternion.Euler(45, 0, 0);
        
        Light pressurePlateLight = pressurePlateLightGO.AddComponent<Light>();
        pressurePlateLight.type = LightType.Spot;
        pressurePlateLight.color = new Color(0.2f, 1f, 0.2f); // Green light
        pressurePlateLight.intensity = 2f;
        pressurePlateLight.spotAngle = 30f;
        pressurePlateLight.range = 15f;
        
        // Add spotlight over the crystal
        GameObject crystalLightGO = new GameObject("CrystalLight");
        crystalLightGO.transform.position = new Vector3(0, 8, 8);
        crystalLightGO.transform.rotation = Quaternion.Euler(45, 0, 0);
        
        Light crystalLight = crystalLightGO.AddComponent<Light>();
        crystalLight.type = LightType.Spot;
        crystalLight.color = new Color(1f, 0.8f, 0.2f); // Golden light
        crystalLight.intensity = 1.5f;
        crystalLight.spotAngle = 25f;
        crystalLight.range = 12f;
    }

    private void CreateTeleporters()
    {
        // Erstelle mehrere Teleporter-Paare für komplexe Navigation

        AudioManager audioManager = FindObjectOfType<AudioManager>();

        // Teleporter Pair 1: Zentraler Bereich (Cyan <-> Magenta)
        CreateTeleporterPair("Teleporter_A", new Vector3(5, 1, 3), Color.cyan,
                           "Teleporter_B", new Vector3(-5, 1, 3), Color.magenta, audioManager);

        // Teleporter Pair 2: Hinten vorne (Grün <-> Blau)
        CreateTeleporterPair("Teleporter_C", new Vector3(0, 1, 8), Color.green,
                           "Teleporter_D", new Vector3(0, 1, -8), Color.blue, audioManager);

        // Teleporter Pair 3: Seitlich (Gelb <-> Rot)
        CreateTeleporterPair("Teleporter_E", new Vector3(12, 1, 0), Color.yellow,
                           "Teleporter_F", new Vector3(-12, 1, 0), Color.red, audioManager);

        Debug.Log("4 Teleporter-Paare erstellt: A↔B, C↔D, E↔F");
    }

    private void CreateTeleporterPair(string name1, Vector3 pos1, Color color1,
                                    string name2, Vector3 pos2, Color color2, AudioManager audioManager)
    {
        // Teleporter 1
        GameObject teleporter1GO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        teleporter1GO.name = name1;
        teleporter1GO.transform.position = pos1;
        teleporter1GO.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);

        Teleporter teleporter1 = teleporter1GO.AddComponent<Teleporter>();
        teleporter1.teleportColor = color1;

        // Teleporter 2
        GameObject teleporter2GO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        teleporter2GO.name = name2;
        teleporter2GO.transform.position = pos2;
        teleporter2GO.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);

        Teleporter teleporter2 = teleporter2GO.AddComponent<Teleporter>();
        teleporter2.teleportColor = color2;

        // Verbinde die Teleporter
        teleporter1.linkedTeleporter = teleporter2;
        teleporter2.linkedTeleporter = teleporter1;

        // Füge Collider hinzu
        AddTeleporterCollider(teleporter1GO);
        AddTeleporterCollider(teleporter2GO);

        // Sound zuweisen
        if (audioManager != null)
        {
            teleporter1.teleportSound = audioManager.teleportSound;
            teleporter2.teleportSound = audioManager.teleportSound;
        }
    }

    private void AddTeleporterCollider(GameObject teleporterGO)
    {
        BoxCollider col = teleporterGO.AddComponent<BoxCollider>();
        col.size = new Vector3(1.5f, 0.5f, 1.5f);
        col.center = new Vector3(0, 0.25f, 0);
        col.isTrigger = true;
    }

    // Sichere Event-Registrierung für Tür-Sieg-Bedingung
    private System.Collections.IEnumerator RegisterDoorWinEvent(Door door)
    {
        // Eine Frame warten, damit die Door-Komponente vollständig initialisiert ist
        yield return null;

        if (door != null && door.OnDoorOpen != null)
        {
            door.OnDoorOpen.AddListener(() => {
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    // Sofortiger Sieg - keine Kristalle mehr nötig
                    gameManager.ForceWin();
                }
            });
        }
    }

    private void CreateSetupHintLogo()
    {
        // Erstelle ein Logo, das anzeigt, wann Unity-Einstellungen geändert werden müssen
        GameObject logoGO = GameObject.Find("SetupHintLogo");
        if (logoGO == null)
        {
            logoGO = new GameObject("SetupHintLogo");
            logoGO.transform.position = new Vector3(0, 3, 0); // Über dem Spieler

            // Erstelle einen einfachen Hintergrund
            GameObject bgGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bgGO.name = "LogoBackground";
            bgGO.transform.SetParent(logoGO.transform);
            bgGO.transform.localPosition = new Vector3(0, 0, 0.1f);
            bgGO.transform.localScale = new Vector3(4, 2, 1);
            bgGO.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0.7f); // Halbtransparent schwarz

            // Erstelle ein einfaches Text-Logo
            GameObject textGO = new GameObject("SetupText");
            textGO.transform.SetParent(logoGO.transform);
            textGO.transform.localPosition = Vector3.zero;

            var textMesh = textGO.AddComponent<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = "!!! UNITY EINSTELLEN !!!\n\nWenn du dieses Logo siehst:\n- Quality Settings uberprufen\n- VSync auf 'Don't Sync'\n- Target FPS auf 60\n- Shadows aktivieren";
                textMesh.fontSize = 28;
                textMesh.color = Color.yellow;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.alignment = TextAlignment.Center;
            }

            // Logo ist standardmäßig unsichtbar - wird nur bei Bedarf angezeigt
            logoGO.SetActive(false);
        }
    }

    // Methode um das Logo ein-/auszuschalten
    public void ShowSetupHint(bool show)
    {
        GameObject logoGO = GameObject.Find("SetupHintLogo");
        if (logoGO != null)
        {
            logoGO.SetActive(show);
        }
    }
}
