//---------------------------------------------------------------------------

#include <vcl.h>
#include <vector>
#include <algorithm>
#include <inifiles.hpp>
#pragma hdrstop

#include "SetUnit.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm2 *Form2;
//---------------------------------------------------------------------------
__fastcall TForm2::TForm2(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm2::FormShow(TObject *Sender)
{
    int hour, minute, second;
    second = Times % 100, minute = (Times / 100) % 100, hour = Times / 10000;
    DateTimePicker->Time = StrToTime(IntToStr(hour) + ":" + IntToStr(minute) + ":" + IntToStr(second));
    switch (Days) {
        case 1:
            ComboBox->ItemIndex = 0;
            break;
        case 2:
            ComboBox->ItemIndex = 1;
            break;
        case 4:
            ComboBox->ItemIndex = 2;
            break;
        case 7:
            ComboBox->ItemIndex = 3;
            break;
        case 15:
            ComboBox->ItemIndex = 4;
            break;
        case 30:
            ComboBox->ItemIndex = 5;
            break;
        default:
            ComboBox->ItemIndex = 0;
    }
    Edit->Text = IntToStr(Files);
}
//---------------------------------------------------------------------------

void __fastcall TForm2::BitBtnClick(TObject *Sender)
{
    String tmp = DateTimePicker->Time.FormatString("hhmmss");
    Times = StrToInt(tmp);
    tmp = ComboBox->Text;
    Days = 0;
    for (int i = 1; i <= tmp.Length(); ++ i) {
        if (tmp[i] >= '0' && tmp[i] <= '9')
            Days = Days * 10 + tmp[i] - '0';
    }
    tmp = Edit->Text;
    Files = StrToInt(tmp);

    TIniFile *ini;
    ini = new TIniFile(ExtractFilePath(Application->ExeName) + "Reg.ini");
    ini->WriteInteger("Time", "time", Times);
    ini->WriteInteger("Day", "day", Days);
    ini->WriteInteger("File", "file", Files);
	delete ini;
}
//---------------------------------------------------------------------------

String __fastcall Find_Last_Backup(String AppName) {
    TSearchRec sr;
    String path = "D:\\管理知识库数据备份\\" + AppName + "\\Auto";
    if (!DirectoryExists(path)) {
        ForceDirectories(path);
        return "";
    }
    String res = "", tmp;
    if (FindFirst(path + "\\*.dmp", faAnyFile, sr) == 0) {
        do {
            try {
                if ((sr.Attr & faDirectory) != 0) {
                    //folder

                }
                else {
                    //file
                    tmp = sr.Name;
                    if (tmp > res)
                        res = tmp;
                }
            } catch(...) { }
        } while (FindNext(sr) == 0);
        FindClose(sr);
    }
    tmp = res.SubString(1, res.Length() - 4);
    res = tmp.SubString(1, 4) + "-" + tmp.SubString(5, 2) + "-" + tmp.SubString(7, 2) + " " +
          tmp.SubString(9, 2) + ":" + tmp.SubString(11, 2) + ":" + tmp.SubString(13, 2);
    return res;
}

void __fastcall TForm2::TimerTimer(TObject *Sender)
{
    if (Files == 0 || Days == 0) {
        Days = 1, Files = 7;
    }
    TDateTime Now_DT = Now();
    String Now_T = Now_DT.FormatString("hhmmss");
    if (StrToInt(Now_T) / 100 == Times / 100) {
        String Now_DateTime = Now_DT.FormatString("YYYYMMDDhhmmss");
        TSearchRec sr, sr_dmp;
        String path = ExtractFilePath(Application->ExeName) + "Backup//";
        if (FindFirst(path + "*.ini", faAnyFile, sr) == 0) {
            do {
                try {
                    if ((sr.Attr & faDirectory) != 0) {
                        //folder
                    }
                    else {
                        String AppName = sr.Name;
                        AppName = AppName.SubString(1, AppName.Length() - 4);
                        String tmp = Find_Last_Backup(AppName);
                        if (tmp == "") {
                            TIniFile *ini;
                            ini = new TIniFile(path + sr.Name);
                            int cnt = ini->ReadInteger("table", "count", 0);
                            std::vector<String> tableName;
                            for (int i = 1; i <= cnt; ++ i) {
                                String table_tmp = ini->ReadString("table", IntToStr(i), "");
                                tableName.push_back(table_tmp);
                            }
							delete ini;
                            String bkup_path = "D:\\管理知识库数据备份\\";
                            String SignIn = "manager/zbxhzbxh@ZBXH";
                            String backup_cmd = "exp " + SignIn + " file=" + bkup_path + AppName + "\\Auto\\" + Now_DateTime + ".dmp tables=(";
                            for (int i = 0; i < cnt - 1; ++ i)
                                backup_cmd += tableName[i].c_str();
                            backup_cmd += tableName[cnt - 1].c_str();
                            backup_cmd += ")\n";
                            system(backup_cmd.c_str());
                            continue;
                        }
                        TDateTime tmp_dt = StrToDateTime(tmp);
                        int delta = (int)Now_DT - (int)tmp_dt;
                        if (delta >= Days) {
                            /*
                                解析ini
                                备份
                            */
                            TIniFile *ini;
                            ini = new TIniFile(path + sr.Name);
                            int cnt = ini->ReadInteger("table", "count", 0);
                            std::vector<String> tableName;
                            for (int i = 1; i <= cnt; ++ i) {
                                String table_tmp = ini->ReadString("table", IntToStr(i), "");
                                tableName.push_back(table_tmp);
                            }
							delete ini;
                            String bkup_path = "D:\\管理知识库数据备份\\";
                            String SignIn = "manager/zbxhzbxh@ZBXH";
                            String backup_cmd = "exp " + SignIn + " file=" + bkup_path + AppName + "\\Auto\\" + Now_DateTime + ".dmp tables=(";
                            for (int i = 0; i < cnt - 1; ++ i)
                                backup_cmd += tableName[i].c_str();
                            backup_cmd += tableName[cnt - 1].c_str();
                            backup_cmd += ")\n";
                            if (system(backup_cmd.c_str()) != 0)
                                logAuto(AppName, backup_cmd);
                            std::vector<String> backup_files;
                            if (FindFirst(bkup_path + AppName + "\\Auto\\*.dmp", faAnyFile, sr_dmp) == 0) {
                                do {
                                    try {
                                        if ((sr_dmp.Attr & faDirectory) != 0) {
                                            //folder
                                        }
                                        else {
                                            String tmp_file = sr_dmp.Name;
                                            backup_files.push_back(tmp_file);
                                        }
                                    }
                                    catch(...){}
                                } while(FindNext(sr_dmp) == 0);
                            }
                            FindClose(sr_dmp);
                            std::sort(backup_files.begin(), backup_files.end());
                            for (int i = 0; i < (int)backup_files.size() - Files; ++ i)
                                DeleteFile(bkup_path + AppName + "\\Auto\\" + backup_files[i]);
                        }
                    }
                }catch(...){}
            } while (FindNext(sr) == 0);
        }
        FindClose(sr);
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm2::FormCreate(TObject *Sender)
{
    TIniFile *ini;
    ini = new TIniFile(ExtractFilePath(Application->ExeName) + "Reg.ini");
    Times = ini->ReadInteger("Time", "time", 0);
    Days = ini->ReadInteger("Day", "day", 1);
    Files = ini->ReadInteger("File", "file", 7);
	delete ini;

    ComboBox->Items->Clear();
    ComboBox->Items->Add("1天");
    ComboBox->Items->Add("2天");
    ComboBox->Items->Add("4天");
    ComboBox->Items->Add("7天");
    ComboBox->Items->Add("15天");
    ComboBox->Items->Add("30天");
    ComboBox->Style = csDropDownList;
}
//---------------------------------------------------------------------------

