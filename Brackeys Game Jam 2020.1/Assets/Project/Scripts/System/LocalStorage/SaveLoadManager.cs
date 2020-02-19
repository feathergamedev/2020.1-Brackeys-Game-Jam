using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadManager : MonoBehaviour
{

	// Use this for initialization
	public static SaveLoadManager instance;

	// Update is called once per frame
	private void Awake()
	{
		if(instance == null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}else if(instance != this)
		{
			Destroy(gameObject);
		}

        Load();
	}

	public void Save()
	{
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream filestream = File.Create(Application.persistentDataPath + "/savefile.dat");

		PlayerInfos saver = new PlayerInfos();

        //--------------------------Data-----------------------------//

        saver.CompleteLevelNum = GlobalData.CompleteLevelNum;
        saver.MuteSFX = GlobalData.MuteSFX;

        //--------------------------Data-----------------------------//
        formatter.Serialize(filestream, saver);
		filestream.Close();

	}

	public void Load()
	{
		if (File.Exists(Application.persistentDataPath + "/savefile.dat"))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream filestream = File.Open(Application.persistentDataPath + "/savefile.dat", FileMode.Open);
			PlayerInfos loader = (PlayerInfos)formatter.Deserialize(filestream);
			filestream.Close();

			//--------------------------Data-----------------------------//

            GlobalData.CompleteLevelNum = loader.CompleteLevelNum;
            GlobalData.MuteSFX = loader.MuteSFX;

            //--------------------------Data----------------------------//

        }
    }

	public void Reset()
	{
		if (File.Exists(Application.persistentDataPath + "/savefile.dat"))
		{
            File.Delete(Application.persistentDataPath + "/savefile.dat");

            GlobalData.CompleteLevelNum = 0;
            GlobalData.MuteSFX = false;
                
            Save();
		}
	}

}

[Serializable]
class PlayerInfos
{
    //--------------------------Data----------------------------//
    public int CompleteLevelNum = 0;
    public bool MuteSFX = false;
    //--------------------------Data----------------------------//

}