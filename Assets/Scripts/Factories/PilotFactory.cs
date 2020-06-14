﻿using System;
using UnityEngine;
public class PilotFactory
{
    public static PlayerPilotModelView CreatePlayerPilotModelView(Transform parentShip)
    {
        GameObject testPlayerPilotPrefab = Resources.Load<GameObject>("Prefabs/Pilot/TestPlayer");
        PlayerPilotModelView modelView = UnityEngine.Object.Instantiate(testPlayerPilotPrefab, parentShip)
            .GetComponent<PlayerPilotModelView>();
        return modelView;
    }

    public static PlayerPilotController CreatePlayerPilotController(PlayerPilotModelView playerMV, ShipModelView shipMV, TrackPath checkpoints, DirectionArrowModelView dirArrowHUD)
    {
        return new PlayerPilotController(playerMV, shipMV, checkpoints, dirArrowHUD);
    }

    public static EnemyPilotModelView CreateEnemyPilotModelView(Transform parentShip)
    {
        GameObject testEnemyPilotPrefab = Resources.Load<GameObject>("Prefabs/Pilot/TestEnemy");
        EnemyPilotModelView modelView = UnityEngine.Object.Instantiate(testEnemyPilotPrefab,parentShip.position + Vector3.up * 2f, Quaternion.identity, parentShip.transform)
            .GetComponent<EnemyPilotModelView>();
        return modelView;
    }
    
    public static EnemyPilotController CreateEnemyPilotController(EnemyPilotModelView enemyMV, ShipModelView shipMV, TrackPath checkpoints)
    {
        return new EnemyPilotController(enemyMV, shipMV, checkpoints);
    }
    
}
