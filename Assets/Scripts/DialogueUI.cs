using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
   [SerializeField] private GameObject _dialogueBox;
   [SerializeField] private TMP_Text _textLabel;
   [SerializeField] private DialogueObject testDialogue;
   
   private TypeWriterEffect typeWriterEffect;
   
   private void Start()
   {
      if (_dialogueBox == null || _textLabel == null || testDialogue == null)
      {
         Debug.LogError("DialogueUI is missing one or more references.", this);
         return;
      }

      typeWriterEffect = GetComponent<TypeWriterEffect>();
      if (typeWriterEffect == null)
      {
         Debug.LogError("TypeWriterEffect component not found on GameObject.", this);
         return;
      }
      ShowDialogue(testDialogue);
   }

   public void ShowDialogue(DialogueObject dialogueObject)
   {
      _dialogueBox.SetActive(true); // Ensure the dialogue box is visible
      StartCoroutine(StepThroughDialogue(dialogueObject));
   }

   private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
   {
      foreach (string dialogue in dialogueObject.Dialogue)
      {
         yield return typeWriterEffect.Run(dialogue, _textLabel);
         yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
         Debug.Log("Return key pressed, advancing to next line.");
      }
      CloseDialogueBox();
   }

   private void CloseDialogueBox()
   {
      EnemyControllerSpum.IsDialogueActive = false;
     _dialogueBox.SetActive(false);
     _textLabel.text = string.Empty;
   }
}

