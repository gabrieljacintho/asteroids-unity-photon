using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum.Asteroids
{
    [Preserve]
    public unsafe class AsteroidsProjectileSystem : SystemMainThreadFilter<AsteroidsProjectileSystem.Filter>, ISignalAsteroidsShipShoot,
        ISignalOnCollisionProjectileHitShip, ISignalOnCollisionProjectileHitAsteroid
    {
        public struct Filter
        {
            public EntityRef Entity;
            public AsteroidsProjectile* Projectile;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            filter.Projectile->TTL -= frame.DeltaTime;
            if (filter.Projectile->TTL <= 0)
            {
                frame.Destroy(filter.Entity);
            }
        }

        public void AsteroidsShipShoot(Frame frame, EntityRef owner, FPVector2 spawnPosition, AssetRef<EntityPrototype> projectilePrototype)
        {
            EntityRef projectileEntity = frame.Create(projectilePrototype);
            Transform2D* projectileTransform = frame.Unsafe.GetPointer<Transform2D>(projectileEntity);
            Transform2D* ownerTransform = frame.Unsafe.GetPointer<Transform2D>(owner);

            projectileTransform->Position = spawnPosition;
            projectileTransform->Rotation = ownerTransform->Rotation;

            AsteroidsProjectile* projectile = frame.Unsafe.GetPointer<AsteroidsProjectile>(projectileEntity);
            var config = frame.FindAsset(projectile->ProjectileConfig);
            projectile->TTL = config.ProjectileTTL;
            projectile->Owner = owner;

            PhysicsBody2D* body = frame.Unsafe.GetPointer<PhysicsBody2D>(projectileEntity);
            body->Velocity = ownerTransform->Up * config.ProjectileInitialSpeed;
        }

        public void OnCollisionProjectileHitShip(Frame frame, CollisionInfo2D info, AsteroidsProjectile* projectile, AsteroidsShip* ship)
        {
            if (projectile->Owner == info.Other)
            {
                info.IgnoreCollision = true;
                return;
            }

            frame.Destroy(info.Entity);
        }

        public void OnCollisionProjectileHitAsteroid(Frame frame, CollisionInfo2D info, AsteroidsProjectile* projectile, AsteroidsAsteroid* asteroid)
        {
            if (asteroid->ChildAsteroid != null)
            {
                frame.Signals.SpawnAsteroid(asteroid->ChildAsteroid, info.Other);
                frame.Signals.SpawnAsteroid(asteroid->ChildAsteroid, info.Other);
            }

            frame.Destroy(info.Entity);
            frame.Destroy(info.Other);
        }
    }
}
