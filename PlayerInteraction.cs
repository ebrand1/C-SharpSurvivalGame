using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    //Used to determine how close the player must be to an object to interact with it
    public float playerReach;

    // Update is called once per frame
    void Update()
    {
        RaycastHit targetInfo;
        if(Physics.Raycast(this.transform.position, this.transform.forward, out targetInfo, playerReach))
        {
            if (targetInfo.collider.gameObject.GetComponent<Interactable>() == null)
            {
                return;
            }
            else if (targetInfo.collider.gameObject.GetComponent<Interactable>().isInteractable)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                        targetInfo.collider.gameObject.GetComponent<Interactable>().Interact();
                }
            }
        }
    }
}
