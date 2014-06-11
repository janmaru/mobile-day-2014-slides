using System;

using Encryption;

public class EncryptHelper
{
    private Symmetric.Provider _p;
    private string _k;

    public EncryptHelper(Symmetric.Provider provider, string key)
    {
        _p = provider;
        _k = key;
    }

    public string encrypt(string data)
    {
        var sym = new Symmetric(_p);
        sym.Key.Text = _k;
        return sym.Encrypt(new Encryption.Data(data)).ToHex();
    }

    public string decrypt(string code)
    {
        try
        {
            var sym = new Symmetric(_p);
            sym.Key.Text = _k;
            var d = new Encryption.Data();
            d.Hex = code;
            return sym.Decrypt(d).Text;
        }
        catch (Exception)
        {
            return string.Empty;
        }

    }
}

