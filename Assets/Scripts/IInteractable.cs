using UnityEngine;

namespace Game.Interactions
{
    // Contrato para cualquier objeto que pueda recibir interacciones de click.
    // Define las acciones estándar sin importar si el objeto es una tuerca, perno, etc.
    public interface IInteractable
    {
        // Método que se ejecutará al recibir un Click Izquierdo del ratón
        void OnPrimaryClick();   // Click Izquierdo
        // Método que se ejecutará al recibir un Click Derecho del ratón
        void OnSecondaryClick(); // Click Derecho
    }
}