using UnityEngine;

namespace CrowdControlSampleGame
{
    public class Bomb : MonoBehaviour
    {
        private bool m_exploded = false;
        private MeshRenderer m_meshRenderer;

        private void Awake()
        {
            m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_exploded || !other.CompareTag("Player"))
            {
                return;
            }

            Player.Instance.Damage();
            m_exploded = true;
            m_meshRenderer.enabled = false;
        }

        public void Respawn()
        {
            m_exploded = false;
            m_meshRenderer.enabled = true;
        }
    }
}
