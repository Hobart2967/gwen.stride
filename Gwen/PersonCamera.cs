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
        public float MinimalHitDistance = 1.0f;

        private Entity firstPersonCameraPivot;
        private Entity thirdPersonCameraPivot;
        private Vector3 camRotation;
        private bool isActive = false;
        private Vector2 maxCameraAnglesRadians;
        private CharacterComponent character;
        private Vector3 _cameraOffset = new Vector3(0, 0, -3);
        private float rotationSpeed = 0.05f;
        private Simulation simulation;

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
            

            character = Entity.Get<CharacterComponent>();
            simulation = this.GetSimulation();
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
            ProcessInput();
                
            if (Input.IsKeyDown(Keys.A))
            {
                camRotation.Y += rotationSpeed;
            } 
            else if(Input.IsKeyDown(Keys.D))
            {
                camRotation.Y -= rotationSpeed;
            }
                
            character.Orientation = Quaternion.RotationY(camRotation.Y);
            
            float x, y, z;
            thirdPersonCameraPivot.Transform.Position.Deconstruct(out x, out y, out z);
            // Apply Y rotation to character entity
            if (Input.IsMouseButtonDown(MouseButton.Right))
            {
                Input.LockMousePosition();

                var mouseMovement = Input.MouseDelta * MouseSpeed;

                camRotation.Y += -mouseMovement.X;

                camRotation.X += InvertMouseY
                    ? -mouseMovement.Y
                    : mouseMovement.Y;

                camRotation.X = MathUtil.Clamp(camRotation.X, maxCameraAnglesRadians.X, maxCameraAnglesRadians.Y);

                Game.IsMouseVisible = false;

                character.Orientation = Quaternion.RotationY(camRotation.Y);
                
                firstPersonCameraPivot.Transform.Rotation = Quaternion.RotationX(camRotation.X);
            }
            else
            {
                Input.UnlockMousePosition();
                Game.IsMouseVisible = true;
                thirdPersonCameraPivot.Transform.Position = new Vector3(x, y, z);
            }

            // Entity.Transform.Rotation = Quaternion.RotationY(camRotation.Y);
            // Apply X camera rotation to the existing camera rotations                

            
            thirdPersonCameraPivot.Transform.Position = new Vector3(x, y, 0);
            thirdPersonCameraPivot.Transform.Position += this._cameraOffset;

            thirdPersonCameraPivot.Transform.UpdateWorldMatrix();

            var raycastStart = firstPersonCameraPivot.Transform.WorldMatrix.TranslationVector;
            var raycastEnd = thirdPersonCameraPivot.Transform.WorldMatrix.TranslationVector;

            

            if (this.simulation.Raycast(raycastStart, raycastEnd, out HitResult result))
            {
                var distance = Vector3.Distance(raycastStart, result.Point);
                if (distance > MinimalHitDistance)
                {
                    thirdPersonCameraPivot.Transform.Position.Z = -(distance - 0.1f);
                }
                else
                {
                    thirdPersonCameraPivot.Transform.Position = new Vector3(this._cameraOffset.X, this._cameraOffset.Y, 0);
                }
            }
        }
    }
}

