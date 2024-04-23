using UnityEngine;
using UnityEngine.UI;

namespace CrowdControlSampleGame {
    public class GameCanvas : MonoBehaviour {
#pragma warning disable 0649
        [SerializeField] private Slider m_healthBar;
        [SerializeField] private Text m_coinLabel;
        [SerializeField] private GameObject m_deathScreen;
        [SerializeField] private Button m_resetButton;
#pragma warning restore 0649

        public static GameCanvas Instance { get; private set; }

        void Awake() {
            Instance = this;
            m_resetButton.onClick.AddListener(ResetGame);
        }

        public void SetCoins(int totalCoins) {
            m_coinLabel.text = string.Format("Coins: {0}", totalCoins);
        }

        public void UpdateHealthBar(int healthPoints) {
            m_healthBar.value = healthPoints;

            if (healthPoints == 0)
                m_deathScreen.SetActive(true);
        }

        private void ResetGame() {
            foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Coin"))
                coin.GetComponent<Coin>().Respawn();

            foreach (GameObject bomb in GameObject.FindGameObjectsWithTag("Bomb"))
                bomb.GetComponent<Bomb>().Respawn();

            m_deathScreen.SetActive(false);
            Player.Instance.Respawn();
        }
    }
}