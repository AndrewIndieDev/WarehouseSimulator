using UnityEngine;

namespace CrowdControlSampleGame
{
    public class Coin : MonoBehaviour
    {
        private float m_rotationSpeed;
        private MeshRenderer m_meshRenderer;
        private Projector m_shadowBlob;
        private bool m_collected = false;

        private void Awake()
        {
            m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
            m_shadowBlob = gameObject.GetComponentInChildren<Projector>();

            m_rotationSpeed = Random.Range(25.0f, 50.0f);

            if (Random.Range(0.0f, 1.0f) < 0.5f)
            {
                m_rotationSpeed *= -1;
            }
        }

        private void FixedUpdate()
        {
            if (m_collected)
            {
                return;
            }

            transform.Rotate(transform.up * Time.fixedDeltaTime * m_rotationSpeed);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_collected || !other.CompareTag("Player"))
            {
                return;
            }

            Player.Instance.GiveCoins(1);
            m_collected = true;
            m_meshRenderer.enabled = false;
            m_shadowBlob.enabled = false;
        }

        public void Respawn()
        {
            m_collected = false;
            m_meshRenderer.enabled = true;
            m_shadowBlob.enabled = true;
        }
    }
}
