using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypeWriterEffect : MonoBehaviour
{
   [SerializeField] private float typeWriterSpeed = 50f;
  public Coroutine Run(string text,TMP_Text label)
  {
    return StartCoroutine(TypeText(text,label));
  }

  private IEnumerator TypeText(string text,TMP_Text label)
  {
      label.text = string.Empty;
      float t = 0;
      int charIndex = 0;
      while (charIndex < text.Length)
      {
          t+= Time.deltaTime * typeWriterSpeed;
          charIndex = Mathf.FloorToInt(t);
          charIndex = Mathf.Clamp(charIndex,0,text.Length);
          label.text = text.Substring(0,charIndex);
          yield return null;
      }

      label.text = text;
  }
}
