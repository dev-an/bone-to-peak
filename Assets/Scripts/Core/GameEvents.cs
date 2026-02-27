using System;

namespace BoneToPeak.Core
{
    public static class GameEvents
    {
        public static event Action<EnemyDeathEventArgs> OnEnemyDeath;
        public static event Action<float> OnPlayerHealthChanged;
        public static event Action OnPlayerDeath;
        public static event Action<int> OnCorpseCountChanged;
        public static event Action<int, int> OnMinionCountChanged;
        public static event Action<int> OnWaveStarted;
        public static event Action<int> OnWaveEnded;

        public static void RaiseEnemyDeath(EnemyDeathEventArgs args)
        {
            OnEnemyDeath?.Invoke(args);
        }

        public static void RaisePlayerHealthChanged(float healthRatio)
        {
            OnPlayerHealthChanged?.Invoke(healthRatio);
        }

        public static void RaisePlayerDeath()
        {
            OnPlayerDeath?.Invoke();
        }

        public static void RaiseCorpseCountChanged(int count)
        {
            OnCorpseCountChanged?.Invoke(count);
        }

        public static void RaiseMinionCountChanged(int current, int max)
        {
            OnMinionCountChanged?.Invoke(current, max);
        }

        public static void RaiseWaveStarted(int waveNumber)
        {
            OnWaveStarted?.Invoke(waveNumber);
        }

        public static void RaiseWaveEnded(int waveNumber)
        {
            OnWaveEnded?.Invoke(waveNumber);
        }
    }
}
