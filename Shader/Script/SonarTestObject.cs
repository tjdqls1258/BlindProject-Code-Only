using UnityEngine;

public class SonarTestObject : MonoBehaviour
{
    [SerializeField] private SonarManager SonarManager;

    [ContextMenu("RandomPositionSet")]
    public void RandomPosition()
    {
        transform.position = new Vector3(Random.Range(-50,50), Random.Range(10,15), Random.Range(-50,50));
        SonarManager = FindAnyObjectByType<SonarManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //SonarManager.SonarAdd(transform.position, 50);
    }
}
