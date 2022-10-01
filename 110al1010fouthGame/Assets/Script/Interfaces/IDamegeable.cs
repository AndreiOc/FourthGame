using UnityEngine;
/// <summary>
/// Interfaccia per tutti gli oggetti dameggiabiili
/// </summary>
public interface IDamageable {
    public float Health{set; get;}
    public void OnHit(float damage, Vector2 knockback);
    /// <summary>
    /// Uguale alla funzione precedente senza il calcolo del knockback
    /// </summary>
    /// <param name="damage"></param>
    public void OnHit(float damage);

}