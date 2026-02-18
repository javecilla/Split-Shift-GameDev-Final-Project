\### V. Code Standards and Conventions



To ensure a clean, maintainable, and collaborative codebase for \*\*Split Shift\*\*, all team members (Jerome, Mico, Rensen, Francis, Giga) must adhere to the following standards:



\#### \*\*A. File Structure and Project Organization\*\*



| Area | Rule | Example |

| :--- | :--- | :--- |

| \*\*Folder Hierarchy\*\* | Organize assets by \*\*Type\*\* first, then by \*\*Context\*\* or \*\*Feature\*\*. | `Assets/Scripts/Player/`, `Assets/Sprites/Enemies/`, `Assets/Prefabs/UI/` |

| \*\*Script Naming\*\* | \*\*PascalCase\*\*. The file name \*\*MUST\*\* match the Class name exactly, or Unity will crash. | `PlayerController.cs`, `GameManager.cs`. \*\*Rejected:\*\* `playerController.cs`, `player\_move.cs`. |

| \*\*Scene Organization\*\* | Keep scenes in the `Scenes/` folder. Separate test levels from production levels. | `Assets/Scenes/MainMenu.unity`, `Assets/Scenes/Level01.unity`, `Assets/Scenes/Testing/JumpTest.unity` |

| \*\*Prefabs\*\* | Everything that is reused (Player, Coins, Enemies) \*\*MUST\*\* be a Prefab. | `Assets/Prefabs/Player.prefab`, `Assets/Prefabs/Coin.prefab` |

| \*\*Asset Naming\*\* | Use \*\*PascalCase\*\* or \*\*snake\_case\*\*, but be consistent per folder. | `PlayerSprite\_Idle.png` or `bg\_music\_loop.mp3` |

| \*\*Material/Physics\*\* | Group physics materials and shaders in their own folders. | `Assets/Materials/Physics/SlipperyIce.physicsMaterial2D` |



\#### \*\*B. C# Scripting and Unity Logic\*\*



| Rule | Requirement | Example |

| :--- | :--- | :--- |

| \*\*Encapsulation\*\* | \*\*Avoid `public` fields\*\* for Inspector variables. Use `\[SerializeField] private` instead. This keeps data safe but editable in Unity. | `\[SerializeField] private float moveSpeed = 5f;` (Instead of `public float moveSpeed;`) |

| \*\*Method Naming\*\* | \*\*PascalCase\*\*. Methods are actions and should be verbs. | `void Jump()`, `void TakeDamage()`, `bool IsGrounded()` |

| \*\*Physics Logic\*\* | Physics calculations (Rigidbody) \*\*MUST\*\* go in `FixedUpdate()`. Input checks go in `Update()`. | Input in `Update()`: `if (Input.GetButtonDown("Jump"))` <br> Force in `FixedUpdate()`: `rb.AddForce(...)` |

| \*\*Optimization\*\* | \*\*Cache components\*\* in `Awake()` or `Start()`. Do NOT use `GetComponent()` inside `Update()`. | `void Start() { rb = GetComponent<Rigidbody2D>(); }` |

| \*\*Empty Methods\*\* | \*\*Delete\*\* empty Unity methods (`Start`, `Update`) if you aren't using them. They cause minor performance overhead. | \*Delete the default empty Update() function if the script only handles triggers.\* |

| \*\*Time Scaling\*\* | Always multiply movement values by `Time.deltaTime` inside `Update()` to make it frame-rate independent. | `transform.Translate(Vector3.right \* speed \* Time.deltaTime);` |

| \*\*Hardcoding\*\* | \*\*Avoid Magic Numbers\*\*. Use constants or serialized fields for values like speed, damage, or duration. | \*\*Bad:\*\* `rb.velocity = new Vector2(10, 0);` <br> \*\*Good:\*\* `rb.velocity = new Vector2(jumpForce, 0);` |



\#### \*\*C. Variable Naming Conventions\*\*



| Type | Naming Convention | Example |

| :--- | :--- | :--- |

| \*\*Private Fields\*\* | \*\*\_camelCase\*\* (underscore prefix is optional but recommended for private). | `private Rigidbody2D \_rb;`, `private float \_currentHealth;` |

| \*\*Public/Serialized\*\* | \*\*camelCase\*\* (Standard for Unity Inspector fields). | `\[SerializeField] private float jumpForce;` |

| \*\*Booleans\*\* | Must be a question or statement (e.g., `is`, `has`, `can`). | `bool isGrounded;`, `bool canDoubleJump;`, `bool hasKey;` |

| \*\*Constants\*\* | \*\*SCREAMING\_SNAKE\_CASE\*\* (For fixed values). | `const float MAX\_GRAVITY = -9.8f;` |

| \*\*Temporary Vars\*\* | \*\*camelCase\*\* (Inside functions). | `float distance = Vector2.Distance(a, b);` |



\#### \*\*D. Documentation and Commits\*\*



| Rule | Requirement | Example |

| :--- | :--- | :--- |

| \*\*Commit Messages\*\* | Use the \*\*Conventional Commits\*\* format: `type(scope): description`. | `feat(player): add double jump logic` <br> `fix(physics): resolve wall sliding bug` <br> `art(level1): add background parallax sprites` |

| \*\*Code Comments\*\* | Comment \*\*WHY\*\*, not \*WHAT\*. Explain complex math or specific Unity workarounds. | `// Raycast downwards to check if the player is standing on the 'Ground' layer` |

| \*\*Git Ignore\*\* | \*\*NEVER\*\* push the `Library/`, `Temp/`, or `Builds/` folders. Ensure `.gitignore` is active. | \*Check .gitignore before pushing.\* |

