using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{

    public static void SaveMoney(float money)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/money.epicity";
        FileStream stream = new FileStream(path, FileMode.Create);

        MoneyData data = new MoneyData(money);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static MoneyData LoadMoney()
    {
        string path = Application.persistentDataPath + "/money.epicity";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            
            MoneyData data = formatter.Deserialize(stream) as MoneyData;
            
            stream.Close();

            return data;
        }
        else
        {
            float info = 0;
            SaveMoney(info);
            return LoadMoney();
        }
    }

    public static void ResetMoney()
    {
        SaveMoney(0);
    }
}
