using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    private Vector3 direction;

    private void OnEnable()
    {
        direction = transform.rotation * Vector3.forward;
        direction = new Vector3(-direction.z, 0f, direction.x);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player")) other.GetComponentInParent<Player>().DecrementHealth();
        if (!other.gameObject.tag.Equals("Enemy")) gameObject.SetActive(false);
    }
}
