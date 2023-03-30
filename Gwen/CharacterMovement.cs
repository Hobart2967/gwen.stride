using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.Physics;

namespace Gwen
{
    public class CharacterMovement : SyncScript
    {
        public int ForwardsSpeed = 1;
        public int BackwardsSpeed = 1;
        public Vector2 MovementMultiplier = new Vector2(3, 0);

        private CharacterComponent character;

        public override void Start()
        {
            character = Entity.Get<CharacterComponent>();
        }

        public override void Update()
        {
            var finalMovementMultiplier = new Vector3(
                this.MovementMultiplier.X,
                this.MovementMultiplier.Y,
                Input.IsKeyDown(Keys.S) ? this.BackwardsSpeed : this.ForwardsSpeed);

            var velocity = new Vector3();

            if (Input.IsKeyDown(Keys.W))
            {
                velocity.Z++;
            }
            if (Input.IsKeyDown(Keys.S))
            {
                velocity.Z--;
            }

            if (Input.IsKeyDown(Keys.Q))
            {
                velocity.X++;
            }

            if (Input.IsKeyDown(Keys.LeftShift)) {
                finalMovementMultiplier = new Vector3(
                    finalMovementMultiplier.X,
                    finalMovementMultiplier.Y,
                    finalMovementMultiplier.Z * 2);
            }

            if (Input.IsKeyDown(Keys.E))
            {
                velocity.X--;
            }

            velocity.Normalize();
            velocity *= finalMovementMultiplier;
            velocity = Vector3.Transform(velocity, Entity.Transform.Rotation);
            character.SetVelocity(velocity);
        }
    }
}
