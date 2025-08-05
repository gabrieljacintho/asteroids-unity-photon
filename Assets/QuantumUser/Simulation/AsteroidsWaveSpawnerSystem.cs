using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum.Asteroids
{
    [Preserve]
    public unsafe class AsteroidsWaveSpawnerSystem : SystemSignalsOnly
    {
        public override void OnInit(Frame frame)
        {
            SpawnAsteroidWave(frame);
        }

        public void SpawnAsteroid(Frame frame, AssetRef<EntityPrototype> childPrototype)
        {
            AsteroidsGameConfig config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            EntityRef asteroid = frame.Create(childPrototype);
            Transform2D* asteroidTransform = frame.Unsafe.GetPointer<Transform2D>(asteroid);

            asteroidTransform->Position = GetRandomEdgePointOnCircle(frame, config.AsteroidSpawnDistanceToCenter);
            asteroidTransform->Rotation = GetRandomRotation(frame);

            if (frame.Unsafe.TryGetPointer<PhysicsBody2D>(asteroid, out var body))
            {
                body->Velocity = asteroidTransform->Up * config.AsteroidInitialSpeed;
                body->AddTorque(frame.RNG->Next(config.AsteroidInitialTorqueMin, config.AsteroidInitialTorqueMax));
            }
        }

        public static FP GetRandomRotation(Frame frame)
        {
            return frame.RNG->Next(0, 360);
        }

        public static FPVector2 GetRandomEdgePointOnCircle(Frame frame, FP radius)
        {
            return FPVector2.Rotate(FPVector2.Up * radius, frame.RNG->Next() * FP.PiTimes2);
        }

        private void SpawnAsteroidWave(Frame frame)
        {
            AsteroidsGameConfig config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            for (int i = 0; i < frame.Global->AsteroidsWaveCount + config.InitialAsteroidsCount; i++)
            {
                SpawnAsteroid(frame, config.AsteroidPrototype);
            }

            frame.Global->AsteroidsWaveCount++;
        }
    }
}
