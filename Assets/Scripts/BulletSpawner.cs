using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private Bullet Prefab;
    [SerializeField]
    private BoxCollider SpawnArea;
    [SerializeField]
    private int BulletsPerSecond = 10;
    [SerializeField]
    private float Speed = 5f;
    [SerializeField]
    private bool UseObjectPool = false;

    private ObjectPool<Bullet> BulletPool;

    private float LastSpawnTime;

    private void Awake()
    {
        BulletPool = new ObjectPool<Bullet>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, false, 200, 100_000);
    }

    private Bullet CreatePooledObject()
    {
        Bullet instance = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
        instance.Disable += ReturnObjectToPool;
        instance.gameObject.SetActive(false);

        return instance;
    }

    private void ReturnObjectToPool(Bullet Instance)
    {
        BulletPool.Release(Instance);
    }

    private void OnTakeFromPool(Bullet Instance)
    {
        Instance.gameObject.SetActive(true);
        SpawnBullet(Instance);
        Instance.transform.SetParent(transform, true);
    }

    private void OnReturnToPool(Bullet Instance)
    {
        Instance.gameObject.SetActive(false);
    }

    private void OnDestroyObject(Bullet Instance)
    {
        Destroy(Instance.gameObject);
    }

    private void OnGUI()
    {
        if (UseObjectPool)
        {
            GUI.Label(new Rect(10, 10, 200, 30), $"Total Pool Size: {BulletPool.CountAll}");
            GUI.Label(new Rect(10, 30, 200, 30), $"Active Objects: {BulletPool.CountActive}");
        }
    }

    private void Update()
    {
        float delay = 1f / BulletsPerSecond;
        if (LastSpawnTime + delay < Time.time)
        {
            int bulletsToSpawnInFrame = Mathf.CeilToInt(Time.deltaTime / delay);
            while (bulletsToSpawnInFrame > 0)
            {
                if (!UseObjectPool)
                {
                    Bullet instance = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
                    instance.transform.SetParent(transform, true);

                    SpawnBullet(instance);
                }
                else
                {
                    BulletPool.Get();
                }

                bulletsToSpawnInFrame--;
            }

            LastSpawnTime = Time.time;
        }
    }

    private void SpawnBullet(Bullet Instance)
    {
        Vector3 spawnLocation = new Vector3(
            SpawnArea.transform.position.x + SpawnArea.center.x + Random.Range(-1 * SpawnArea.bounds.extents.x, SpawnArea.bounds.extents.x),
            SpawnArea.transform.position.y + SpawnArea.center.y + Random.Range(-1 * SpawnArea.bounds.extents.y, SpawnArea.bounds.extents.y),
            SpawnArea.transform.position.z + SpawnArea.center.z + Random.Range(-1 * SpawnArea.bounds.extents.z, SpawnArea.bounds.extents.z)
        );

        Instance.transform.position = spawnLocation;

        Instance.Shoot(spawnLocation, SpawnArea.transform.right, Speed);
    }

}
