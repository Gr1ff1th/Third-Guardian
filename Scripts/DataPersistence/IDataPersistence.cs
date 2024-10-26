using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//describes the methods that the implementing script needs to have
public interface IDataPersistence
{
    void LoadData(GameData data);

    void SaveData(ref GameData data);
}
