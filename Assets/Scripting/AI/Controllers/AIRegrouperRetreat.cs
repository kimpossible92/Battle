using UnityEngine;

namespace CoverShooter
{
    /// <summary>
    /// Triggers a regroup when the AI is retreating.
    /// </summary>
    [RequireComponent(typeof(Actories))]
    [RequireComponent(typeof(AIMovement))]
    public class AIRegrouperRetreat : AIBaseRegrouper
    {
        private void OnRetreat()
        {
            Regroup();
        }
    }
}
