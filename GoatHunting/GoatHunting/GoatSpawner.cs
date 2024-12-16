using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace GoatHunting
{
    public class GoatSpawner
    {
        private Timer _spawnTimer;
        private Form _parentForm;
        private List<Goat> _goats;
        private Random _rand;

        public event Action<Goat> OnGoatSpawned;

        public GoatSpawner(Form parentForm, List<Goat> goats)
        {
            _parentForm = parentForm;
            _goats = goats;
            _rand = new Random();

            // Setup spawn timer
            _spawnTimer = new Timer
            {
                Interval = 3000 // Spawn interval (3 seconds)
            };
            _spawnTimer.Tick += SpawnGoats;
            _spawnTimer.Start();
        }

        private void SpawnGoats(object sender, EventArgs e)
        {
            // Spawn 3 goats every 3 seconds
            for (int i = 0; i < 3; i++)
            {
                Point newPosition = new Point(
                    _rand.Next(0, _parentForm.ClientSize.Width - 60), // Width of goat
                    _rand.Next(0, _parentForm.ClientSize.Height - 75) // Height of goat
                );

                Goat newGoat = new Goat(newPosition, _parentForm);
                _goats.Add(newGoat);
                _parentForm.Controls.Add(newGoat.GetPictureBox());

                OnGoatSpawned?.Invoke(newGoat);
            }
        }

        // Dispose the spawner when no longer needed
        public void Dispose()
        {
            _spawnTimer?.Stop();
            _spawnTimer?.Dispose();
        }
    }
}
