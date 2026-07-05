using UnityEngine;
using Bop.Managers;

namespace Bop.UI
{
    public class SequenceIndicator : MonoBehaviour
    {
        [Header("Dependencias")]
        [SerializeField, Tooltip("Referencia al Manager central")] 
        private BopManager bopManager;

        [Header("Configuración de Animación")]
        [SerializeField] private float fixedPlaneY = 0.005f; // Define el piso virtual

        [SerializeField, Tooltip("Desplazamiento en el eje Z para no tapar el perno")] 
        private float zOffset = 0.5f;
        [SerializeField] private float bobSpeed = 4f; // velocidad rebote
        [SerializeField] private float bobHeight = 0.1f; // rebote en eje z

        [SerializeField, Tooltip("Velocidad de traslado horizontal entre pernos")] 
        private float transitionSpeed = 10f; // Velocidad de la interpolación

        private Transform currentTarget;
        // Almacena la posición plana en (X, 0, Z base) para la interpolación
        private Vector3 currentTrackingPos;

        private void OnEnable()
        {
            if (bopManager != null)
                bopManager.OnTargetChanged += HandleTargetChanged;
        }

        private void OnDisable()
        {
            if (bopManager != null)
                bopManager.OnTargetChanged -= HandleTargetChanged;
        }

        private void HandleTargetChanged(Transform newTarget)
        {
            bool wasNull = currentTarget == null;
            currentTarget = newTarget;

            // Activa o desactiva el GameObject de la flecha según haya un objetivo
            gameObject.SetActive(currentTarget != null);
            
            // Si acaba de aparecer (primer perno) se salta la transición y hace snap instantáneo
            if (wasNull && currentTarget != null)
            {
                currentTrackingPos = new Vector3(currentTarget.position.x, 0f, currentTarget.position.z + zOffset);
            }
        }

        // Usamos LateUpdate para asegurar que calculamos sobre la posición final del objetivo 
        // en caso de que el objeto padre (la válvula BOP) se estuviera moviendo en este frame.
        private void LateUpdate()
        {
            if (currentTarget == null) return;

            Vector3 targetPos = currentTarget.position;

            // Calcula la posición objetivo base con offset en Z para no tapar perno
            Vector3 targetTrackingPos = new Vector3(targetPos.x, 0f, targetPos.z + zOffset);
            
            // Interpolación suave (Lerp) de seguimiento en X y Z base
            currentTrackingPos = Vector3.Lerp(currentTrackingPos, targetTrackingPos, Time.deltaTime * transitionSpeed);

            // Aplica el rebote (salto) dinámico sumándolo a la posición Z actual
            float dynamicZ = currentTrackingPos.z + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            
            // Fusión final de matrices: Y queda inamovible usando fixedPlaneY
            transform.position = new Vector3(currentTrackingPos.x, fixedPlaneY, dynamicZ);
            
        }
    }
}
