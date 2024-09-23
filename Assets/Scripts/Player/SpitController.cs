using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitController : MonoBehaviour
{
    public GameObject Spit;
    private bool canSpit = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && canSpit)
        {
            SpitStar();
        }
    }

    public void GainStarAbility()
    {
        canSpit = true;
    }

    void SpitStar()
    {
        Instantiate(Spit, transform.position, transform.rotation);
        canSpit = false;
        Destroy(Spit);
        Debug.Log("Spit the star");
    }

}
