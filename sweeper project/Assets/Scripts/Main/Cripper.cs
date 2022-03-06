using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System;

public class CryptoManager
{
    AesCryptoServiceProvider provider;

    public CryptoManager()
    {
        provider = new AesCryptoServiceProvider();

        provider.BlockSize = 128;
        provider.KeySize = 256;
        provider.Key = Constances.levels;
        provider.IV = Constances.difficulties;
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
