using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace GoatHunting
{
    public class GameForm : Form
    {
        private const int PlayerInitialPositionX = 50;
        private const int PlayerInitialPositionY = 50;
        private const int AnimationInterval = 16;
        private const int MaxHealth = 100; // Max health of the player


        private Player _player;
        private List<Goat> _goats = new List<Goat>();
        private GoatSpawner _goatSpawner;
        public List<Goat> Goats => _goats;
        private System.Windows.Forms.Timer _animationTimer;

        private bool goLeft, goRight, goUp, goDown;
        private bool gameOver;

        // Declare ProgressBar for health
        private ProgressBar _healthBar;
        private Label _gameOverLabel;
        private Button _restartButton;

        //buat kill count
        private Label _killCountLabel;
        private int _killCount = 0;

        private Button _backButton;

        private static int _highestKillCount = 0;
        public static int HighestKillCount => _highestKillCount;

        public GameForm()
        {
            InitializeComponent();
            InitializeLevel();
            InitializeKillCountLabel();
            InitializeBackground();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new Size(800, 600);
            this.Name = "GameForm";
            this.Text = "Goat Hunting";
            this.BackColor = Color.LightGray;
            this.KeyPreview = true;
            this.ResumeLayout(false);
        }

        private void InitializeBackground()
        {
            // Set the background color of the form to green
            this.BackColor = Color.Green;

            // Ensure player and goats are in front of the background
            _player.GetPictureBox().BringToFront();
            foreach (var goat in _goats)
            {
                goat.GetPictureBox().BringToFront();
            }
        }

        private void InitializeLevel()
        {
            // Clear existing controls
            this.Controls.Clear();

            gameOver = false;
            this.Text = "Level Newbie";

            // Initialize player
            _player = new Player(new Point(PlayerInitialPositionX, PlayerInitialPositionY));
            this.Controls.Add(_player.GetPictureBox());

            // Add bullet to form
            _player.OnBulletFired += AddBulletToForm;

            // Add health to form
            _player.OnPlayerHealthChanged += CheckPlayerHealth;

            // Initialize goats
            InitializeGoats();

            _goatSpawner = new GoatSpawner(this, _goats);
            _goatSpawner.OnGoatSpawned += AddGoatToForm;

            // Initialize health bar
            InitializeHealthBar();

            // Timer
            _animationTimer = new System.Windows.Forms.Timer { Interval = AnimationInterval };
            _animationTimer.Tick += (sender, e) => UpdateGame();
            _animationTimer.Start();

            // Movement and shooting key bindings
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
        }

        private void InitializeHealthBar()
        {
            _healthBar = new ProgressBar
            {
                Location = new Point(10, 10), // Position the bar at the top left
                Size = new Size(200, 20), // Size of the bar
                Maximum = MaxHealth, // Max health
                Value = MaxHealth, // Start at max health
                ForeColor = Color.Green, // Color of the health bar
                BackColor = Color.Red, // Background color of the health bar
                Style = ProgressBarStyle.Continuous // Smooth progress
            };

            this.Controls.Add(_healthBar);
        }

        private void InitializeKillCountLabel()
        {
            _killCountLabel = new Label
            {
                Text = "Goats Killed: 0",
                Location = new Point(this.ClientSize.Width - 150, 10), // Position in upper right
                AutoSize = true,
                Font = new Font("Arial", 12)
            };
            this.Controls.Add(_killCountLabel);
        }

        private void InitializeGoats()
        {
            _goats.Clear(); // Clear existing goats

            var initialGoatPositions = new List<Point>
            {
                new Point(200, 150),
                new Point(400, 300),
                new Point(600, 450)
            };

            foreach (var position in initialGoatPositions)
            {
                var goat = new Goat(position, this);
                goat.OnGoatSpawned += AddGoatToForm;
                goat.OnPlayerDamaged += damage => _player.TakeDamage(damage);
                //goat.OnGoatKilled += HandleGoatDestroyed;
                _goats.Add(goat);
                this.Controls.Add(goat.GetPictureBox());
            }
        }

        private void AddGoatToForm(Goat goat)
        {
            if (gameOver)
                return;

            _goats.Add(goat);
            goat.OnPlayerDamaged += damage => _player.TakeDamage(damage);
            this.Controls.Add(goat.GetPictureBox());
        }

        private void UpdateGame()
        {
            if (gameOver)
                return;

            if (goLeft) _player.Walk(Keys.Left, this.ClientSize);
            if (goRight) _player.Walk(Keys.Right, this.ClientSize);
            if (goUp) _player.Walk(Keys.Up, this.ClientSize);
            if (goDown) _player.Walk(Keys.Down, this.ClientSize);

            _player.Animate();

            // Create a copy of the list to safely iterate
            var goatsCopy = new List<Goat>(_goats);
            foreach (var goat in goatsCopy)
            {
                goat.UpdatePosition(_player.GetPictureBox().Location);
                goat.CheckCollision(_player);
            }
        }

        private void CheckPlayerHealth(int currentHealth)
        {
            // Update the health bar when player's health changes
            _healthBar.Value = currentHealth;

            if (currentHealth <= 0)
            {
                TriggerGameOver();
            }
        }

        private void TriggerGameOver()
        {
            gameOver = true;
            _animationTimer.Stop();
            _goatSpawner.Dispose();

            //if (_killCount > _highestKillCount)
            //{
            //    _highestKillCount = _killCount;
            //}

            int currentHighScore = HighScoreManager.LoadHighScore();

            if (_killCount > currentHighScore)
            {
                HighScoreManager.SaveHighScore(_killCount);
            }

            // Clear existing controls
            this.Controls.Clear();

            // Create game over label
            _gameOverLabel = new Label
            {
                Text = "GAME OVER",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(
                    (this.ClientSize.Width - TextRenderer.MeasureText("GAME OVER", new Font("Arial", 36, FontStyle.Bold)).Width) / 2,
                    this.ClientSize.Height / 2 - 150
                )
            };
            this.Controls.Add(_gameOverLabel);

            // Create kill count label
            var killCountLabel = new Label
            {
                Text = $"Total Goats Killed: {_killCount}",
                Font = new Font("Arial", 20, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(
                    (this.ClientSize.Width - TextRenderer.MeasureText($"Total Goats Killed: {_killCount}", new Font("Arial", 20, FontStyle.Regular)).Width) / 2,
                    this.ClientSize.Height / 2
                )
            };
            this.Controls.Add(killCountLabel);

            // Create restart button
            _restartButton = new Button
            {
                Text = "Restart Game",
                Font = new Font("Arial", 16),
                Size = new Size(200, 50),
                Location = new Point(
                    (this.ClientSize.Width - 200) / 2,
                    this.ClientSize.Height / 2 + 100
                )
            };
            _restartButton.Click += RestartGame;
            this.Controls.Add(_restartButton);

            // Create exit button
            _backButton = new Button
            {
                Text = "Exit",
                Font = new Font("Arial", 16),
                Size = new Size(120, 40),
                Location = new Point((this.ClientSize.Width - 120) / 2, this.ClientSize.Height / 2 + 160)
            };
            _backButton.Click += BackButton_Click;
            this.Controls.Add(_backButton);
        }


        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RestartGame(object sender = null, EventArgs e = null)
        {
            // Clear all existing controls
            this.Controls.Clear();

            // Dispose of existing game objects
            if (_player != null)
            {
                _player.GetPictureBox().Dispose();
                _player = null;
            }

            // Dispose of all goats
            foreach (var goat in _goats)
            {
                goat.Dispose();
                goat.GetPictureBox().Dispose();
            }
            _goats.Clear();

            _goatSpawner.Dispose();

            // Reset game state
            gameOver = false;
            goLeft = goRight = goUp = goDown = false;

            // Reinitialize the level
            InitializeLevel();
            InitializeKillCountLabel();

            _killCount = 0;
            if (_killCountLabel != null)
            {
                _killCountLabel.Text = "Goats Killed: 0";
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (gameOver && e.KeyCode == Keys.Enter)
            {
                RestartGame();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Left:
                    goLeft = true;
                    break;
                case Keys.Right:
                    goRight = true;
                    break;
                case Keys.Up:
                    goUp = true;
                    break;
                case Keys.Down:
                    goDown = true;
                    break;
                case Keys.Space:
                    _player.FireBulletBasedOnDirection();
                    break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    goLeft = false;
                    break;
                case Keys.Right:
                    goRight = false;
                    break;
                case Keys.Up:
                    goUp = false;
                    break;
                case Keys.Down:
                    goDown = false;
                    break;
            }
        }

        private void AddBulletToForm(Bullet bullet)
        {
            this.Controls.Add(bullet.GetPictureBox());
        }

        private void IncrementKillCount()
        {
            _killCount++;
            _killCountLabel.Text = $"Goats Killed: {_killCount}";
        }

        public void HandleGoatDestroyed(Goat goat)
        {
            // Ensure the goat is not already inactive
            if (goat.IsActive)
            {
                // Call the Kill method to handle the goat's destruction
                goat.Kill();
            }

            // Remove the goat from the list
            _goats.Remove(goat);

            // Check if the PictureBox is not null before disposing
            if (goat.GetPictureBox() != null)
            {
                this.Controls.Remove(goat.GetPictureBox());
                goat.GetPictureBox().Dispose();
            }

            // Increment the kill count
            IncrementKillCount();
        }

    }
}