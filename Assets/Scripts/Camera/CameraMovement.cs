using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/**
 * a class that handles camera movement events
 */
public static class CameraMovement
{
    // parameter: distance camera moved
    public static UnityEvent<Vector3> cameraMoveCallback = new();

    /**
     * moves the camera to the target position over the given duration
     * invoked as StartCoroutine(CameraMovement.smoothCameraMove(*args))
     * 
     * @param targetPosition the target position to move the camera to
     * @param duration the duration over which to move the camera
     * @param doneCallback a callback to invoke when the movement is done
     */
    public static IEnumerator smoothCameraMove(Vector2 targetPosition, float duration=0, Action doneCallback=null)
    {
        // run until duration is reached
        Vector3 startPos = Camera.main.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float step = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            // calculate the current position
            Vector3 position = new Vector3(
                Mathf.Lerp(startPos.x, targetPosition.x, step),
                Mathf.Lerp(startPos.y, targetPosition.y, step),
                startPos.z
            );

            // update position and wait for next frame
            cameraMoveCallback.Invoke(position - Camera.main.transform.position);
            Camera.main.transform.position = position;
            yield return null;
        }

        // Final position
        Camera.main.transform.position = new Vector3(targetPosition.x, targetPosition.y, startPos.z);
        doneCallback?.Invoke();
    }

    /**
     * shakes the camera with the given magnitude and duration
     * invoked as StartCoroutine(CameraMovement.shakeCamera(*args))
     * 
     * @param magnitude the magnitude of the shake
     * @param duration the duration of the shake
     * @param doneCallback a callback to invoke when the movement is done
     */
    public static IEnumerator shakeCamera(float magnitude, float duration, Action doneCallback=null)
    {
        // randomly offset camera position over duration 
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 delta = UnityEngine.Random.insideUnitCircle * magnitude;
            Camera.main.transform.localPosition = originalPos + delta;
            yield return null;
        }

        // reset position
        Camera.main.transform.localPosition = originalPos;
        doneCallback?.Invoke();
    }
}
