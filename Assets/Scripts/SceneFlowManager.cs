using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bop.Managers
{
    // Maneja el flujo de las escenas y reinicios del nivel.
    public class SceneFlowManager : MonoBehaviour
    {
        [Header("Configuración de Reinicio")]
        [SerializeField, Tooltip("Retraso en segundos antes de reiniciar la escena automáticamente (0 para instantáneo).")]
        private float restartDelay = 2f;

        // Método público para ser llamado desde un Botón UI.
        //Reinicia la escena instantáneamente.
        public void RestartScene()
        {
            // Recarga la escena activa usando su nombre actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Método asíncrono para ser llamado por eventos (como OnLevelCompleted).
        // Permite un retraso para que el jugador lea el mensaje final antes de recargar.
        public async void RestartSceneWithDelayAsync()
        {
            try
            {
                // Awaitable nativo para evitar bloqueos del hilo principal
                await Awaitable.WaitForSecondsAsync(restartDelay, destroyCancellationToken);
                RestartScene();
            }
            catch (System.OperationCanceledException)
            {
                // Silencia la cancelación si el objeto se destruye antes de tiempo
                Debug.Log("[SceneFlowManager] Reinicio cancelado.");
            }
        }
    }
}