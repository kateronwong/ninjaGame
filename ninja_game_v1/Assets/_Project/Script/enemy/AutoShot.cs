using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShot : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    public float timeInterval = 3f;

    private void Start()
    {
        StartCoroutine(GenerateBullets());
    }

    IEnumerator GenerateBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeInterval);
            ShootBullet();
        }
    }

    void ShootBullet()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
    }
}
