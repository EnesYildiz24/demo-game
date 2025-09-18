using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [Header("Setup Settings")]
    public bool autoSetup = true;
    public Material defaultMaterial;
    public Material crystalMaterial;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupCompleteGame();
        }
    }
    
    [ContextMenu("Setup Complete Game")]
    public void SetupCompleteGame()
    {
        Debug.Log("Setting up complete Quantum Puzzle Game...");
        
        // Create Core Managers (except AudioManager)
        CreateCoreManagers();
        
        // Create Player first
        CreatePlayer();
        
        // Create AudioManager after player
        CreateAudioManager();
        
        // Create Environment
        CreateEnvironment();
        
        // Create Interactive Objects
        CreateInteractiveObjects();
        
        // Create Physics Zones
        CreatePhysicsZones();
        
        // Create UI
        CreateUI();
        
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
            cameraGO.transform.localPosition = new Vector3(0, 0.5f, 0);
            
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.fieldOfView = 60f;
            
            // Add PostProcessingManager to camera
            cameraGO.AddComponent<PostProcessingManager>();
            
            // Set position
            playerGO.transform.position = new Vector3(0, 1, 0);
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
            groundGO.transform.position = new Vector3(0, 0, 0);
            groundGO.transform.localScale = new Vector3(10, 1, 10);
            groundGO.layer = LayerMask.NameToLayer("Default");
        }
        
        // Walls
        CreateWall("Wall_North", new Vector3(0, 2.5f, 50), new Vector3(100, 5, 1));
        CreateWall("Wall_South", new Vector3(0, 2.5f, -50), new Vector3(100, 5, 1));
        CreateWall("Wall_East", new Vector3(50, 2.5f, 0), new Vector3(1, 5, 100));
        CreateWall("Wall_West", new Vector3(-50, 2.5f, 0), new Vector3(1, 5, 100));
    }
    
    private void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wallGO = GameObject.Find(name);
        if (wallGO == null)
        {
            wallGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallGO.name = name;
            wallGO.transform.position = position;
            wallGO.transform.localScale = scale;
        }
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
}
