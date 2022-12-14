using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SafeBox : InteractiveObject, IPunObservable
{
    public GameController gameController;

    public int[] fourNumbers = new int[4] { -1, -1, -1, -1 };

    public int[] guessNumbers = new int[40];
    public int[] guessResults = new int[20];
    public int textIndex = 0;

    private bool win = false;

    protected GameObject inputUI;
    private InputField inputField;

    private PlayerController usingPlayer = null;

    public override void TurnOnUI()
    {
        base.TurnOnUI();

        inputField = ui.transform.GetChild(0).gameObject.GetComponent<InputField>();
        // ui.transform.GetChild(1).gameObject.GetComponent<Text>().text = fourNumbers[0].ToString() + fourNumbers[1].ToString()
        //  + fourNumbers[2].ToString() + fourNumbers[3].ToString(); ;
        inputField.ActivateInputField();

        //Use onSubmit
        inputField.onSubmit.AddListener(delegate { ShootNumberToServer(inputField); });

    }

    private void ShootNumberToServer(InputField input)
    {

        if (input.text.Length > 0)
        {
            Debug.Log(input.text);
        }
        string inputString = input.text;
        if (usingPlayer.UseBall())
        {
            photonView.RPC("GuessNumber", RpcTarget.All, inputString);
        }
    }

    [PunRPC]
    public void GuessNumber(string inputString)
    {
        BullsAndCows bullsAndCows = new BullsAndCows();
        int[] guessResult = bullsAndCows.GuessNumber(inputString, fourNumbers);
        if (guessResult[0] != -1)
        {
            for (int i = 0; i < 4; i++)
            {
                char[] guessed = inputString.ToCharArray();
                int curguess = (int)char.GetNumericValue(guessed[i]);
                guessNumbers[textIndex * 4 + i] = curguess;
            }

            for (int i = 0; i < 2; i++)
            {
                guessResults[textIndex * 2 + i] = guessResult[i];
            }

            textIndex++;

            gameController.UpdateGuessBoard();
        }
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
            stream.SendNext(fourNumbers);
            stream.SendNext(textIndex);
            stream.SendNext(guessNumbers);
            stream.SendNext(guessResults);
        }
        else
        {
            fourNumbers = (int[])stream.ReceiveNext();
            textIndex = (int)stream.ReceiveNext();
            guessNumbers = (int[])stream.ReceiveNext();
            guessResults = (int[])stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (usingPlayer == null)
        {
            usingPlayer = other.gameObject.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        usingPlayer = null;
    }
}