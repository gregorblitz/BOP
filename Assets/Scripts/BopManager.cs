using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bop.Managers
{
    public class BopManager : MonoBehaviour
    {
        [Header("Referencias del Nivel")]
        [SerializeField] private List<BoltInteractable> pernosEnNivel = new();

        [Header("Eventos")]
        // Conecta respuestas desde la interfaz gráfica del Editor con UnityEvent
        public UnityEvent OnLevelCompleted;

        private int pernosApretados;

        private void OnEnable()
        {
            // Bucle para suscribir el Manager al evento de éxito de cada perno en la lista
            foreach (BoltInteractable perno in pernosEnNivel)
            {
                perno.OnBoltSecured += HandleBoltSecured;
            }
        }

        private void OnDisable()
        {
            // Bucle para desuscribir de todos los pernos y mantener memoria limpia
            foreach (BoltInteractable perno in pernosEnNivel)
            {
                perno.OnBoltSecured -= HandleBoltSecured;
            }
        }

        // Event Handler: Se dispara cuando un perno se reporta asegurado
        private void HandleBoltSecured()
        {
            pernosApretados++; // Aumenta contador de progreso

            // Condicion: se han apretado todos los pernos?
            if (pernosApretados >= pernosEnNivel.Count)
            {
                // Disparamos secuencia de fin de nivel de forma asíncrona
                ExecuteWinSequenceAsync();
            }
        }

        // Async indica que contiene código asíncrono.
        private async void ExecuteWinSequenceAsync()
        {
            Debug.Log("Todos los tornillos están apretados.");
            await Awaitable.WaitForSecondsAsync(1.5f); // Pausa cinemática
            OnLevelCompleted?.Invoke();
        }
    }
}