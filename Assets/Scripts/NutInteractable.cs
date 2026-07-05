using System;
using UnityEngine;
using Game.Interactions;

public class NutInteractable : MonoBehaviour, IInteractable
{
    private Animator animator;
    private readonly int screwTriggerHash = Animator.StringToHash("Screw");

    // Evento de tipo Action. Patrón Observador
    // Avisa a los suscriptores cuando la tuerca este asegurada.
    public event Action OnNutSecured;
    
    // Evita lógicas duplicadas
    private bool isSecured;

    [Header("Configuración de Animación")]
    [SerializeField, Tooltip("Tiempo en segundos que dura la animación de atornillado")]
    private float animationDuration = 1.5f; // Ajustar en el Inspector según duracion animación atornillado

    // Referencia el componente al inicio
    private void Awake()
    {
        // Referencia el componente al inicio
        animator = GetComponent<Animator>();
    }

    // Asocia tuerca al perno. Llamado por el BoltInteractable con Click Izquierdo.
    public void Place(Transform targetScrew)
    {
        transform.SetParent(targetScrew, false);
        // Asigna posición y rotación en una sola llamada
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        gameObject.SetActive(true);
    }

    // Click Izquierdo en la tuerca: No hace nada (ya está colocada)
    public void OnPrimaryClick() { }

    // Click Derecho en la tuerca: Atornillar
    public async void OnSecondaryClick()
    {
        // Si ya está asegurada evita doble atornillado
        if (isSecured) return;

        isSecured = true;

        if (animator.runtimeAnimatorController != null)
        {
            animator.SetTrigger(screwTriggerHash);
        }

        try
        {
            // Espera la duración de la animación sin bloquear el hilo.
            // destroyCancellationToken cancela automáticamente la espera si el objeto es destruido.
            await Awaitable.WaitForSecondsAsync(animationDuration, destroyCancellationToken);

            // Una vez que el Awaitable termina (animación completada) dispara el evento
            OnNutSecured?.Invoke();
            Debug.Log($"[NutInteractable] Tuerca {gameObject.name} atornillada exitosamente.");
        }
        catch (OperationCanceledException)
        {
            // Captura segura: Ocurre si la escena cambia o el objeto se destruye antes de terminar el await.
            Debug.Log($"[NutInteractable] Animación cancelada para {gameObject.name}.");
        }
    }
}
