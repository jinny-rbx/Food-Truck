using UnityEngine;

public class badFood : MonoBehaviour
{
    public float points;
    public float energy;
    public GameObject obj;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            PlayerControl player = other.GetComponent<PlayerControl>();
            player.EnergyManager(-energy);
            player.SatisfactionManager(points);
            Destroy(obj);
        }
    }
}