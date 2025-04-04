using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SkillSpawner : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] InputActionReference _buttonAction; // Assign in Inspector (e.g., XRI LeftHand PrimaryButton)
    [SerializeField] float _spawnDelay = 0.2f;

    [Header("Skill Settings")]
    [SerializeField] GameObject _skillPrefab; // Your skill VFX/prefab
    [SerializeField] Transform _spawnPoint;    // Where the skill appears (e.g., controller tip)
    [SerializeField] Transform movingPoint;

    private bool _canSpawn = true;
    public bool isProjectile = false;
    public float skillDuration = 5.0f; // Default duration
    public float projectileSpeed = 10.0f;

    void Start()
    {
        _buttonAction.action.performed += OnButtonPressed;
        _buttonAction.action.Enable();
    }

    private void OnButtonPressed(InputAction.CallbackContext context)
    {
        if (_canSpawn && _skillPrefab != null && _spawnPoint != null)
        {
            Debug.Log("Button Pressed! Spawning skill.");

            // Instantiate the skill
            GameObject skill = Instantiate(_skillPrefab, _spawnPoint.position, _spawnPoint.rotation);

            // If it's a projectile, attach movement logic
            if (isProjectile)
            {
                Projectile projectile = skill.AddComponent<Projectile>(); // Add movement script
                projectile.Initialize(_spawnPoint.position, movingPoint.position, projectileSpeed, skillDuration);
            }

            // Cooldown system
            _canSpawn = false;
            Invoke(nameof(ResetSpawn), _spawnDelay);
            Destroy(skill, skillDuration);
        }
    }

    private void ResetSpawn()
    {
        _canSpawn = true;
    }
}
