using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CreditPagePerform : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_SFX;

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
        yield return new WaitForSeconds(0.55f);
        FadeInSFX();
        yield return new WaitForSeconds(8.0f);
        SceneManager.LoadScene("Main");
    }

    void FadeInSFX()
    {
        m_SFX.Play();
        DOTween.To(() => m_SFX.volume, x => m_SFX.volume = x, 0.55f, 1.0f);
    }
}
