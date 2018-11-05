//---------------------------------------------------------------------------

#include <vcl.h>
#include <vector>
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
    ComboBox->Items->Clear();
    ComboBox->Items->Add("1天");
    ComboBox->Items->Add("2天");
    ComboBox->Items->Add("4天");
    ComboBox->Items->Add("7天");
    ComboBox->Items->Add("15天");
    ComboBox->Items->Add("30天");
    ComboBox->ItemIndex = 0;
    ComboBox->Style = csDropDownList;
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
    TDateTime Now_DT = Now();
    String Now_T = Now_DT.FormatString("hhmmss");
    if (StrToInt(Now_T) / 100 == Times / 100) {
        String Now_DateTime = Now_DT.FormatString("YYYYMMDDhhmmss");
        TSearchRec sr;
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
                        int delta = Now_DT - tmp_dt;
                        if (delta > Days) {
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
                            String bkup_path = "D:\\管理知识库数据备份\\";
                            String SignIn = "manager/zbxhzbxh@ZBXH";
                            String backup_cmd = "exp " + SignIn + " file=" + bkup_path + AppName + "\\Auto\\" + Now_DateTime + ".dmp tables=(";
                            for (int i = 0; i < cnt - 1; ++ i)
                                backup_cmd += tableName[i].c_str();
                            backup_cmd += tableName[cnt - 1].c_str();
                            backup_cmd += ")\n";
                            if (system(backup_cmd.c_str()) != 0)
                                logAuto(AppName, backup_cmd);
                        }
                    }
                }catch(...){}
            } while (FindNext(sr) == 0);
        }
        FindClose(sr);
    }
}
//---------------------------------------------------------------------------

