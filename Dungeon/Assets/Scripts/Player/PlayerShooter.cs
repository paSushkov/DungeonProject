using UnityEngine;


public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] float shootForce = 50f;
    private Transform camTransform;
    
    private void Start()
    {
        camTransform = Camera.main.transform;
        InputManager.Instance.ShootInputDone += Shoot;
    }

    private void Shoot()
    {
        var myProjectile = Instantiate(projectile, camTransform.position + camTransform.forward, camTransform.rotation);
        if (myProjectile.TryGetComponent(out Rigidbody projectileRb))
        {
            projectileRb.AddForce(camTransform.forward * shootForce, ForceMode.Impulse);
            
        }
    }

}
