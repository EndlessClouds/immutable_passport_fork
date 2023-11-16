using System.Globalization;
using System.Numerics;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Immutable.Passport;
using Immutable.Passport.Model;
using System.Collections.Generic;

public class AuthenticatedScript : MonoBehaviour
{
#pragma warning disable CS8618
    [SerializeField] private Text output;

    [SerializeField] private Canvas authenticatedCanvas;
    [SerializeField] private Button accessTokenButton;
    [SerializeField] private Button idTokenButton;
    [SerializeField] private Button getAddressButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button showTransferButton;

    [SerializeField] private Canvas transferCanvas;

    [SerializeField] private InputField tokenIdInput1;
    [SerializeField] private InputField tokenAddressInput1;
    [SerializeField] private InputField receiverInput1;

    [SerializeField] private InputField tokenIdInput2;
    [SerializeField] private InputField tokenAddressInput2;
    [SerializeField] private InputField receiverInput2;

    [SerializeField] private InputField tokenIdInput3;
    [SerializeField] private InputField tokenAddressInput3;
    [SerializeField] private InputField receiverInput3;

    [SerializeField] private Button transferButton;
    [SerializeField] private Button cancelTransferButton;

    // ZkEvm
    [SerializeField] private Button connectEvmButton;
    [SerializeField] private Button sendTransactionButton;
    [SerializeField] private Button requestAccountsButton;
    [SerializeField] private Button getBalanceButton;

    // ZkEVM Get Balance Transaction
    [SerializeField] private Canvas zkGetBalanceCanvas;
    [SerializeField] private InputField zkGetBalanceAccount;

    // ZkEVM Send Transaction
    [SerializeField] private Canvas zkSendTransactionCanvas;
    [SerializeField] private InputField zkSendTransactionTo;
    [SerializeField] private InputField zkSendTransactionValue;
    [SerializeField] private InputField zkSendTransactionData;

    private Passport passport;
#pragma warning restore CS8618

    void Start()
    {
        if (Passport.Instance != null)
        {
            passport = Passport.Instance;
        }
        else
        {
            ShowOutput("Passport Instance is null");
        }
    }

    public async void GetAddress()
    {
        ShowOutput($"Called GetAddress()...");
        try
        {
            string? address = await passport.GetAddress();
            ShowOutput(address ?? "No address");
        }
        catch (PassportException e)
        {
            ShowOutput($"Unable to get address: {e.Type}");
        }
        catch (Exception)
        {
            ShowOutput("Unable to get address");
        }
    }

    public async void Logout()
    {
        await passport.Logout();
        SceneManager.LoadScene(sceneName: "UnauthenticatedScene");
    }

    public async void GetAccessToken()
    {
        string? accessToken = await passport.GetAccessToken();
        ShowOutput(accessToken ?? "No access token");
    }

    public async void GetIdToken()
    {
        string? idToken = await passport.GetIdToken();
        ShowOutput(idToken ?? "No ID token");
    }

    public async void GetEmail()
    {
        string? email = await passport.GetEmail();
        ShowOutput(email ?? "No email");
    }

    public async void ShowTransfer()
    {
        authenticatedCanvas.gameObject.SetActive(false);
        transferCanvas.gameObject.SetActive(true);
    }

    public async void CancelTransfer()
    {
        authenticatedCanvas.gameObject.SetActive(true);
        transferCanvas.gameObject.SetActive(false);
        clearInputs();
    }

    public async void Transfer()
    {
        if (tokenIdInput1.text != "" && tokenAddressInput1.text != "" && receiverInput1.text != "")
        {
            ShowOutput("Transferring...");
            transferButton.gameObject.SetActive(false);
            cancelTransferButton.gameObject.SetActive(false);

            try
            {
                List<NftTransferDetails> details = getTransferDetails();

                if (details.Count > 1)
                {
                    CreateBatchTransferResponse response = await passport.ImxBatchNftTransfer(details.ToArray());
                    ShowOutput($"Transferred {response.transfer_ids.Length} items successfully");
                }
                else
                {
                    UnsignedTransferRequest request = UnsignedTransferRequest.ERC721(
                        details[0].receiver,
                        details[0].tokenId,
                        details[0].tokenAddress
                    );
                    CreateTransferResponseV1 response = await passport.ImxTransfer(request);
                    ShowOutput($"Transferred successfully. Transfer id: {response.transfer_id}");
                }

                clearInputs();
            }
            catch (Exception e)
            {
                ShowOutput($"Unable to transfer: {e.Message}");
            }

            transferButton.gameObject.SetActive(true);
            cancelTransferButton.gameObject.SetActive(true);
        }
    }

