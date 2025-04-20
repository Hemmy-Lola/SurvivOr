using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ScanAndPlayManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject scanningPanel;     // racine du Canvas Scan (Panel ou Canvas)
    public GameObject crosshairCanvas;   // racine du Canvas Crosshair
    public Button btnRestart;
    public Button btnDone;

    [Header("AR")]
    public ARPlaneManager planeManager;
    public ARSession arSession;

    [Header("Gameplay")]
    public EnemySpawner spawner;

    void Start()
    {
        // Bind des boutons
        btnRestart.onClick.AddListener(RestartScanning);
        btnDone.onClick.AddListener(FinishScanning);

        // Lance le scanning au démarrage
        BeginScanning();
    }

    void BeginScanning()
    {
        scanningPanel.SetActive(true);
        crosshairCanvas.SetActive(false);
        spawner.enabled = false;

        arSession.Reset();
        planeManager.enabled = true;
        SetAllPlanesActive(true);
    }

    public void RestartScanning()
    {
        BeginScanning();
    }

    void FinishScanning()
    {
        // On exige au moins 1 plane détecté
        if (planeManager.trackables.count == 0)
        {
            Debug.Log("[Scan] Aucun plane détecté, continuez à scanner.");
            return;
        }

        scanningPanel.SetActive(false);
        crosshairCanvas.SetActive(true);

        planeManager.enabled = false;
        SetAllPlanesActive(false);

        spawner.enabled = true;
    }

    void SetAllPlanesActive(bool active)
    {
        foreach (var p in planeManager.trackables)
            p.gameObject.SetActive(active);
    }
}