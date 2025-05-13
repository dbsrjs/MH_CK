using UnityEngine;

public class LockOnSystem : MonoBehaviour
{
    [Header("Lock-On Settings")]
    [SerializeField] private float lockOnRange = 15f;
    [SerializeField] private string enemyTag = "Enemy";

    private Transform lockOnTarget;
    private bool isLockingOn = false;

    void Update()
    {
        HandleLockOnInput();

        if (isLockingOn && lockOnTarget != null)
        {
            RotateToTarget();
        }
    }

    private void HandleLockOnInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isLockingOn)
                UnlockTarget();
            else
                FindLockOnTarget();
        }
    }

    private void RotateToTarget()
    {
        Vector3 direction = (lockOnTarget.position - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void FindLockOnTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistanceSqr = lockOnRange * lockOnRange;
        Transform closestTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceSqr = (enemy.transform.position - transform.position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestTarget = enemy.transform;
            }
        }

        if (closestTarget != null)
        {
            lockOnTarget = closestTarget;
            isLockingOn = true;
        }
    }

    private void UnlockTarget()
    {
        lockOnTarget = null;
        isLockingOn = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);
    }
}
