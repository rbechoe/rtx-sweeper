using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System;

public class DataTest
{
    AesCryptoServiceProvider provider;
    private byte[] Key = { 97, 141, 170, 229, 117, 115, 88, 165, 126, 136, 13, 124, 160, 224, 112, 68, 140, 64, 117, 213, 142, 182, 23, 105, 39, 144, 169, 56, 1, 129, 236, 45 };
    private byte[] Vector = { 103, 162, 191, 108, 252, 238, 217, 168, 175, 193, 68, 234, 40, 102, 206, 70 };

    public DataTest()
    {
        provider = new AesCryptoServiceProvider();

        provider.BlockSize = 128;
        provider.KeySize = 256;
        provider.Key = Key;
        provider.IV = Vector;
        provider.Mode = CipherMode.CBC;
        provider.Padding = PaddingMode.PKCS7;
    }

    public string Encrypt(string clearText)
    {
        ICryptoTransform transform = provider.CreateEncryptor();
        byte[] cryptedBytes = transform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(clearText), 0, clearText.Length);

        string str = Convert.ToBase64String(cryptedBytes);

        return str;
    }

    public string Decrypt(string cryptText)
    {
        ICryptoTransform transform = provider.CreateDecryptor();

        byte[] cryptedBytes = Convert.FromBase64String(cryptText);

        byte[] decryptedBytes = transform.TransformFinalBlock(cryptedBytes, 0, cryptedBytes.Length);

        string str = ASCIIEncoding.ASCII.GetString(decryptedBytes);

        return str;
    }

}
