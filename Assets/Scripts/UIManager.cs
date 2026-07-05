using System;
using System.Threading;
using UnityEngine;
using TMPro;
using Bop.Managers;

namespace Bop.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Dependencias")]
        [SerializeField] private BopManager bopManager;
        
        [Header("Elementos UI")]
        [SerializeField, Tooltip("Texto principal que guía al jugador")] 
        private TextMeshProUGUI instructionText;
        
        [SerializeField, Tooltip("Texto flotante o central para alertas de error")] 
        private TextMeshProUGUI feedbackText;

        // Token para gestionar la cancelación de la espera asíncrona si se hace clics muy rápido
        private CancellationTokenSource feedbackCts;

        private void OnEnable()
        {
            if (bopManager == null) return;
            bopManager.OnInstructionUpdated += HandleInstructionUpdated;
            bopManager.OnSequenceError += HandleSequenceError;
        }

        private void OnDisable()
        {
            if (bopManager == null) return;
            bopManager.OnInstructionUpdated -= HandleInstructionUpdated;
            bopManager.OnSequenceError -= HandleSequenceError;
            
            // Limpieza de memoria
            feedbackCts?.Cancel();
            feedbackCts?.Dispose();
        }

        private void Start()
        {
            feedbackText.gameObject.SetActive(false);
        }

        private void HandleInstructionUpdated(string message)
        {
            instructionText.text = message;
        }

        private async void HandleSequenceError(int expected, int clicked)
        {
            // Cancela cualquier mensaje de error previo que estuviera en pantalla
            feedbackCts?.Cancel();
            feedbackCts?.Dispose();
            feedbackCts = new CancellationTokenSource();

            // Vincula el token del ciclo de vida del obj con token de cancelación
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, feedbackCts.Token);

            try
            {
                feedbackText.gameObject.SetActive(true);
                // Uso de string interpolation moderna
                feedbackText.text = $"<color=#FF4C4C>¡Error de Secuencia!</color>\nClickeaste el perno #{clicked}, pero debes ajustar el perno #{expected}.";

                // Pausa asíncrona no bloqueante
                await Awaitable.WaitForSecondsAsync(5f, linkedCts.Token);

                // Oculta el texto tras 3 segundos
                feedbackText.gameObject.SetActive(false);
            }
            catch (OperationCanceledException)
            {
                
                // Se ejecuta si se destruye el objeto o si se hace otro clic incorrecto antes de los 3 segundos.
            }
        }
    }
}