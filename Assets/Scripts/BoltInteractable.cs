using System;
using UnityEngine;
using Game.Interactions; 
using Bop.Managers;

public class BoltInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuración de Secuencia")]
    [SerializeField, Tooltip("El orden de este perno en el patrón de apriete (1 al 8)")]
    private int sequenceIndex;

    [Header("Referencia a la Tuerca")]
    // Referencia a la tuerca asignada a este perno
    [SerializeField] private NutInteractable assignedNut;

    // Evento vital para que el BopManager sepa cuándo se completó este perno (Observer Pattern)
    public event Action OnBoltSecured;

    // Estado interno del perno
    private bool hasNut;

    private BopManager _manager;

    public void Initialize(BopManager manager)
    {
        _manager = manager;
    }

    // Suscripción de eventos en OnEnable/OnDisable
    // Suscribe al evento de la tuerca al activarse el objeto (OnEnable)
    // El operador '+=' añade 'HandleNutSecured' como escuchador del evento 'OnNutSecured'.
    private void OnEnable() => assignedNut.OnNutSecured += HandleNutSecured;

    // Desuscribe al desactivarse el objeto (OnDisable)
    private void OnDisable() => assignedNut.OnNutSecured -= HandleNutSecured;

    // Retransmite el evento: Cuando tuerca esta asegurada, perno dispara su propio evento
    private void HandleNutSecured() => OnBoltSecured?.Invoke();

    // Click Izquierdo: Colocar la tuerca
    public void OnPrimaryClick()
    {
        // Si ya tiene tuerca abortar acción
        if (hasNut) return;

        // Validación de la secuencia requerida orden establecido (1-8)
        //Envia mensaje si no se sigue la secuencia
        if (!_manager.IsNextInSequence(sequenceIndex))
        {
            Debug.LogWarning($"[Secuencia] Acción anulada. Esperando el perno #{_manager.CurrentExpectedIndex}, pero se clickeó el #{sequenceIndex}.");
            return;
        }

        hasNut = true;
        // Invoca el método de la tuerca, le pasa la posición (este perno) como padre
        assignedNut.Place(transform);
        Debug.Log($"[BoltInteractable] Tuerca transferida a {gameObject.name}. Ahora haz click derecho sobre la tuerca.");
    }

    // Click Derecho: Atornillar
    public void OnSecondaryClick()
    {
        if (!hasNut)
        {
            Debug.LogWarning("[BoltInteractable] Primero debes colocar la tuerca (Click Izquierdo).");
        }
    }
}