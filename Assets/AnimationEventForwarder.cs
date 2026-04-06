using UnityEngine;

/// <summary>
/// Forwards animator events from child GameObjects to parent DinosaurBehaviour.
/// Solves the issue where Animator is on a child but the damage method is on the parent.
/// </summary>
public class AnimationEventForwarder : MonoBehaviour
{
    private DinosaurBehaviour _parentDinosaurBehaviour;

    private void Awake()
    {
        // Cache the parent's DinosaurBehaviour component
        _parentDinosaurBehaviour = GetComponentInParent<DinosaurBehaviour>();

        if (_parentDinosaurBehaviour == null)
        {
            Debug.LogError("AnimationEventForwarder: Could not find DinosaurBehaviour in parent! " +
                "Make sure this script is on a child of the Dinosaur GameObject.", gameObject);
        }
    }

    /// <summary>
    /// Called by the RedAttack animation event.
    /// Forwards the call to the parent DinosaurBehaviour's RedAttackDamage() method.
    /// </summary>
    public void TriggerRedAttackDamage()
    {
        if (_parentDinosaurBehaviour == null)
        {
            Debug.LogWarning("AnimationEventForwarder: Parent DinosaurBehaviour not found!", gameObject);
            return;
        }

        _parentDinosaurBehaviour.RedAttackDamage();
    }
}
