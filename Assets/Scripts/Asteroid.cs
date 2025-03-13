using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float health = 2f; // Giá trị mặc định, sẽ được cập nhật bởi AsteroidSpawner
    public bool isBigAsteroid;
    [SerializeField] private GameObject[] powerUpPrefabs;
    [SerializeField] private ParticleSystem explosionPrefab; // Prefab hiệu ứng nổ


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            float damage = collision.GetComponent<ProjectileController>().GetDamage();
            health -= damage;
            Debug.Log($"Asteroid hit! Health: {health}, Damage: {damage}");
            Destroy(collision.gameObject);
            
            if (health <= 0)
            {
                // Tạo hiệu ứng nổ trước khi hủy thiên thạch
                if (explosionPrefab != null)
                {
                    Debug.Log("Explosion");
                    ParticleSystem explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    explosion.Play();
                }
                if (isBigAsteroid)
                {
                    GameManager.Instance.AddScore(100);
                    SpawnPowerUp(0.3f);
                }
                else
                {
                    GameManager.Instance.AddScore(50);
                    SpawnPowerUp(0.2f);
                }
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayAsteroidDestroySFX(); // Âm thanh phá hủy thiên thạch
                }
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Deadzone"))
        {
            Destroy(gameObject);
        }
    }

    private void SpawnPowerUp(float dropChance)
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length < 4)
        {
            Debug.LogWarning("PowerUpPrefabs is not properly set or has insufficient elements! Current length: " + (powerUpPrefabs?.Length ?? 0));
            return;
        }

        if (Random.value <= dropChance)
        {
            int powerUpIndex = GetRandomPowerUpIndex();
            Debug.Log("Spawning power-up at index: " + powerUpIndex);
            GameObject powerUp = Instantiate(powerUpPrefabs[powerUpIndex], transform.position, Quaternion.identity);
            PowerUp powerUpScript = powerUp.GetComponent<PowerUp>();
            if (powerUpScript != null)
            {
                powerUpScript.SetPowerUpType((PowerUp.PowerUpType)powerUpIndex);
            }
            else
            {
                Debug.LogError("PowerUp script not found on prefab at index " + powerUpIndex);
            }
        }
    }

    private int GetRandomPowerUpIndex()
    {
        float roll = Random.value;
        if (roll < 0.5f) return (int)PowerUp.PowerUpType.Star;
        else if (roll < 0.75f) return (int)PowerUp.PowerUpType.Shield;
        else if (roll < 0.9f) return (int)PowerUp.PowerUpType.UpgradeShot;
        else return (int)PowerUp.PowerUpType.Invincibility;
    }

    // Phương thức để cập nhật máu từ AsteroidSpawner
    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }
}