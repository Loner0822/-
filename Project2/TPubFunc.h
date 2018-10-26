//---------------------------------------------------------------------------

#ifndef TPubFuncH
#define TPubFuncH
#include <vcl.h>
//---------------------------------------------------------------------------
void LoadFile( AnsiString path, char *& xcdmap, int &length );

bool CutCharStr(char*& org, char*& rec, int beg, int end, int max); //截取字符数据

int GetTypeSize( const AnsiString &type, int &flag );  //判断字段类型对应的字段长度

AnsiString GetValue( char *& org, int type ); //将字符型转为AnsiString

int GetBitValue( BYTE b, int pos ); //获取一个BYTE中每一bit的二进制值 0代表最右

AnsiString ParsChGuid( char* chGUID );  //转guid



#endif
