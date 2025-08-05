using UnityEngine.Scripting;

namespace Quantum.Asteroids
{
    [Preserve]
    public unsafe class AsteroidsCollisionSystem : SystemSignalsOnly, ISignalOnCollisionEnter2D
    {
        public void OnCollisionEnter2D(Frame frame, CollisionInfo2D info)
        {
            // Projectile is colliding with something
            if (frame.Unsafe.TryGetPointer<AsteroidsProjectile>(info.Entity, out var projectile))
            {
                if (frame.Unsafe.TryGetPointer<AsteroidsShip>(info.Other, out var ship))
                {
                    frame.Signals.OnCollisionProjectileHitShip(info, projectile, ship);

                }
                else if (frame.Unsafe.TryGetPointer<AsteroidsAsteroid>(info.Other, out var asteroid))
                {
                    frame.Signals.OnCollisionProjectileHitAsteroid(info, projectile, asteroid);
                }
            }
            // Ship is colliding with something
            else if (frame.Unsafe.TryGetPointer<AsteroidsShip>(info.Entity, out var ship))
            {
                if (frame.Unsafe.TryGetPointer<AsteroidsAsteroid>(info.Other, out var asteroid))
                {
                    frame.Signals.OnCollisionAsteroidHitShip(info, ship, asteroid);
                }
            }
        }
    }
}
