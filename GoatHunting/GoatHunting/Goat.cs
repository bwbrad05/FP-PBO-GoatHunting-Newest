using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace GoatHunting
{
    public class Goat
    {
        private const int GoatWidth = 60;
        private const int GoatHeight = 75;
        private const int MovementSpeed = 5; // Speed at which the goat moves
        private const int DamageAmount = 10; // Amount of damage the goat deals
        private const int SpawnInterval = 3000;
        private const int CollisionCooldown = 1000; // 1 second cooldown for collision

        public event Action<int> OnPlayerDamaged; // Event to notify player damage
        public event Action<Goat> OnGoatSpawned; // Event to notify when a new goat is spawned

        private PictureBox _goatPictureBox;
        private Point _targetPosition; // Player's position to be attracted to
        private bool _isActive = true; // Indicates if the goat is active
        public bool IsActive { get { return _isActive; } set { _isActive = value; } }
        private bool _collisionCooldownActive = false; // Collision cooldown flag
        //private Timer _spawnTimer;
        private Timer _cooldownTimer; // Timer to handle collision cooldown
        private Form _parentForm;

        public event Action<Goat> OnGoatKilled;

        public Goat(Point initialPosition, Form parentForm = null)
        {
            _parentForm = parentForm;

            _goatPictureBox = new PictureBox
            {
                Size = new Size(GoatWidth, GoatHeight),
                Location = initialPosition,
                //BackColor = Color.YellowGreen,
                Tag = "Goat",
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            byte[] goatPic = Resource.goatnobg;

            try
            {
                // Convert byte array to Image
                using (MemoryStream ms = new MemoryStream(goatPic))
                {
                    _goatPictureBox.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                _goatPictureBox.BackColor = Color.Black;
                Console.WriteLine("Failed to load goat image: " + ex.Message);
            }


            // Setup spawn timer if parent form is provided
            //if (parentForm != null)
            //{
            //    _spawnTimer = new Timer
            //    {
            //        Interval = SpawnInterval
            //    };
            //    _spawnTimer.Tick += SpawnNewGoat;
            //    _spawnTimer.Start();
            //}

            // Setup cooldown timer
            _cooldownTimer = new Timer
            {
                Interval = CollisionCooldown
            };
            _cooldownTimer.Tick += (sender, e) =>
            {
                _collisionCooldownActive = false;
                _cooldownTimer.Stop();
            };


        }

        //private void SpawnNewGoat(object sender, EventArgs e)
        //{
        //    if (!_isActive || _parentForm == null)
        //    {
        //        return;
        //    }

        //    // Generate a random position for the new goat
        //    Random rand = new Random();
        //    Point newPosition = new Point(
        //        rand.Next(0, _parentForm.ClientSize.Width - GoatWidth),
        //        rand.Next(0, _parentForm.ClientSize.Height - GoatHeight)
        //    );

        //    Goat newGoat = new Goat(newPosition, _parentForm);
        //    OnGoatSpawned?.Invoke(newGoat);
        //}

        public PictureBox GetPictureBox() => _goatPictureBox;

        public void UpdatePosition(Point playerPosition)
        {
            if (!_isActive)
                return;

            // Calculate direction towards the player
            int directionX = playerPosition.X - _goatPictureBox.Left;
            int directionY = playerPosition.Y - _goatPictureBox.Top;

            // Normalize direction vector to unit vector
            double magnitude = Math.Sqrt(directionX * directionX + directionY * directionY);
            if (magnitude > 0)
            {
                double normalizedX = directionX / magnitude;
                double normalizedY = directionY / magnitude;

                // Offset to stop the goat near the player, avoiding overlap
                int stopDistance = 60; // Distance from the player where the goat should stop
                if (magnitude > stopDistance)
                {
                    // Move goat towards the player but stop at the defined offset
                    int moveX = (int)(normalizedX * MovementSpeed);
                    int moveY = (int)(normalizedY * MovementSpeed);

                    _goatPictureBox.Left += moveX;
                    _goatPictureBox.Top += moveY;
                }
            }
        }

        public void Kill()
        {
            if (!_isActive) return; // Prevent multiple calls to Kill

            _isActive = false;

            // Notify that the goat is killed
            OnGoatKilled?.Invoke(this); // This should not lead to a recursive call

            // Dispose of the PictureBox
            if (_goatPictureBox != null)
            {
                var parent = _goatPictureBox.Parent;
                if (parent is GameForm form)
                {
                    form.Goats.Remove(this); // Remove this goat instance from the list
                }
                _goatPictureBox.Dispose();
                _goatPictureBox = null; // Prevent further access
            }

            _cooldownTimer?.Dispose();

        }


        public void CheckCollision(Player player)
        {
            // Only check collision if the goat is active and not in cooldown
            if (_isActive == false || _collisionCooldownActive)
                return;

            // If the goat intersects with the player and is active
            if (_goatPictureBox.Bounds.IntersectsWith(player.GetPictureBox().Bounds))
            {
                // Notify player damage
                OnPlayerDamaged?.Invoke(DamageAmount);

                // Start collision cooldown
                _collisionCooldownActive = true;
                _cooldownTimer.Start();
            }
        }

        //public void Reactivate(Point newPosition)
        //{
        //    _isActive = true;
        //    _goatPictureBox.Location = newPosition;
        //    _spawnTimer?.Start();
        //}

        // Cleanup method to stop the timers
        public void Dispose()
        {
            //_spawnTimer?.Stop();
            //_spawnTimer?.Dispose();
            _cooldownTimer?.Stop();
            _cooldownTimer?.Dispose();
        }
    }
}
