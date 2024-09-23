using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KirbyAbilityManager : MonoBehaviour
{
    private ProjectileController projectileController;
    private SpitController spitController;

    private void Start()
    {
        projectileController = GetComponent<ProjectileController>();
        spitController = GetComponent<SpitController>();
    }

    public void InhaleEnemy(string enemyType)
    {
        if (enemyType == "Kibble0")
        {
            projectileController.ResetShots();
            spitController.GainStarAbility();
        }
        else if (enemyType == "Waddle0")
        {
            spitController.GainStarAbility();
        }
    }
}
