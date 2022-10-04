using UnityEngine;
/// <summary>
/// Interfaccia per tutti gli oggetti dameggiabiili
/// </summary>
public interface IDamageable {
    public int Health{set; get;}
    public void OnHit(int damage, Vector2 knockback);
    /// <summary>
    /// Uguale alla funzione precedente senza il calcolo del knockback
    /// </summary>
    /// <param name="damage"></param>
    public void OnHit(int damage);

}