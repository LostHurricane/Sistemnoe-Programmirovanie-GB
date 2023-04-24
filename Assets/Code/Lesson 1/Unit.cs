using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private int _health;
    private Coroutine _coroutine;

    // Start is called before the first frame update
    void Start()
    {
        _health = 0;
        RecieveHealing();
    }

    private void Update()
    {
        //RecieveHealing();
    }

    private void RecieveHealing ()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(HealingProcess ());
        }
        else
        {
            Debug.Log("cant start healing");
        }
    }

    private IEnumerator HealingProcess ()
    {
        float timeStep = 0.5f;
        float maxHealingTime = 3f;

        float time = 0;
        while (time <= maxHealingTime)
        {
            if (_health > 100)
            {
                _health = 100;
                break;
            }
            Debug.Log($"hp = {_health}, time = {time}");

            _health += 5;
            yield return new WaitForSeconds(timeStep);
            time += timeStep;
        }
        Cleanup();
    }

    private void Cleanup ()
    {
        _coroutine = null;
    }

}

