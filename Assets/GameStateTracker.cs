using UnityEngine;

/// <summary>
/// Tracks game state including enemies killed, boss status, and time.
/// Calculates star rating based on performance metrics.
/// </summary>
public class GameStateTracker : MonoBehaviour
{
    public static GameStateTracker Instance;

    // Enemy tracking
    private int totalEnemies = 0;
    private int enemiesKilled = 0;
    private bool bossExists = false;
    private bool bossDefeated = false;

    // Thresholds (calculated based on enemy count)
    private int threshold1Star = 0;
    private int threshold2Stars = 0;

    // Time tracking
    private float gameStartTime = 0f;
    private const float TIME_LIMIT = 600f; // 10 minutes

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✅ GameStateTracker instance created");
        }
        else
        {
            Debug.LogWarning("⚠️ GameStateTracker instance already exists, destroying duplicate");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameStartTime = Time.time;
        CountEnemiesInScene();
        CalculateThresholds();
        
        Debug.Log($"🎮 Game started - Total enemies: {totalEnemies}, Boss present: {bossExists}");
        Debug.Log($"⭐ Star thresholds - 1 Star: {threshold1Star} kills, 2 Stars: {threshold2Stars} kills");
    }

    /// <summary>
    /// Count all enemies in the scene and identify boss.
    /// </summary>
    private void CountEnemiesInScene()
    {
        EnemyBase[] allEnemies = FindObjectsOfType<EnemyBase>();
        totalEnemies = allEnemies.Length;

        // Check if boss exists by name (not by tag since boss is tagged as "Enemy")
        foreach (EnemyBase enemy in allEnemies)
        {
            if (enemy.gameObject.name.Contains("Boss") || enemy.gameObject.name.Contains("Dinosaur"))
            {
                bossExists = true;
                break;
            }
        }
    }

    /// <summary>
    /// Calculate star thresholds based on total enemies (percentage-based).
    /// </summary>
    private void CalculateThresholds()
    {
        // 1 Star: 35% of enemies OR boss defeated
        threshold1Star = Mathf.CeilToInt(totalEnemies * 0.35f);
        
        // 2 Stars: 75% of enemies
        threshold2Stars = Mathf.CeilToInt(totalEnemies * 0.75f);
    }

    /// <summary>
    /// Called when an enemy is defeated.
    /// </summary>
    public void ReportEnemyDefeated(EnemyBase enemy)
    {
        enemiesKilled++;

        // Check if this was the boss by name (not by tag since boss is tagged as "Enemy")
        if (enemy.gameObject.name.Contains("Boss") || enemy.gameObject.name.Contains("Dinosaur"))
        {
            bossDefeated = true;
            Debug.Log("🎯 Boss defeated!");
        }

        Debug.Log($"💀 Enemy killed: {enemiesKilled}");
        Debug.Log($"   [{enemy.gameObject.name}] Progress: {enemiesKilled}/{totalEnemies}");
    }

    /// <summary>
    /// Calculate the star rating based on current performance.
    /// 0 Stars: No achievements
    /// 1 Star: Boss defeated OR 35% of enemies killed
    /// 2 Stars: 75% of enemies killed
    /// 3 Stars: All enemies + Boss + Time limit (not implemented for loss)
    /// </summary>
    public int CalculateStarRating()
    {
        // Check for immediate death (0 stars)
        if (enemiesKilled == 0)
        {
            Debug.Log("⭐ Star Rating: 0 (No enemies defeated)");
            return 0;
        }

        // 1 Star: boss defeated OR threshold1Star enemies killed
        if (bossDefeated || enemiesKilled >= threshold1Star)
        {
            // 2 Stars: threshold2Stars enemies killed
            if (enemiesKilled >= threshold2Stars)
            {
                Debug.Log("⭐⭐ Star Rating: 2 (75% enemies defeated)");
                return 2;
            }

            Debug.Log("⭐ Star Rating: 1 (Boss or 35% threshold reached)");
            return 1;
        }

        // No stars if didn't reach thresholds
        Debug.Log("⭐ Star Rating: 0 (Below thresholds)");
        return 0;
    }

    /// <summary>
    /// Get the game time elapsed since start.
    /// </summary>
    public float GetGameTimeElapsed()
    {
        return Time.time - gameStartTime;
    }

    /// <summary>
    /// Check if time limit was met (for future 3-star system).
    /// </summary>
    public bool IsWithinTimeLimit()
    {
        return GetGameTimeElapsed() <= TIME_LIMIT;
    }

    // Getters for debugging and UI
    public int GetEnemiesKilled() => enemiesKilled;
    public int GetTotalEnemies() => totalEnemies;
    public bool IsBossDefeated() => bossDefeated;
    public int GetThreshold1Star() => threshold1Star;
    public int GetThreshold2Stars() => threshold2Stars;

    /// <summary>
    /// Reset state on scene restart.
    /// </summary>
    public void ResetState()
    {
        enemiesKilled = 0;
        bossDefeated = false;
        gameStartTime = Time.time;
        Debug.Log("🔄 Game state tracker reset");
    }
}
