﻿using System;
using CoreNFC;
using TID.NFC.Maui.TestIdea.Enums;
using UIKit;
using TID.NFC.Maui.TestIdea.Interfaces;

namespace TID.NFC.Maui.TestIdea.Platforms.iOS;

public class NfcService : INfcService
{
    public async Task SendAsync(byte[] bytes)
    {
        var isNfcAvailable = UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
        if (isNfcAvailable && NFCNdefReaderSession.ReadingAvailable)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var sessionDelegate = new SessionDelegate(bytes);
                    var session = new NFCNdefReaderSession(sessionDelegate, null, true);
                    session.BeginSession();

                    var status = await sessionDelegate.WasDataTransmitted.Task;
                    if (status != NfcTransmissionStatus.Success)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Suitable error message", "Ok");
                    }

                }
                catch
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "There was an error while trying to create a NFC session", "Ok");
                }
            });
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Read is not supported by this tag", "Ok");
        }
    }
}

