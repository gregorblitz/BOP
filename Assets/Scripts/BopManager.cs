using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bop.Managers
{
    public class BopManager : MonoBehaviour
    {
        [Header("Referencias del Nivel")]

        // Hace una lista con los pernos en el orden requerido y se asignan en el inspector (obj vacio BopManager)
        [SerializeField] private List<BoltInteractable> pernosEnNivel = new();

        [Header("Eventos")]
        // Conecta respuestas desde la interfaz gráfica del Editor con UnityEvent
        public UnityEvent OnLevelCompleted;

        // Evento que notifica a la UI la posición del perno actual
        public event Action<Transform> OnTargetChanged;

        // Eventos para la UI
        public event Action<string> OnInstructionUpdated;
        public event Action<int, int> OnSequenceError;

        private int pernosApretados;

        // Expone el índice esperado actual
        public int CurrentExpectedIndex => pernosApretados + 1;

        private void Awake()
        {
            // Inyección de dependencia: pasamos este manager a cada perno al inicio
            foreach (BoltInteractable perno in pernosEnNivel)
            {
                perno.Initialize(this);
            }
        }

        // Emite el primer perno una vez que todos los objetos se inicializaron
        private void Start()
        {
            UpdateIndicator();
        }

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

        // Método de validación consultado por los pernos
        public bool IsNextInSequence(int sequenceIndex)
        {
            return sequenceIndex == CurrentExpectedIndex;
        }
        // Métodos públicos para ser invocados por los pernos
        public void ReportSequenceError(int clickedIndex) => OnSequenceError?.Invoke(CurrentExpectedIndex, clickedIndex);
        
        public void UpdateInstruction(string message) => OnInstructionUpdated?.Invoke(message);

        // Event Handler: Se dispara cuando un perno se reporta asegurado
        private void HandleBoltSecured()
        {
            pernosApretados++; // Aumenta contador de progreso

            // Actualiza la UI al siguiente perno
            UpdateIndicator();

            // Condicion: se han apretado todos los pernos?
            if (pernosApretados >= pernosEnNivel.Count)
            {
                // Disparamos secuencia de fin de nivel de forma asíncrona
                ExecuteWinSequenceAsync();
            }
        }

        // Determina cual es el siguiente objeto lógico en la secuencia
        private void UpdateIndicator()
        {
            // Bucle sobre la lista del orden de los pernos
            foreach (BoltInteractable perno in pernosEnNivel)
            {
                //Compara coincidencias de los indices del perno seleccionado y el de la secuencia
                if (perno.SequenceIndex == CurrentExpectedIndex)
                {
                    // Verifica si hay script suscrito al evento OnTargetChanged y envia transform a suscriptores
                    OnTargetChanged?.Invoke(perno.transform);
                    // Instrucción por defecto al pasar a un nuevo perno
                    UpdateInstruction($"Oprime Click Izquierdo en el perno #{CurrentExpectedIndex} indicado por la flecha para poner la tuerca.");
                    return;
                }
            }

            // Si no hay más pernos, oculta el indicador
            OnTargetChanged?.Invoke(null);
            UpdateInstruction("¡Secuencia completada con éxito!");
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