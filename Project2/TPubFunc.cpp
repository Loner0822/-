//---------------------------------------------------------------------------


#pragma hdrstop

#include "TPubFunc.h"
#include "logrec.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)
void LoadFile( AnsiString path, char *& xcdmap, int &length )
{
    TMemoryStream *ms = new TMemoryStream();
    ms->LoadFromFile(path);
    ms->Position = 0;
    xcdmap = new char[ms->Size];
    ms->Read( xcdmap, ms->Size );
    length = ms->Size;
}

bool CutCharStr(char*& org, char*& rec, int beg, int end, int max) //截取字符数据
{
    if(max >= end)
    {
        if(end >= beg)
        {
            rec = new char[end-beg+1];
            int i = beg;
            int j = 0;
            while(1)
            {
                if(i < end)
                {
                    rec[j] = org[i];
                    i++;
                    j++;
                }
                else
                {
                    rec[j] = '\0'; //结尾补 \0
                    break;
                }
            }
            return true;
        }
        else
        {
            logrec("截取的二进制数据时end小于beg");
            return false;
        }
    }
    else
    {
        logrec("二进制数据长度小于解析的图纸数据长度");
        return false;
    }
}

int GetTypeSize( const AnsiString &type, int &flag )
{
    if( type == "BYTE" )
    {
        flag = 1;
        return 1;
    }
    else if( type == "WORD" )
    {
        flag = 2;
        return 2;
    }
    else if( type == "float" )
    {
        flag = 3;
        return 4;
    }
    else if( type == "long" )
    {
        flag = 4;
        return 8;
    }
    else if( type == "char[4]" )
    {
        flag = 5;
        return 4;
    }else if( type == "int" )
    {
        flag = 6;
        return 4;
    }else if( type == "GUID" )
    {
        flag = 7;
        return 16;
    }else if( type == "DWORD" )
    {
        flag = 8;
        return 4;
    }else if( type == "short" )
    {
        flag = 9;
        return 2;
    }else if( type == "GValue" )
    {
        flag = 1000;
        return 1000;
    }else if( type == "GRow[]" )
    {
        flag = 1001;
        return 1001;
    }else if( type == "char[]" )
    {
        flag = 1002;
        return 1002;
    }else if( type == "TEXT" )
    {
        flag = 1003;
        return 1003;
    }
}

AnsiString GetValue( char *&org, int type )
{
    switch(type)
    {
        case 1: //BYTE
            return AnsiString(*((BYTE*)org));
        case 2://WORD unsigned short 
            return AnsiString(*((WORD*)org));
        case 3: //float
            return FloatToStr(*((float*)org));
        case 4://long
            return IntToStr(*((long*)org));
        case 5://char
            return AnsiString( org );
        case 6://int
            return IntToStr(*((int*)org));
        case 7://GUID
            return ParsChGuid( org );
        case 8:
            return AnsiString(*((DWORD*)org));
        case 9:
            return AnsiString(*((short*)org));
        default:
            return "";
    }
}

int GetBitValue( BYTE b, int pos )
{
    BYTE temp = b;
    int posVal[8] = {0};
    int i = 0;
    while(temp != 0)
    {
        posVal[i] = temp%2;
        temp = temp/2;
        i++;
    }
    if(pos >= 0 && pos < 8)
    {
        return posVal[pos];
    }
    else
        return -1; //表示此位数无效
}

AnsiString ParsChGuid( char* chGUID ) //GUID，具体格式见图纸数据格式定义
{
    AnsiString rec = "";
    char* temp;
    CutCharStr(chGUID, temp, 0, 4, 16);
    rec += AnsiString(*((DWORD*)temp)) + "+";
    delete[] temp;
    CutCharStr(chGUID, temp, 4, 6, 16);
    rec += AnsiString(*((WORD*)temp)) + "+";
    delete[] temp;
    CutCharStr(chGUID, temp, 6, 8, 16);
    rec += AnsiString(*((WORD*)temp)) + "+";
    delete[] temp;
    for(int i=0; i<8; i++)
    {
        if(CutCharStr(chGUID, temp, i+8, i+9, 16))
        {
            if(i == 7)
            {
                rec += AnsiString(*((WORD*)temp));
            }
            else
            {
                rec += AnsiString(*((WORD*)temp)) + "+";
            }
            delete[] temp;
        }
    }
    return rec;
}



