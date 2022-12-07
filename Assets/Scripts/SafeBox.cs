using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SafeBox : InteractiveObject, IPunObservable
{
    public Text[] guessResultTexts;
    int textIndex = 0;

    private int[] fourNumbers;

    private InputField inputField;

    public void Start()
    {
        inputField = ui.gameObject.GetComponentInChildren<InputField>();


        inputField.onEndEdit.AddListener(delegate { LockInput(inputField); });

        //Use onSubmit
        inputField.onSubmit.AddListener(delegate { ShootNumberToServer(inputField); });
    }

    private void LockInput(InputField input)
    {
        if (input.text.Length > 0)
        {
            Debug.Log("Text has been entered");
        }
        else if (input.text.Length == 0)
        {
            Debug.Log("Main Input Empty");
        }
    }

    private void ShootNumberToServer(InputField input)
    {
        string inputString = input.text;
        photonView.RPC("GuessNumber", RpcTarget.All, inputString);
    }

    [PunRPC]
    private void GuessNumber(string inputString)
    {
        BullsAndCows bullsAndCows = new BullsAndCows();
        int[] guessResult = bullsAndCows.GuessNumber(inputString, fourNumbers);
        if (guessResult[0] != -1)
        {
            string guessResultString = MakeGuessResultString(inputString, guessResult);
            guessResultTexts[textIndex].text = guessResultString;
            textIndex++;
        }
    }

    private string MakeGuessResultString(string inputString, int[] strikeBallResult)
    {
        string guessResult;

        guessResult = inputString +
        " strike : " + strikeBallResult[0] +
        " ball : " + strikeBallResult[1];

        return guessResult;
    }

    public override void Interact(GameObject other)
    {

    }


    public void SetFourNumbers(int[] fourNumbers)
    {
        for (int i = 0; i < fourNumbers.Length; i++)
        {
            this.fourNumbers[i] = fourNumbers[i];
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(guessResultTexts);
            stream.SendNext(textIndex);
            stream.SendNext(fourNumbers);
        }
        else
        {
            guessResultTexts = (Text[])stream.ReceiveNext();
            textIndex = (int)stream.ReceiveNext();
            fourNumbers = (int[])stream.ReceiveNext();
        }
    }
}