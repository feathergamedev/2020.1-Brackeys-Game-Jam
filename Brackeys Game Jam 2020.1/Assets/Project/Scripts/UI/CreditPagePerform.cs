using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditPagePerform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreditPerform());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CreditPerform()
    {
        yield return new WaitForSeconds(8.0f);
        SceneManager.LoadScene("Main");
    }
}
