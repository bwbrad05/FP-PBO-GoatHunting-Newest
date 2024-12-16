using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace GoatHunting
{
    public class Bullet
    {
        private const int BulletWidth = 20;
        private const int BulletHeight = 10;
        private const int Speed = 24;
        private Timer _movementTimer;
        private PictureBox _bulletPictureBox;
        private int _directionX;
        private int _directionY;
        private Form _parentForm;

        public event EventHandler BulletDestroyed;

        public Bullet(Point startPosition, int directionX, int directionY, Form parentForm)
        {
            _parentForm = parentForm;
            _bulletPictureBox = new PictureBox
            {
                Size = new Size(BulletWidth, BulletHeight),
                Location = startPosition,
                BackColor = Color.Black // You can change this to an image if you have a sprite for the bullet
            };

            _directionX = directionX;
            _directionY = directionY;

            _movementTimer = new Timer { Interval = 30 }; // Adjust interval for smoothness
            _movementTimer.Tick += Move;
            _movementTimer.Start();

            // Add the bullet to the form
            _parentForm.Controls.Add(_bulletPictureBox);
        }

        public PictureBox GetPictureBox() => _bulletPictureBox;

        private void Move(object sender, EventArgs e)
        {
            _bulletPictureBox.Left += _directionX * Speed;
            _bulletPictureBox.Top += _directionY * Speed;

            // Check for collision with goats
            CheckGoatCollision();

            // Remove the bullet when it goes off-screen
            if (_bulletPictureBox.Right < 0 || _bulletPictureBox.Left > _parentForm.ClientSize.Width ||
                _bulletPictureBox.Bottom < 0 || _bulletPictureBox.Top > _parentForm.ClientSize.Height)
            {
                DestroyBullet();
            }
        }

        private void CheckGoatCollision()
        {
            // Find the parent GameForm
            GameForm gameForm = _parentForm as GameForm;
            if (gameForm == null) return;

            // Create a copy of the goats list to safely iterate
            var goatsCopy = new List<Goat>(gameForm.Goats);

            foreach (var goat in goatsCopy)
            {
                PictureBox goatPictureBox = goat.GetPictureBox();

                if (goatPictureBox != null && _bulletPictureBox.Bounds.IntersectsWith(goatPictureBox.Bounds))
                {
                    // Trigger the goat's kill method through the game form's handler
                    gameForm.HandleGoatDestroyed(goat);

                    // Destroy the bullet
                    DestroyBullet();
                    break;
                }
            }
        }

        private void DestroyBullet()
        {
            _movementTimer.Stop();
            _movementTimer.Dispose();

            // Remove the bullet from the form
            _parentForm.Controls.Remove(_bulletPictureBox);
            _bulletPictureBox.Dispose();

            // Trigger the destroyed event
            BulletDestroyed?.Invoke(this, EventArgs.Empty);
        }
    }

}