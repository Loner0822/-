//---------------------------------------------------------------------------

#ifndef TPubFuncH
#define TPubFuncH
#include <vcl.h>
//---------------------------------------------------------------------------
void LoadFile( AnsiString path, char *& xcdmap, int &length );

bool CutCharStr(char*& org, char*& rec, int beg, int end, int max); //��ȡ�ַ�����

int GetTypeSize( const AnsiString &type, int &flag );  //�ж��ֶ����Ͷ�Ӧ���ֶγ���

AnsiString GetValue( char *& org, int type ); //���ַ���תΪAnsiString

int GetBitValue( BYTE b, int pos ); //��ȡһ��BYTE��ÿһbit�Ķ�����ֵ 0��������

AnsiString ParsChGuid( char* chGUID );  //תguid



#endif
