//---------------------------------------------------------------------------

#ifndef ExceptLogH
#define ExceptLogH
//---------------------------------------------------------------------------
#include <stdio.h>
#include <vcl.h>
void SaveLog(String path,String Msg)   //�������Ϣ�ĺ���
{
    AnsiString str ="";
    str=DateTimeToStr(Now()) +"  "+ Msg +"\n";
    FILE *fp;
    fp=fopen(path.c_str(),"a");
    fputs(str.c_str(),fp);
    fclose(fp);
}
#endif
