//---------------------------------------------------------------------------


#pragma hdrstop

#include "logrec.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

void logrec( const AnsiString &Content )
{    
    AnsiString str = Content;
    //str=DateTimeToStr(Now()) +"  "+str;
    //unsigned short year,mon,day;
    //Now().DecodeDate(&year,&mon,&day);

    AnsiString file = "temp\\xcd.log";
    AnsiString Path = ExtractFilePath(Application->ExeName) + file;
    ofstream f1(Path.c_str(), ios::app);
    f1<<str.c_str()<<endl;
    f1.close();
}

TStringList * SplitStr( const AnsiString &str, const AnsiString &SplitFlag )
{
    TStringList *list = new TStringList;
    String temp = str;
    int SplitFlagSize = SplitFlag.Length();
    while(!temp.IsEmpty())
    {
        if( temp.Pos(SplitFlag) > 0 )
        {
            list->Add(temp.SubString(1,temp.Pos(SplitFlag)-1).Trim());
            temp = temp.SubString(temp.Pos(SplitFlag) + SplitFlagSize, temp.Length()).Trim();
        }
        if( temp.Pos(SplitFlag) == 0 )
        {
            list->Add(temp.Trim());
            temp = "";
        }
    }
    return list;
}
