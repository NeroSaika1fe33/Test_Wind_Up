using System.Collections.Generic;
using System.IO;
using System.Resources;
using UnityEngine;

//CSVの記述方法について
//一番上の行はデータ型を入れる。
//二番目の行はデータ名をいれること。
//一番左はパーツ名にすること。
public class PartsDataManager : MonoBehaviour
{
    //シングルトン
    public static PartsDataManager Instance => Singleton<PartsDataManager>.Instance;

    int _Number_of_data = 0;
    int _Number_of_Parts = 0;
    List<string> _DataType = new List<string>();                 //パーツデータのデータ型を保存する配列
    List<string> _DataName = new List<string>();                 //データ名を保存する配列
    List<string> _PartsName = new List<string>();                //パーツ名を保存する配列
    List<string> _PartsID = new List<string>();                  //パーツIDを保存する配列
    List<List<string>> _PartsData_All = new List<List<string>>();//パーツデータを保存する2次元配列
                                                                 // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {

    }
    private void Awake()
    {
        FileLoad();         //CSVのデータの読み取りと保存
                            //デバッグ表示
                            //PartsDataDebug();
                            //AllPartsData();
        DontDestroyOnLoad(gameObject);
    }

    //パーツデータのデバッグ表示
    public void PartsDataDebug()
    {
        string type = "";
        type += "データ数";
        type += _Number_of_data;
        type += ":データ名";
        for (int i = 0; i < _Number_of_data; i++)
        {
            type += "[";
            type += _DataName[i];
            type += "]";
        }
        UnityEngine.Debug.Log(type);
        string name = "";
        name += "パーツ数";
        name += _Number_of_Parts;
        name += ":パーツ名";
        for (int i = 0; i < _Number_of_Parts; i++)
        {
            name += "[";
            name += _PartsName[i];
            name += "]";
        }
        UnityEngine.Debug.Log(name);
    }

    //パーツ名とデータ名に対応する情報をstringで返す
    public string Get_PartsData_string(string PartsName, string DataName)
    {
        string result = "";
        for (int i = 0; i < _Number_of_Parts; i++)
        {
            if (_PartsName[i] == PartsName)
            {
                for (int j = 0; j < _Number_of_data; j++)
                {
                    if (_DataName[j] == DataName)
                    {
                        result = _PartsData_All[i][j];
                    }
                }
            }
        }
        return result;
        ;
    }

    //パーツ名とデータ名に対応する情報をintで返す(数値以外は0で返す)
    public int Get_PartsData_int(string PartsName, string DataName)
    {
        int result = 0;
        for (int i = 0; i < _Number_of_Parts; i++)
        {
            if (_PartsName[i] == PartsName)
            {
                for (int j = 0; j < _Number_of_data; j++)
                {
                    if (_DataName[j] == DataName)
                    {
                        if (_DataType[j] == "int")
                        {
                            result = int.Parse(_PartsData_All[i][j]);
                        }
                    }
                }
            }
        }
        return result;
    }

    //パーツ名に対応するパーツタイプを返す
    public string Get_PartsType(string PartsName)
    {
        string result = "";
        for (int i = 0; i < _Number_of_Parts; i++)
        {
            if (_PartsName[i] == PartsName)
            {
                for (int j = 0; j < _Number_of_data; j++)
                {
                    if (_DataName[j] == "パーツタイプ")
                    {
                        result = _PartsData_All[i][j];
                    }
                }
            }
        }
        return result;
    }

    //全パーツのデータを出力
    public void AllPartsData()
    {

        for (int i = 0; i < _Number_of_Parts; i++)
        {
            string result = "";
            for (int j = 0; j < _Number_of_data; j++)
            {
                result += "[";
                result += _PartsData_All[i][j];
                result += "]";
            }
            UnityEngine.Debug.Log(result);
        }
    }

    //NameをIDにする
    public string Get_PartsID(string name)
    {
        string ID = "";
        for (int i = 0; i < _Number_of_Parts; i++)
        {
            if (_PartsName[i] == name)
            {
                for (int j = 0; j < _Number_of_data; j++)
                {
                    if (_DataName[j] == "ID")
                    {
                        ID = _PartsData_All[i][j];
                    }
                }
            }
        }
        return ID;
    }

    //IDをNameにする
    public string Get_PartsName(string id)
    {
        string Name = "";
        for (int i = 0; i < _Number_of_Parts; i++)
        {
            if (_PartsID[i] == id)
            {
                for (int j = 0; j < _Number_of_data; j++)
                {
                    if (_DataName[j] == "パーツ名")
                    {
                        Name = _PartsData_All[i][j];
                    }
                }
            }
        }
        return Name;
    }
    //
    public List<string> Get_PartsName()
    {
        return _PartsName;
    }
    public int Number_of_Parts
    {
        get { return _Number_of_Parts; }
    }

    //CSVのデータを読み込んで保存する関数
    public void FileLoad()
    {
        int Number_of_data = 0;     //データ数
        int Parts_of_data = 0;      //パーツ数
        //CSVファイルの読み取り
        string DataPath = "Assets/csv/Parts_Data.csv";
        FileStream DataFile = new FileStream(DataPath, FileMode.Open, FileAccess.Read);
        StreamReader DataReader = new StreamReader(DataFile);
        string DataTypes = DataReader.ReadLine();       //データ型を取り込み
        string[] DataTypeList = DataTypes.Split(',');   //配列に変換
        //データ型情報を保存
        for (int i = 0; i < DataTypeList.Length; i++)
        {
            _DataType.Add(DataTypeList[i]);
            Number_of_data += 1;         //データ数をカウント
        }
        _Number_of_data = Number_of_data;//データ数を設定

        //データ名を保存
        string DataName = DataReader.ReadLine();        //データ名を取り込み
        string[] DataNameList = DataName.Split(',');    //配列に変換
        for (int i = 0; i < DataNameList.Length; i++)
        {
            _DataName.Add(DataNameList[i]);
        }
        //パーツデータを保存
        for (int c = 0; c < DataTypeList.Length; c++)
        {
            string PartsData = DataReader.ReadLine();       //パーツを取り込み
            if (PartsData == null) break;                   //パーツがなければ保存終了
            string[] PartsDataList = PartsData.Split(',');  //配列に変換
            _PartsName.Add(PartsDataList[0]);               //パーツ名を保存
            _PartsID.Add(PartsDataList[1]);
            Parts_of_data += 1;                             //パーツ数をカウント
            List<string> _PartsData = new List<string>();   //パーツ1つのデータを保存する配列
            for (int i = 0; i < PartsDataList.Length; i++)  //パーツデータを取り込み
            {
                _PartsData.Add(PartsDataList[i]);
            }
            _PartsData_All.Add(_PartsData);

        }
        _Number_of_Parts = Parts_of_data;     //パーツ数を設定
    }


}
