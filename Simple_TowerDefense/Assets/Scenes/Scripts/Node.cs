﻿using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color notEnoughMoneyColor;
    public Vector3 positionOffset;

    //Only displays a bit of text 'node' inspector in the editor, help ourselves
    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBlueprint turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    private Renderer rend;
    private Color startColor;

    BuildManager buildManager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;
    }
    public Vector3 GetBuildPosition()
    {
        TurretBlueprint _blueprint = buildManager.GetTowerToBuild();
        
        if (_blueprint == null)
        {
            //업그레이드 한 경우 빌드UI에서 택한 BuildManager 의 _blueprint 요소가 없어짐. 
            //따라서 남아있는 turretBlueprint 요소에서 position 값을 들고옴
            if (!isUpgraded)
                positionOffset = turretBlueprint.prefab.transform.position;
            else
                positionOffset = turretBlueprint.upgradedPrefab.transform.position;

            return transform.position + positionOffset;
        }
        if(!isUpgraded)
            positionOffset = _blueprint.prefab.transform.position;
        else
            positionOffset = _blueprint.upgradedPrefab.transform.position;

        return transform.position + positionOffset;
    }

    private void OnMouseDown()
    {
        //turret을 select후 shop UI와 node가 간섭끼치지 않게 하기 위한 코드
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if(turret != null)
        {
            buildManager.SelectNode(this);
            return;
        }

        if (!buildManager.CanBuild)
            return;

        BuildTurret(buildManager.GetTowerToBuild());
    }

    void BuildTurret (TurretBlueprint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("Not enough money to build turret!");
            return;
        }
        PlayerStats.Money -= blueprint.cost;

        GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f); //Destroy effect after 5 seconds

        Debug.Log("Turret build!" );
    }

    public void UpgradeTurret()
    {
        if (PlayerStats.Money < turretBlueprint.upgradeCost)
        {
            Debug.Log("Not enough money to upgrade turret!");
            return;
        }
        PlayerStats.Money -= turretBlueprint.upgradeCost;

        //Get rid of the old turret
        Destroy(turret);

        isUpgraded = true;

        //Build a new one
        GameObject _turret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f); //Destroy effect after 5 seconds

        Debug.Log("Turret upgraded!");
    }

    public void SellTurret()
    {
        if (!isUpgraded) { //업그레이드 안된 경우
            //Destroy current Turret and earn some amount of money
            PlayerStats.Money += turretBlueprint.GetSellAmount();

        }
        else
        { //업그레이드 된 경우
            PlayerStats.Money += turretBlueprint.Get_UpgradeSellAmount();
        }

        //Spawn a cool effect
        GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f); //Destroy effect after 5 seconds

        Destroy(turret);
        isUpgraded = false;
        turretBlueprint = null;

    }

    private void OnMouseEnter()
    {
        //turret을 select후 shop UI와 node가 간섭끼치지 않게 하기 위한 코드
        if (EventSystem.current.IsPointerOverGameObject())
            return; 

        //To make sure that we will only highlight the different notes when we hover over them
        if (!buildManager.CanBuild)
            return;

        if (buildManager.HasMoney)
        {
            rend.material.color = hoverColor;
        }
        else
        {
            rend.material.color = notEnoughMoneyColor;
        }
    }
    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }
}
