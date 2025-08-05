using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum.Asteroids
{
    [Preserve]
    public unsafe class BoundarySystem : SystemMainThreadFilter<BoundarySystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform2D* Transform;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            AsteroidsGameConfig config = frame.FindAsset(frame.RuntimeConfig.GameConfig);

            if (IsOutOfBounds(filter.Transform->Position, config.MapExtends, out FPVector2 newPosition))
            {
                // The Teleport function on the transform is used to signal to the EntityViewInterpolator that interpolation for this movement should be skipped.
                filter.Transform->Teleport(frame, newPosition);
            }
        }

        /// <summary>
        /// Test if a position is out of bounds and provide a warped position.
        /// When the entity leaves the bounds it will emerge on the other side.
        /// </summary>
        public bool IsOutOfBounds(FPVector2 position, FPVector2 mapExtends, out FPVector2 newPosition)
        {
            newPosition = position;

            if (position.X >= -mapExtends.X && position.X <= mapExtends.X &&
                position.Y >= -mapExtends.Y && position.Y <= mapExtends.Y)
            {
                // position is inside map bounds
                return false;
            }

            // warp x position
            if (position.X < -mapExtends.X)
            {
                newPosition.X = mapExtends.X;
            }
            else if (position.X > mapExtends.X)
            {
                newPosition.X = -mapExtends.X;
            }

            // warp y position
            if (position.Y < -mapExtends.Y)
            {
                newPosition.Y = mapExtends.Y;
            }
            else if (position.Y > mapExtends.Y)
            {
                newPosition.Y = -mapExtends.Y;
            }

            return true;
        }
    }
}