    private List<NftTransferDetails> getTransferDetails()
    {
        List<NftTransferDetails> details = new List<NftTransferDetails>();

        details.Add(
            new NftTransferDetails(
                receiverInput1.text,
                tokenIdInput1.text,
                tokenAddressInput1.text
            )
        );

        if (tokenIdInput2.text != "" && tokenAddressInput2.text != "" && receiverInput2.text != "")
        {
            details.Add(
                new NftTransferDetails(
                    receiverInput2.text,
                    tokenIdInput2.text,
                    tokenAddressInput2.text
                )
            );
        }

        if (tokenIdInput3.text != "" && tokenAddressInput3.text != "" && receiverInput3.text != "")
        {
            details.Add(
                new NftTransferDetails(
                    receiverInput3.text,
                    tokenIdInput3.text,
                    tokenAddressInput3.text
                )
            );
        }

        return details;
    }

    // ZKEvm
    public async void ConnectEvm()
    {
        try
        {
            await passport.ConnectEvm();
            ShowOutput("Connected to EVM");
            connectEvmButton.gameObject.SetActive(false);
            sendTransactionButton.gameObject.SetActive(true);
            requestAccountsButton.gameObject.SetActive(true);
            getBalanceButton.gameObject.SetActive(true);
        }
        catch (Exception ex)
        {
            ShowOutput($"Failed to connect to EVM: {ex.Message}");
        }
    }

    public async void SendZkTransaction()
    {
        try
        {
            ShowOutput($"Called sendTransaction()...");
            string? response = await passport.ZkEvmSendTransaction(new TransactionRequest()
            {
                to = zkSendTransactionTo.text,
                value = zkSendTransactionValue.text,
                data = zkSendTransactionData.text

            });
            ShowOutput($"Transaction hash: {response}");
        }
        catch (Exception ex)
        {
            ShowOutput($"Failed to request accounts: {ex.Message}");
        }
    }

    public void ShowZkSendTransaction()
    {
        authenticatedCanvas.gameObject.SetActive(false);
        zkSendTransactionCanvas.gameObject.SetActive(true);
        zkSendTransactionTo.text = "";
        zkSendTransactionValue.text = "";
        zkSendTransactionData.text = "";
    }

    public void CancelZkSendTransaction()
    {
        authenticatedCanvas.gameObject.SetActive(true);
        zkSendTransactionCanvas.gameObject.SetActive(false);
    }

    public async void RequestAccounts()
    {
        try
        {
            ShowOutput($"Called RequestAccounts()...");
            List<string> accounts = await passport.ZkEvmRequestAccounts();
            ShowOutput(String.Join(", ", accounts));
        }
        catch (Exception ex)
        {
            ShowOutput($"Failed to request accounts: {ex.Message}");
        }
    }

    public async void GetBalance()
    {
        try
        {
            ShowOutput($"Called GetBalance()...");
            string balance = await passport.ZkEvmGetBalance(zkGetBalanceAccount.text);
            var balanceBI = BigInteger.Parse(balance.Replace("0x", "0"), NumberStyles.HexNumber);
            ShowOutput($"Hex: {balance}\nDec: {balanceBI.ToString()}");
        }
        catch (Exception ex)
        {
            ShowOutput($"Failed to get balance: {ex.Message}");
        }
    }

    public void ShowZkGetBalance()
    {
        authenticatedCanvas.gameObject.SetActive(false);
        zkGetBalanceCanvas.gameObject.SetActive(true);
        zkGetBalanceAccount.text = "";
    }

    public void CancelZkGetBalance()
    {
        authenticatedCanvas.gameObject.SetActive(true);
        zkGetBalanceCanvas.gameObject.SetActive(false);
    }

    private void ShowOutput(string message)
    {
        if (output != null)
        {
            output.text = message;
        }
    }

    private void clearInputs()
    {
        tokenIdInput1.text = "";
        tokenAddressInput1.text = "";
        receiverInput1.text = "";

        tokenIdInput2.text = "";
        tokenAddressInput2.text = "";
        receiverInput2.text = "";

        tokenIdInput3.text = "";
        tokenAddressInput3.text = "";
        receiverInput3.text = "";
    }
}
