using System;
using UnityEngine;

namespace Sloop.Economy
{
    [Serializable]
    public struct ResourceAmount
    {
        public Resource type;
        [Min(0)] public int amount;
    }
}
