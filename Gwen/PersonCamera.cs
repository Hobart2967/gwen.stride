using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.Physics;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Gwen
{
    public class PersonCamera : SyncScript
    {
        public float MouseSpeed = 0.6f;
        public float MaxLookUpAngle = -50;
        public float MaxLookDownAngle = 50;
        public bool InvertMouseY = false;
        public float MaxCameraDistance = 20;

        private Entity firstPersonCameraPivot;
        private Entity thirdPersonCameraPivot;
        private Vector3 camRotation;
        private bool isActive = false;
        private Vector2 maxCameraAnglesRadians;
        private CharacterComponent character;
        private Vector3 _cameraOffset = new Vector3(0, 0, -3);
        
        public override void Start()
        {
            firstPersonCameraPivot = Entity.FindChild("Pivot");
            thirdPersonCameraPivot = Entity.FindChild("ThirdPerson");

            // Convert the Max camera angles from Degress to Radions
            maxCameraAnglesRadians = new Vector2(MathUtil.DegreesToRadians(MaxLookUpAngle), MathUtil.DegreesToRadians(MaxLookDownAngle));

            // Store the initial camera rotation
            camRotation = Entity.Transform.RotationEulerXYZ;

            // Set the mouse to the middle of the screen
            Input.MousePosition = new Vector2(0.5f, 0.5f);

            isActive = true;
            Game.IsMouseVisible = false;

            character = Entity.Get<CharacterComponent>();
        }

        private void ProcessInput()
        {
            var checks = new List<Func<bool>> {
                () => isActive,
                () => Math.Abs(Input.MouseWheelDelta) > 0,
                () => Input.MouseWheelDelta + this._cameraOffset.Z <= 0,
                () => Input.MouseWheelDelta + this._cameraOffset.Z > this.MaxCameraDistance * -1
            };

            if (checks.All(x => x()))
            {
                this._cameraOffset += new Vector3(0, 0, Input.MouseWheelDelta);
            }
        }

        public override void Update()
        {
            if (Input.IsKeyPressed(Keys.Escape))
            {
                isActive = !isActive;
                Game.IsMouseVisible = !isActive;
                Input.UnlockMousePosition();
            }

            ProcessInput();
           
            if (isActive)
            {
                Input.LockMousePosition();
                var mouseMovement = Input.MouseDelta * MouseSpeed;

                // Update camera rotation values
                camRotation.Y += -mouseMovement.X;
                camRotation.X += InvertMouseY ? -mouseMovement.Y : mouseMovement.Y;
                camRotation.X = MathUtil.Clamp(camRotation.X, maxCameraAnglesRadians.X, maxCameraAnglesRadians.Y);

                // Apply Y rotation to character entity
                if (Input.IsMouseButtonDown(MouseButton.Right))
                {
                    character.Orientation = Quaternion.RotationY(camRotation.Y);
                }
                else 
                {
                    firstPersonCameraPivot.Transform.Rotation = Quaternion.RotationY(camRotation.Y);
                }
                // Entity.Transform.Rotation = Quaternion.RotationY(camRotation.Y);

                // Apply X camera rotation to the existing camera rotations
                firstPersonCameraPivot.Transform.Rotation = Quaternion.RotationX(camRotation.X);
                thirdPersonCameraPivot.Transform.Position = new Vector3(0);
                thirdPersonCameraPivot.Transform.Position += this._cameraOffset;
            }
        }
    }
}

