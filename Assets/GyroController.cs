using UnityEngine;

public class GyroController : MonoBehaviour

{   private Gyroscope _gyro;
    private bool _gyroActive; 
    private Quaternion _gyroRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if (SystemInfo.supportsGyroscope)
        {
            _gyro = Input.gyro;
            _gyro.enabled = true;
            _gyroActive = true;
        }
        else
        {
            Debug.LogWarning("Gyro not available");
            _gyroActive = false;
        }
        _gyroRotation = Quaternion.Euler(90f, 0f, 0f);
    }

    void Update()
    {
        if (_gyroActive)
        {
            Quaternion deviceRotation = _gyro.attitude;

            deviceRotation = new Quaternion(
                -deviceRotation.x,
                deviceRotation.y,
                deviceRotation.z,
                deviceRotation.w
            );
            transform.localRotation = _gyroRotation * deviceRotation;
        }
    }
}
