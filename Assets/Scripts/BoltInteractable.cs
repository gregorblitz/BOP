using System;
using UnityEngine;
using Game.Interactions; 

public class BoltInteractable : MonoBehaviour, IInteractable
{
    [Header("Referencia a la Tuerca")]
    // Referencia a la tuerca asignada a este perno
    [SerializeField] private NutInteractable assignedNut;

    // Evento vital para que el BopManager sepa cuándo se completó este perno (Observer Pattern)
    public event Action OnBoltSecured;

    // Estado interno del perno
    private bool hasNut;

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