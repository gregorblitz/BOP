using UnityEngine;
using UnityEngine.InputSystem; // Requiere el paquete "Input System" instalado
using Game.Interactions;

namespace Game.Controllers
{

    // Gestiona la entrada del jugador y proyecta Raycasts para interactuar con objetos 3D.

    public class InteractionController : MonoBehaviour
    {
        [Header("Configuración de Raycast")]
        // Referencia a la cámara para calcular la trayectoria del rayo desde la pantalla
        [SerializeField, Tooltip("Cámara principal desde donde se proyecta el click.")]
        private Camera mainCamera;
        
        [SerializeField, Tooltip("Distancia máxima de interacción.")]
        private float interactRange = 100f;

        // Máscara de capa (LayerMask) para ignorar objetos no interactuables, ahorrando cálculos

        [SerializeField, Tooltip("Capas donde se encuentran los objetos interactuables para optimizar físicas.")]
        private LayerMask interactableLayer = ~0; // Por defecto: Todo

        private void Start()
        {
            // Auto-asignación de seguridad si se olvida en el Inspector
            // Si la cámara no fue asignada en el inspector la busca automáticamente
            if (mainCamera == null) mainCamera = Camera.main;
        }

        private void Update()
        {
            // Verificación de seguridad, evita errores si no hay mouse conectado
            if (Mouse.current == null) return;

            // Detección de Click Izquierdo usando New Input System
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ProcessClick(isPrimaryClick: true); 
            }
            // Detección de Click Derecho 
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                ProcessClick(isPrimaryClick: false);
            }
        }

        /// Dispara un rayo desde la posición del cursor en pantalla hacia escena 3D.
        private void ProcessClick(bool isPrimaryClick)
        {
            // Obtiene posición x,y actual del cursor en la pantalla
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            // Traduce coordenadas 2D a vector 3D (Rayo) usando la perspectiva de la cámara
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            // Ejecuta el Raycast 'out RaycastHit hit' guarda datos objeto golpeado
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                // TryGetComponent verifica si obj golpeado tiene interfaz IInteractable
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    // Ejecuta el método correspondiente según el botón presionado
                    if (isPrimaryClick)
                    {
                        interactable.OnPrimaryClick();
                    }
                    else
                    {
                        interactable.OnSecondaryClick();
                    }
                }
            }
        }
    }
}