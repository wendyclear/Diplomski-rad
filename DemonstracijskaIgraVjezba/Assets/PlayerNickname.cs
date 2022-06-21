using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerNickname : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInput;
    private string _defaultNickname;

    private void Awake()
    {
        /*
         * Potrebno je zadati neki "defaultni" nadimak trenutnom igracu - 
         * Na primjer = "Player" + nasumican broj u nekom intervalu.
         */
    }
    public void ChangeUsername()
    {
        /*
         * U MenuManageru nalazi se objekt TitleMenu, a u njemu InputFieldUsername. 
         * Provjerite koja metoda tog objekta poziva upravo ovu te promijenite 
         * nadimak igracu koristeci referencu _usernameInput
         */
    }
}
