using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class PhysicManager : MonoBehaviour
{
    void OnEnable() => Physics2D.simulationMode = SimulationMode2D.Script;
    void OnDisable() => Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
}

