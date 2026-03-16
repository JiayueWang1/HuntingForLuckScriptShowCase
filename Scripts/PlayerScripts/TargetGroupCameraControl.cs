using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MetroidvaniaTools;

public class TargetGroupCameraControl : MonoBehaviour {
    public Transform Player;
    public Transform Boss1;
    public Transform Boss2;
    public CinemachineTargetGroup targetGroup;
    public float playerRadiusBeforeAwake;
    public float playerRadiusAfterAwake;
    public float bossRadiusAwake;
    public float playerWeight;
    public float bossWeight;
    private void Start() {
        List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>();
        targets.Add(new CinemachineTargetGroup.Target{target = Player, radius = playerRadiusBeforeAwake, weight = 1});
        targetGroup.m_Targets = targets.ToArray();
        LSleep.SleepEvent += AddBossToTargetGroup;
    }

    public void AddBossToTargetGroup() {
        List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>();
        targets.Add(new CinemachineTargetGroup.Target{target = Player, radius = playerRadiusAfterAwake, weight = playerWeight});
        targets.Add(new CinemachineTargetGroup.Target{target = Boss1, radius = bossRadiusAwake, weight = bossWeight});
        targets.Add(new CinemachineTargetGroup.Target{target = Boss2, radius = bossRadiusAwake, weight = bossWeight});
        targetGroup.m_Targets = targets.ToArray();
    }
}
