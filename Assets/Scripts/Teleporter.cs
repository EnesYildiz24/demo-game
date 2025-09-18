using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleporter Settings")]
    public Teleporter linkedTeleporter; // Das Ziel-Teleporter
    public bool isActive = true;
    public float teleportCooldown = 1f; // Verhindert sofortiges Zurück-Teleportieren

    [Header("Visual Effects")]
    public ParticleSystem teleportEffect;
    public Color teleportColor = Color.cyan;
    public float effectDuration = 2f;

    [Header("Audio")]
    public AudioClip teleportSound;
    [Range(0f, 1f)] public float teleportVolume = 0.8f;

    private float lastTeleportTime;
    private AudioSource audioSource;
    private Renderer teleporterRenderer;

    void Start()
    {
        // AudioSource hinzufügen falls nicht vorhanden
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Renderer finden
        teleporterRenderer = GetComponent<Renderer>();
        if (teleporterRenderer == null)
        {
            teleporterRenderer = GetComponentInChildren<Renderer>();
        }

        // Teleporter visuell hervorheben
        if (teleporterRenderer != null)
        {
            teleporterRenderer.material.color = teleportColor;
            teleporterRenderer.material.SetFloat("_EmissionColor", teleportColor);
        }

        // Teleport-Effekt vorbereiten
        if (teleportEffect == null)
        {
            // Erstelle einfachen Partikel-Effekt
            CreateTeleportEffect();
        }
    }

    private void CreateTeleportEffect()
    {
        GameObject effectGO = new GameObject("TeleportEffect");
        effectGO.transform.SetParent(transform);
        effectGO.transform.localPosition = Vector3.zero;

        teleportEffect = effectGO.AddComponent<ParticleSystem>();

        // Konfiguriere Partikel-System
        var main = teleportEffect.main;
        main.duration = effectDuration;
        main.loop = false;
        main.startLifetime = 1f;
        main.startSpeed = 2f;
        main.startSize = 0.5f;
        main.startColor = teleportColor;

        var emission = teleportEffect.emission;
        emission.rateOverTime = 50f;

        var shape = teleportEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        // Nur Player teleportieren
        if (other.CompareTag("Player"))
        {
            // Cooldown prüfen
            if (Time.time - lastTeleportTime < teleportCooldown) return;

            // Teleport ausführen
            TeleportPlayer(other.transform);
        }
    }

    private void TeleportPlayer(Transform playerTransform)
    {
        if (linkedTeleporter == null)
        {
            Debug.LogWarning("Teleporter hat kein Ziel-Teleporter!");
            return;
        }

        // Speichere aktuelle Position für Cooldown
        lastTeleportTime = Time.time;

        // Teleport-Position berechnen (etwas über dem Boden)
        Vector3 targetPosition = linkedTeleporter.transform.position + Vector3.up * 1.5f;

        // Player teleportieren
        playerTransform.position = targetPosition;

        // Effekte abspielen
        PlayTeleportEffects();

        // Linked Teleporter Cooldown setzen
        if (linkedTeleporter != null)
        {
            linkedTeleporter.lastTeleportTime = Time.time;
        }

        Debug.Log($"Player teleportiert von {name} zu {linkedTeleporter.name}");
    }

    private void PlayTeleportEffects()
    {
        // Sound abspielen
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound, teleportVolume);
        }

        // Partikel-Effekt abspielen
        if (teleportEffect != null)
        {
            teleportEffect.Play();
        }

        // Linked Teleporter Effekt auch abspielen
        if (linkedTeleporter != null && linkedTeleporter.teleportEffect != null)
        {
            linkedTeleporter.teleportEffect.Play();
        }
    }

    // Gizmos für bessere Übersicht im Editor
    void OnDrawGizmos()
    {
        if (linkedTeleporter != null)
        {
            // Linie zum Ziel-Teleporter zeichnen
            Gizmos.color = teleportColor;
            Gizmos.DrawLine(transform.position, linkedTeleporter.transform.position);

            // Ziel-Teleporter als Sphere zeichnen
            Gizmos.DrawWireSphere(linkedTeleporter.transform.position, 0.5f);
        }

        // Eigenen Bereich als Cube zeichnen
        Gizmos.color = isActive ? teleportColor : Color.gray;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
    }

    // Teleporter aktivieren/deaktivieren
    public void SetActive(bool active)
    {
        isActive = active;
        if (teleporterRenderer != null)
        {
            teleporterRenderer.material.color = active ? teleportColor : Color.gray;
        }
    }
}
