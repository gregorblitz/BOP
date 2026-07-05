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
        [SerializeField] private float yOffset = 0.5f;
        [SerializeField] private float bobSpeed = 4f;
        [SerializeField] private float bobHeight = 0.1f;

        private Transform currentTarget;
        private float baseY;

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
            currentTarget = newTarget;
            
            // Activa o desactiva el GameObject de la flecha según haya un objetivo
            gameObject.SetActive(currentTarget != null);
        }

        // Usamos LateUpdate para asegurar que calculamos sobre la posición final del objetivo 
        // en caso de que el objeto padre (la válvula BOP) se estuviera moviendo en este frame.
        private void LateUpdate()
        {
            if (currentTarget == null) return;

            Vector3 targetPos = currentTarget.position;
            baseY = targetPos.y + yOffset;
            
            // Animación matemática (Zero Allocation)
            float dynamicY = baseY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            
            transform.position = new Vector3(targetPos.x, dynamicY, targetPos.z);
            
            // Descomenta la siguiente línea si el indicador es un Sprite 2D (Billboard) 
            // para que siempre mire hacia la cámara del jugador.
            // transform.forward = Camera.main.transform.forward;
        }
    }
}
