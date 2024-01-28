using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawn_vfx;
    public float range;
    // Start is called before the first frame update
    public void Spawn(GameObject prefab)
    {
        Vector2 offset = Extentions.FromAngle(Random.Range(0, 360) * Mathf.Deg2Rad, Random.Range(0.0f, range));
        Instantiate(prefab, transform.position + new Vector3(offset.x, 0, offset.y), Quaternion.Euler(0, Random.Range(0, 360), 0));
        Instantiate(spawn_vfx, transform.position + new Vector3(offset.x, 1, offset.y), Quaternion.Euler(0, Random.Range(0, 360), 0));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Extentions.GizmosDrawCircle(transform.position, range);
    }
}
