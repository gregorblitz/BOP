using UnityEngine;

public class NutController : MonoBehaviour
{
    private Animator animator;
    
    // Almacenamos el Hash del Trigger
    private readonly int screwTriggerHash = Animator.StringToHash("Screw");

    private void Awake()
    {
        // Referencia el componente al inicio
        animator = GetComponent<Animator>();
    }

    // Emparenta y posiciona la tuerca en el perno objetivo sin animarla.
    public void Place(Transform targetScrew)
    {
        // Emparenta la tuerca al perno
        // soluciona bugs visuales al emparentar objetos 3D.
        transform.SetParent(targetScrew, false);

        // Resetea coordenadas locales para que coincidan con la animación
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Activa el modelo de la tuerca
        gameObject.SetActive(true);
    }

    // Dispara la animación en el Animator Controller.

    public void AnimateScrewing()
    {
        // Verifica si existe un controlador de animación para evitar errores
        if (animator.runtimeAnimatorController != null)
        {
            // Activa el Trigger usando el Hash en lugar del string directo "Screw"
            animator.SetTrigger(screwTriggerHash);
        }
        else
        {
            // Si no hay controlador envia mensaje error
            Debug.LogError($"[NutController] El Animator en {gameObject.name} no tiene un Controller asignado.");
        }
    }
}