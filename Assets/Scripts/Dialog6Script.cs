using System.Collections;
using UnityEngine;

public class Dialog6Script : DialogTrigger
{
    [SerializeField] private GameObject monster;
    [SerializeField] private GameObject fire;
    [SerializeField] private GameObject safeLight;
    [SerializeField] private GameObject lightsParent;
    public override void EndedDialog()
    {
        monster.SetActive(true);

        fire.SetActive(false);
        safeLight.SetActive(true);

        StartCoroutine(FadeLightsToColor(Color.red, 2f));
    }

    private IEnumerator FadeLightsToColor(Color targetColor, float duration)
    {
        Light[] lights = lightsParent.GetComponentsInChildren<Light>();

        // Salva cores iniciais
        Color[] startColors = new Color[lights.Length];
        for (int i = 0; i < lights.Length; i++)
            startColors[i] = lights[i].color;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].color = Color.Lerp(startColors[i], targetColor, t);
            }

            yield return null;
        }

        // Garante que termine exatamente na cor alvo
        for (int i = 0; i < lights.Length; i++)
            lights[i].color = targetColor;
    }
}
