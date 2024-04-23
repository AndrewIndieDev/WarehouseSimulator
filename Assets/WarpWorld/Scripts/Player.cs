using UnityEngine;
using System.Collections.Generic;
using System;

namespace CrowdControlSampleGame
{
    public class Player : MonoBehaviour
    {
        private Rigidbody m_rigidBody;
        private Renderer m_renderer;
        private int m_coins = 0;
        private int m_health = 5;

        private float m_yVelocity = 0.0f;
        private float m_prevY = 0.0f;

        private bool m_grounded = false;
        private bool m_jumpedPressed = false;
        private bool m_invertedControls = false;

        private Vector3 m_spawnPoint;
        private MeshRenderer m_meshRenderer;
        private Projector m_shadowBlob;

        public static Player Instance { get; private set; }
        public bool Dead { get { return m_health <= 0; } }

        private Dictionary<string, Color> m_playerColors = new Dictionary<string, Color>()
        {
            { "White", Color.white },
            { "Red", Color.red },
            { "Green", Color.green },
            { "Blue", Color.blue }, 
        };

        private void Awake()
        {
            Instance = this;
            m_rigidBody = GetComponent<Rigidbody>();
            m_renderer = GetComponent<Renderer>();
        }

        // Use this for initialization
        void Start()
        {
            GameCanvas.Instance.SetCoins(m_coins);
            GameCanvas.Instance.UpdateHealthBar(m_health);
            m_spawnPoint = transform.position;
            m_meshRenderer = GetComponent<MeshRenderer>();
            m_shadowBlob = GetComponentInChildren<Projector>();
        }

        private void Update()
        {
            if (Dead)
            {
                return;
            }

            if (!m_jumpedPressed)
            {
                m_jumpedPressed = Input.GetButtonDown("Jump");
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Dead)
            {
                return;
            }

            Vector3 velocity = Camera.main.transform.forward * Input.GetAxis("Vertical");
            velocity += Camera.main.transform.right * Input.GetAxis("Horizontal");
            m_yVelocity = Mathf.Max(m_yVelocity - Time.fixedDeltaTime * 2.0f, -1.0f);

            if (m_invertedControls)
            {
                velocity *= -1;
            }

            if (m_jumpedPressed && m_grounded)
            {
                m_yVelocity = 1.0f;
            }

            m_jumpedPressed = false;

            m_grounded = m_yVelocity < 0.01f && m_prevY == transform.position.y;
            m_rigidBody.linearVelocity = new Vector3(velocity.x, m_yVelocity, velocity.z) * 3000.0f * Time.fixedDeltaTime;
            m_prevY = transform.position.y;
        }

        public bool CCJump()
        {
            if (!m_grounded)
            {
                return false;
            }

            m_jumpedPressed = true;
            m_yVelocity = 1.0f;
            return true;
        }

        public bool GiveCoins(uint amount = 1) {
            int proposedCoins = Convert.ToInt32(m_coins + amount);

            if (proposedCoins < 0 || proposedCoins >= 100)
                return false;

            m_coins = proposedCoins;
            GameCanvas.Instance.SetCoins(m_coins);

            return true;
        }
		
		public bool TakeCoins(uint amount = 1) {
            int proposedCoins = Convert.ToInt32(m_coins - amount);

            if (proposedCoins < 0 || proposedCoins >= 100)
                return false;

            m_coins = proposedCoins;
            GameCanvas.Instance.SetCoins(m_coins);

            return true;
        }

        public bool Damage()
        {
            m_health--;
            GameCanvas.Instance.UpdateHealthBar(m_health);

            if (Dead)
            {
                m_meshRenderer.enabled = false;
                m_shadowBlob.enabled = false;
            }

            return true;
        }

        public bool Recover()
        {
            if (m_health >= 5)
            {
                return false;
            }

            m_health++;
            GameCanvas.Instance.UpdateHealthBar(m_health);

            return true;
        }

        public void ChangeColor(string colorName)
        {
            m_renderer.material.SetColor("_Color", m_playerColors[colorName]);
        }

        public bool SetInvertControlState(bool state)
        {
            if (m_invertedControls == state)
            {
                return false;
            }

            m_invertedControls = state;
            return true;
        }

        public void Respawn()
        {
            transform.position = m_spawnPoint;
            m_coins = 0;
            m_health = 5;
            GameCanvas.Instance.SetCoins(m_coins);
            GameCanvas.Instance.UpdateHealthBar(m_health);
            m_meshRenderer.enabled = true;
            m_shadowBlob.enabled = true;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}

