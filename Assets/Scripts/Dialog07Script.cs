using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dialog07Script : IntegerConditionalCollisionDialogTrigger
{
    [SerializeField] private GameObject finalPanel;
    public override void EndedDialog()
    {
        finalPanel.SetActive(true);
        StartCoroutine(Finish());
    }

    private IEnumerator Finish()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }
}
