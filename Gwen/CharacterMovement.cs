using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.Physics;

namespace Gwen
{
    public class CharacterMovement : SyncScript
    {
        public Vector3 MovementMultiplier = new Vector3(3, 0, 4);
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
                this.MovementMultiplier.Z);

            var velocity = new Vector3();
            if (Input.IsKeyDown(Keys.W))
            {
                velocity.Z++;
            }
            if (Input.IsKeyDown(Keys.S))
            {
                velocity.Z--;
            }

            if (Input.IsKeyDown(Keys.A))
            {
                velocity.X++;
            }

            if (Input.IsKeyDown(Keys.LeftShift)) {
                finalMovementMultiplier = new Vector3(
                    this.MovementMultiplier.X,
                    this.MovementMultiplier.Y,
                    this.MovementMultiplier.Z * 2);
            }

            if (Input.IsKeyDown(Keys.D))
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
